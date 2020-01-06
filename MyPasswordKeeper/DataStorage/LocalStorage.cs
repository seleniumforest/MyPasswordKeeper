using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyPasswordKeeper.DataStorage
{
    class LocalStorage : IDataStorage
    {
        private static string archiveName => "userdata.zip";
        private static string appDataFolderName => "MyPasswordKeeper";
        public static string pathToArchiveFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataFolderName);
        public static string pathToArchive => Path.Combine(pathToArchiveFolder, archiveName);
        public static bool isArchiveExists => File.Exists(pathToArchive);


        private string _path;
        public LocalStorage(string path)
        {
            this._path = path;
        }

        public LocalStorage()
        {
            _path = pathToArchive;
        }


        public async Task<(List<Identity> identities, bool success, string message)> Load(string password)
        {
            using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
            using var zf = new ZipFile(fs)
            {
                Password = password
            };

            try
            {
                var ze = zf.GetEntry(Storage.fileNameInArchive);
                if (ze == null)
                {
                    throw new ArgumentException(Labels.CantOpenArchive);
                }

                using var s = zf.GetInputStream(ze);
                using var sr = new StreamReader(s);
                var data = await sr.ReadToEndAsync();

                return (Helpers.Deserialize(data), true, string.Empty);
            }
            catch (ArgumentException)
            {
                return (null, false, Labels.CantOpenArchive); //todo Create another message
            }
            catch (ZipException)
            {
                return (null, false, Labels.CantOpenArchive);
            }
        }

        public async Task<(bool success, string message)> Upload(MemoryStream data)
        {
            try
            {
                await File.WriteAllBytesAsync(_path, data.ToArray());
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
