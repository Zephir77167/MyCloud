using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MyCloud;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util.Store;

namespace Mycloud
{
    public class GoogleDrive : IStorage
    {
        private IConfigurableHttpClientInitializer _credentials;
        private DriveService _service;
        private string _currFolder = "root";
        private List<Google.Apis.Drive.v2.Data.File> _folders = new List<Google.Apis.Drive.v2.Data.File>();
        private List<Google.Apis.Drive.v2.Data.File> _files = new List<Google.Apis.Drive.v2.Data.File>();

        private const int KB = 0x400;
        private const int DownloadChunkSize = 256 * KB;

        public GoogleDrive(string userName, string password)
        {
            Connect(userName, password);
        }

        public void Connect(string userName, string password)
        {
            _credentials = GetInstalledApplicationCredentials(userName, password);

            _service = new DriveService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credentials,
                    ApplicationName = "MyCloud",
                });
        }

        public IConfigurableHttpClientInitializer GetInstalledApplicationCredentials(string userName, string password)
        {
            string[] scopes = new string[] { DriveService.Scope.Drive,
                                 DriveService.Scope.DriveFile};
            var clientId = "113374830036-c6vu0c9c11p4l5a8p1c69d6gngrlv2q9.apps.googleusercontent.com";
            var clientSecret = "A-7CN0F8Xwz9egABvDG-VKS1";

            var secrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                scopes,
                Environment.UserName,
                CancellationToken.None,
                new FileDataStore("MyCloud.GoogleDrive.Auth.Store." + Guid.NewGuid()))
                .Result;
        }

        public void UpdateFileAndFolderList()
        {
            _files.Clear();
            ChildrenResource.ListRequest request = _service.Children.List(_currFolder);

            do
            {
                try
                {
                    ChildList children = request.Execute();

                    foreach (ChildReference child in children.Items)
                    {
                        try
                        {
                            Google.Apis.Drive.v2.Data.File file = _service.Files.Get(child.Id).Execute();

                            if (file.MimeType == "application/vnd.google-apps.file")
                            {
                                _files.Add(file);
                            }
                            else if (file.MimeType == "application/vnd.google-apps.folder")
                            {
                                _folders.Add(file);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: " + e.Message);
                        }
                    }
                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
        }

        public List<string> GetFolderList()
        {
            List<string> folders = new List<string>();

            foreach (Google.Apis.Drive.v2.Data.File folder in _folders)
            {
                folders.Add(folder.Title);
            }

            return (folders);
        }

        public List<string> GetFileList()
        {
            List<string>    files = new List<string>();

            foreach (Google.Apis.Drive.v2.Data.File file in _files)
            {
                files.Add(file.Title);
            }

            return (files);
        }

        public void GoToFolder(string folderName)
        {
            foreach (var folder in _folders)
            {
                if (folder.Title == folderName)
                {
                    _currFolder = folder.Id;
                }
            }
            
        }

        public void GoBackToParent()
        {
            ParentsResource.ListRequest request = _service.Parents.List(_currFolder);

            try
            {
                ParentList parents = request.Execute();

                _currFolder = parents.Items[0].Id;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public bool DownloadFile(string fileName)
        {
            Google.Apis.Drive.v2.Data.File fileToDownload = null;

            foreach (var file in _files)
            {
                if (file.Title == fileName)
                {
                    fileToDownload = file;
                }
            }

            if (fileToDownload == null)
            {
                return (false);
            }

            if (!String.IsNullOrEmpty(fileToDownload.DownloadUrl))
            {
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(fileToDownload.DownloadUrl);
                    byte[] arrBytes = x.Result;
                    System.IO.File.WriteAllBytes("./", arrBytes);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return false;
                }
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return false;
            }
        }
    }
}