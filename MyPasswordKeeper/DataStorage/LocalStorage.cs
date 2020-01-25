using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
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


        public async Task<List<Identity>> Load(string password)
        {
            using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
            using var zf = new ZipFile(fs)
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

        public async Task<bool> Upload(MemoryStream data)
        {
            await File.WriteAllBytesAsync(_path, data.ToArray());
            return true;
        }
    }
}
