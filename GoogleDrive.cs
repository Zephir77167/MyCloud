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
        private string _root = "root";
        private List<Google.Apis.Drive.v2.Data.File> _folders = new List<Google.Apis.Drive.v2.Data.File>();
        private List<Google.Apis.Drive.v2.Data.File> _files = new List<Google.Apis.Drive.v2.Data.File>();

        private const int KB = 0x400;
        private const int DownloadChunkSize = 256 * KB;
        public string icone { get; set; }
        public string name { get; set; }
        public GoogleDrive()
        {
            icone = "Resources/googledrive.png";
            Connect();

            try
            {
                About about = _service.About.Get().Execute();

                name = about.Name;
                _currFolder = about.RootFolderId;
                _root = _currFolder;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            UpdateFileAndFolderList();
        }

        public void Connect()
        {
            _credentials = GetInstalledApplicationCredentials();

            _service = new DriveService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credentials,
                    ApplicationName = "MyCloud",
                });
        }

        public IConfigurableHttpClientInitializer GetInstalledApplicationCredentials()
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
                new FileDataStore("MyCloud/MyCloud.GoogleDrive.Auth.Store." + Guid.NewGuid()))
                .Result;
        }

        public void UpdateFileAndFolderList()
        {
            _files.Clear();
            _folders.Clear();

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

                            if (file.MimeType == "application/vnd.google-apps.folder")
                            {
                                _folders.Add(file);
                            }
                            else
                            {
                                _files.Add(file);
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

            UpdateFileAndFolderList();
        }

        public void GoBackToParent()
        {
            if (_currFolder == _root)
            {
                return;
            }

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

            UpdateFileAndFolderList();
        }

        public bool DownloadFile(string fileName, string downloadPath)
        {
            Google.Apis.Drive.v2.Data.File fileToDownload = null;
            string downloadUrl;

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

            if (!String.IsNullOrEmpty(downloadUrl = fileToDownload.DownloadUrl)
                || (fileToDownload.ExportLinks != null
                    && ((fileToDownload.ExportLinks.ContainsKey("application/vnd.oasis.opendocument.text")
                            && !String.IsNullOrEmpty(downloadUrl = fileToDownload.ExportLinks["application/vnd.openxmlformats-officedocument.wordprocessingml.document"]))
                        || (fileToDownload.ExportLinks.ContainsKey("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                            && !String.IsNullOrEmpty(downloadUrl = fileToDownload.ExportLinks["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]))
                        || (fileToDownload.ExportLinks.ContainsKey("application/pdf")
                            && !String.IsNullOrEmpty(downloadUrl = fileToDownload.ExportLinks["application/pdf"]))
                        || (fileToDownload.ExportLinks.ContainsKey("application/zip")
                            && !String.IsNullOrEmpty(downloadUrl = fileToDownload.ExportLinks["application/zip"])))))
            {
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(downloadUrl);
                    byte[] arrBytes = x.Result;

                    string extension;
                    if (fileToDownload.ExportLinks.ContainsKey("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                    {
                        extension = ".docx";
                    }
                    else if (fileToDownload.ExportLinks.ContainsKey("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                    {
                        extension = ".xls";
                    }
                    else if (fileToDownload.ExportLinks.ContainsKey("application/pdf"))
                    {
                        extension = ".pdf";
                    }
                    else if (fileToDownload.ExportLinks.ContainsKey("application/zip"))
                    {
                        extension = ".zip";
                    }
                    else
                    {
                        extension = ".txt";
                    }

                    System.IO.File.WriteAllBytes(downloadPath + "\\" + fileToDownload.Title + extension, arrBytes);
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