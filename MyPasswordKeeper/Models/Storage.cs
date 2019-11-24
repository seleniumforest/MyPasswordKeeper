using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyPasswordKeeper.Models
{
    public class Storage
    {
        public string PathToArchive { get; set; }
        public string Password { get; set; }
        public IEnumerable<Identity> Data { get; set; }
        private string fileNameInArchive => "userdata";

        public async Task<bool> IsValidStorage()
        {
            using var fs = new FileStream(PathToArchive, FileMode.Open, FileAccess.Read);
            using var zf = new ZipFile(fs)
            {
                Password = Password
            };

            try
            {
                var ze = zf.GetEntry(fileNameInArchive);
                if (ze == null)            
                    return false;
                using var s = zf.GetInputStream(ze);
            }
            catch (ZipException)
            {
                return false;
            }

            return true;
        }

        public async Task<List<Identity>> Load()
        {
            using var fs = new FileStream(PathToArchive, FileMode.Open, FileAccess.Read);
            using var zf = new ZipFile(fs)
            {
                Password = Password
            };
            var ze = zf.GetEntry(fileNameInArchive);
            if (ze == null)
            {
                throw new ArgumentException($"{fileNameInArchive} not found");
            }

            using var s = zf.GetInputStream(ze);
            using var sr = new StreamReader(s);
            var data = await sr.ReadToEndAsync();

            return data.Split('\n').Select(x =>
            {
                var s = x.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return s.Length != 3 ? new Identity() : new Identity()
                {
                    ServiceName = s[0],
                    Username = s[1],
                    Password = s[2]
                };
            }).Where(x => x.IsValid()).ToList();
        }

        public async Task Save()
        {
            var data = string.Join('\n', Data.Select(x => $"{x.ServiceName} {x.Username} {x.Password}"));
            using (var output = new MemoryStream())
            {
                using (var zipStream = new ZipOutputStream(output))
                {
                    zipStream.SetLevel(9);

                    if (!string.IsNullOrEmpty(Password))
                    {
                        zipStream.Password = Password;
                    }

                    var newEntry = new ZipEntry(fileNameInArchive) { DateTime = DateTime.Now };
                    zipStream.PutNextEntry(newEntry);

                    StreamUtils.Copy(new MemoryStream(Encoding.UTF8.GetBytes(data)), zipStream, new byte[4096]);
                }

                await File.WriteAllBytesAsync(PathToArchive, output.ToArray());
            }
        }
    }
}
