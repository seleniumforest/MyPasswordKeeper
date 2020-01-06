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

        public static MemoryStream CreateArchive(string password, IEnumerable<Identity> identities)
        {
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

                return output;
            }
        }
    }
}
