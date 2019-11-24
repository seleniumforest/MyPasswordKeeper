using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyPasswordKeeper
{
    public static class Helpers
    {
        public static List<Identity> StringToIdentities(string data)
        {
            return data.Split('\n').Select(x =>
            {
                var s = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return s.Length != 3 ? null : new Identity()
                {
                    ServiceName = s[0],
                    Username = s[1],
                    Password = s[2]
                };
            }).Where(x => x.IsValid() && x != null).ToList();
        }

        public static string IdentitiesToString(IEnumerable<Identity> data)
        {
            return string.Join('\n', data.Select(x => $"{x.ServiceName} {x.Username} {x.Password}"));
        }

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

                return (StringToIdentities(data), true);
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

            var data = IdentitiesToString(identities);
            using (var output = new MemoryStream())
            {
                using (var zipStream = new ZipOutputStream(output))
                {
                    zipStream.SetLevel(9);

                    if (!string.IsNullOrEmpty(password))
                    {
                        zipStream.Password = password;
                    }

                    var newEntry = new ZipEntry(Storage.fileNameInArchive) { DateTime = DateTime.Now };
                    zipStream.PutNextEntry(newEntry);

                    StreamUtils.Copy(new MemoryStream(Encoding.UTF8.GetBytes(data)), zipStream, new byte[4096]);
                }

                await File.WriteAllBytesAsync(path, output.ToArray());
                return true;
            }
        }
    }
}
