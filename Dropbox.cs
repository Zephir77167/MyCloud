using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloud;
using System.Windows;
using Dropbox.Api;
using System.Net;
using System.Net.Http;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using System.Threading;
using DropboxRestAPI;

namespace Mycloud
{
    public class Dropbox : IStorage
    {
        private const string apiKey = "eo9ia55b510q9tr";
        private const string apiSecretKey = "vo6zz2nxo15jxkf";
        
        private string _path = string.Empty;
        private List<string> _folders = new List<string>();
        private List<string> _files = new List<string>();
        public string icone { get; set; }
        public string name { get; set; }
        public Dropbox()
        {
            icone = "Resources/dropbox.png";
            name = "test DropBox cloud";
            Connect();
        }

        public async void Connect()
        {
            var options = new DropboxRestAPI.Options
            {
                ClientId = apiKey,
                ClientSecret = apiSecretKey,
                RedirectUri = "http://localhost/mycloud"
            };

            // Initialize a new Client (without an AccessToken)
            var client = new DropboxRestAPI.Client(options);
            // Get the OAuth Request Url
            var authRequestUrl = client.Core.OAuth2.Authorize("code");

            var connectWindow = new DropBoxConnect(options, authRequestUrl.AbsoluteUri);
            connectWindow.ShowDialog();
            if (!connectWindow.Result)
                return;
            // Exchange the Authorization Code with Access/Refresh tokens
            var token = await client.Core.OAuth2.TokenAsync(connectWindow.authorizeCodeUrl);

            // Get account info
            var accountInfo = await client.Core.Accounts.AccountInfoAsync();
            Console.WriteLine("Uid: " + accountInfo.uid);
            Console.WriteLine("Display_name: " + accountInfo.display_name);
            Console.WriteLine("Email: " + accountInfo.email);

            // Get root folder without content
            var rootFolder = await client.Core.Metadata.MetadataAsync("/", list: false);
            Console.WriteLine("Root Folder: {0} (Id: {1})", rootFolder.Name, rootFolder.path);

            // Get root folder with content
            rootFolder = await client.Core.Metadata.MetadataAsync("/", list: true);
            foreach (var folder in rootFolder.contents)
            {
                Console.WriteLine(" -> {0}: {1} (Id: {2})",
                    folder.is_dir ? "Folder" : "File", folder.Name, folder.path);
            }
            /*
            // Initialize a new Client (with an AccessToken)
            var client2 = new DropboxRestAPI.Client(options);

            // Create a new folder
            var newFolder = await client2.Core.FileOperations.CreateFolderAsync("/New Folder");

            // Find a file in the root folder
            var file = rootFolder.contents.FirstOrDefault(x => x.is_dir == false);

            // Download a file
            var tempFile = Path.GetTempFileName();
            using (var fileStream = System.IO.File.OpenWrite(tempFile))
            {
                await client2.Core.Metadata.FilesAsync(file.path, fileStream);
            }

            //Upload the downloaded file to the new folder

            using (var fileStream = System.IO.File.OpenRead(tempFile))
            {
                var uploadedFile = await client2.Core.Metadata.FilesPutAsync(fileStream, newFolder.path + "/" + file.Name);
            }

            // Search file based on name
            var searchResults = await client2.Core.Metadata.SearchAsync("/", file.Name);
            foreach (var searchResult in searchResults)
            {
                Console.WriteLine("Found: " + searchResult.path);
            }
            */
        }

        public void UpdateFileAndFolderList()
        {
            
        }

        public List<string> GetBucketList()
        {
            List<string> buckets = new List<string>();

            return (buckets);
        }

        public List<string> GetFolderList()
        {
            _folders.Clear();

            //var task = Task.Run((Func<Task>)Dropbox.ListFolderIn);
            //task.Wait();

            return (_folders);
        }


        public List<string> GetFileList()
        {
            _files.Clear();

            //var task = Task.Run((Func<Task>)Dropbox.ListFilesIn);
            //task.Wait();

            return (_files);
        }



        public void GoToFolder(string folderName)
        {
            _path += "/" + folderName;
        }

        public void GoBackToParent()
        {
            int index = _path.LastIndexOf('/');

            _path = ((index == -1) ? (string.Empty) : (_path.Substring(0, index)));
        }

        public bool DownloadFile(string file, string downloadPath)
        {
            //var task = Task.Run((Func<Task>)Dropbox.Download);
            //task.Wait();

            return (true);
        }


    }
}