using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using ICSharpCode.SharpZipLib.Zip;
using MyPasswordKeeper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;

namespace MyPasswordKeeper.DataStorage
{
    /// <summary>
    /// todo integration with google cloud
    /// </summary>
    /// 

    class GoogleDriveStorage : IDataStorage
    {
        static string[] scopes = { DriveService.Scope.DriveFile };
        static string ApplicationName = "MyPasswordKeeper";

        public async Task<List<Identity>> Load(string password)
        {
            var svc = await getService();
            FilesResource.ListRequest listRequest = (await getService()).Files.List();
            listRequest.PageSize = 1;
            listRequest.Fields = "nextPageToken, files(id, name)";

            var files = await listRequest.ExecuteAsync();

            var file = files.Files.FirstOrDefault();

            var req = svc.Files.Get(file.Id);
            var str = new MemoryStream();
            await req.DownloadAsync(str);

            using var zf = new ZipFile(str)
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
            var fileMetadata = new File()
            {
                Name = "userdata.zip"
            };

            using (MemoryStream ms = new MemoryStream(data.ToArray()))
            {
                FilesResource.CreateMediaUpload request = (await getService()).Files.Create(fileMetadata, ms, "application/zip");
                request.Fields = "id";
                var result = await request.UploadAsync();

                if (result.Status == UploadStatus.Completed)
                    return true;
                else
                    return false;
            }
        }

        private async Task<DriveService> getService()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}
