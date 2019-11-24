using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyPasswordKeeper.Models
{
    public static class UserSettings
    {
        static string archiveName => "userdata.zip";
        static string appDataFolderName => "MyPasswordKeeper";
        public static string pathToArchiveFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataFolderName);
        public static string pathToArchive => Path.Combine(pathToArchiveFolder, archiveName);
        public static bool isArchiveExists => File.Exists(pathToArchive);
    }
}
