using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Mycloud
{
    public class GoogleDrive
    {
        private IConfigurableHttpClientInitializer _credentials;
        public GoogleDrive()
        {
            _credentials = GetInstalledApplicationCredentials();

            Run(_credentials);
        }

        private const int KB = 0x400;
        private const int DownloadChunkSize = 256 * KB;

        public IConfigurableHttpClientInitializer
            GetInstalledApplicationCredentials()
        {
            var secrets = new ClientSecrets
            {
                // TODO: récupérer les infos de connexion via une fenêtre
                ClientId = "YOUR_CLIENT_ID.apps.googleusercontent.com",
                ClientSecret = "YOUR_CLIENT_SECRET"
            };
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets, new[] { StorageService.Scope.DevstorageFullControl },
                Environment.UserName, new CancellationTokenSource().Token)
                .Result;
        }

        public void Run(IConfigurableHttpClientInitializer credential)
        {
            string projectId = "";
            string bucketName = "";

            StorageService service = new StorageService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "GCS Sample",
                });

            Console.WriteLine("List of buckets in current project");
            // TODO: c'est quoi projectId ? -> le récup
            Buckets buckets = service.Buckets.List(projectId).Execute();

            foreach (var bucket in buckets.Items)
            {
                Console.WriteLine(bucket.Name);
            }

            // TODO: choisir un bucketName

            Console.WriteLine("Total number of items in bucket: "
                + buckets.Items.Count);
            Console.WriteLine("=============================");

            // using Google.Apis.Storage.v1.Data.Object to disambiguate from
            // System.Object
            Google.Apis.Storage.v1.Data.Object fileobj =
                new Google.Apis.Storage.v1.Data.Object()
                {
                    Name = "somefile.txt"
                };

            Console.WriteLine("Creating " + fileobj.Name + " in bucket "
                + bucketName);
            byte[] msgtxt = Encoding.UTF8.GetBytes("Lorem Ipsum");

            service.Objects.Insert(fileobj, bucketName,
                new MemoryStream(msgtxt), "text/plain").Upload();

            Console.WriteLine("Object created: " + fileobj.Name);

            Console.WriteLine("=============================");

            Console.WriteLine("Reading object " + fileobj.Name + " in bucket: "
                + bucketName);
            var req = service.Objects.Get(bucketName, fileobj.Name);
            Google.Apis.Storage.v1.Data.Object readobj = req.Execute();

            Console.WriteLine("Object MediaLink: " + readobj.MediaLink);

            // download using Google.Apis.Download and display the progress
            string pathUser = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);
            var fileName = Path.Combine(pathUser, "Downloads") + "\\"
                + readobj.Name;
            Console.WriteLine("Starting download to " + fileName);
            var downloader = new MediaDownloader(service)
            {
                ChunkSize = DownloadChunkSize
            };
            // add a delegate for the progress changed event for writing to
            // console on changes
            downloader.ProgressChanged += progress =>
                Console.WriteLine(progress.Status + " "
                + progress.BytesDownloaded + " bytes");

            using (var fileStream = new System.IO.FileStream(fileName,
                System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var progress =
                    downloader.Download(readobj.MediaLink, fileStream);
                if (progress.Status == DownloadStatus.Completed)
                {
                    Console.WriteLine(readobj.Name
                        + " was downloaded successfully");
                }
                else
                {
                    Console.WriteLine("Download {0} was interrupted. Only {1} "
                    + "were downloaded. ",
                        readobj.Name, progress.BytesDownloaded);
                }
            }
            Console.WriteLine("=============================");
        }
    }
}