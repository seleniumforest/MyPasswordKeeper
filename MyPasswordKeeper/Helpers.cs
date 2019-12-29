using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace MyPasswordKeeper
{
    public static class Helpers
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static List<Identity> Deserialize(string data) => JsonSerializer.Deserialize<List<Identity>>(data, options);

        public static string Serialize(IEnumerable<Identity> data) => JsonSerializer.Serialize(data, options);

        public static async Task<(List<Identity> identities, bool success)> TryLoadArchive(string path, string pass)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var zf = new ZipFile(fs)
            {
                Password = pass
            };

            try
            {
                var ze = zf.GetEntry(Storage.fileNameInArchive);
                if (ze == null)
                {
                    throw new ArgumentException($"{Storage.fileNameInArchive} not found");
                }

                using var s = zf.GetInputStream(ze);
                using var sr = new StreamReader(s);
                var data = await sr.ReadToEndAsync();

                return (Deserialize(data), true);
            }
            catch (ZipException)
            {
                return (null, false);
            }
        }

        public static async Task<bool> TrySaveArchive(string password, IEnumerable<Identity> identities, string path = "")
        {
            if (string.IsNullOrEmpty(path))
                path = UserSettings.pathToArchive;

            if (!UserSettings.isArchiveExists)
                Directory.CreateDirectory(UserSettings.pathToArchiveFolder);

            var data = Serialize(identities);
            using (var output = new MemoryStream())
            {
                using (var zipStream = new ZipOutputStream(output))
                {
                    zipStream.SetLevel(9);

                    if (!string.IsNullOrEmpty(password))
                    {
                        zipStream.Password = password;
                    }

                    var newEntry = new ZipEntry(Storage.fileNameInArchive) { DateTime = DateTime.Now, IsUnicodeText = true };
                    zipStream.PutNextEntry(newEntry);

                    StreamUtils.Copy(new MemoryStream(Encoding.UTF8.GetBytes(data)), zipStream, new byte[4096]);
                }

                await File.WriteAllBytesAsync(path, output.ToArray());
                return true;
            }
        }
    }
}
