using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace MyPasswordKeeper.ArchiveWorker
{
    class ZipArchiveWorker : IArchiveWorker
    {
        public async Task<MemoryStream> CreateArchive(string password, IEnumerable<Identity> identities)
        {
            var data = JsonSerializer.Serialize(identities, Storage.serializerOptions);
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

                    await new MemoryStream(Encoding.UTF8.GetBytes(data)).CopyToAsync(zipStream, 4096);
                }

                return output;
            }
        }

        public async Task<IEnumerable<Identity>> GetDataFromArchive(MemoryStream stream, string password)
        {
            using var zf = new ZipFile(stream)
            {
                Password = password
            };

            var ze = zf.GetEntry(Storage.fileNameInArchive);
            if (ze == null)
            {
                throw new ArgumentException(Labels.CantOpenArchive);
            }

            using var s = zf.GetInputStream(ze);
            using var sr = new StreamReader(s);
            var data = await sr.ReadToEndAsync();

            return JsonSerializer.Deserialize<List<Identity>>(data, Storage.serializerOptions);
        }
    }
}
