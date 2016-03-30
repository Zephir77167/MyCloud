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
using Nito.AsyncEx;

namespace Mycloud
{
    public class Dropbox : IStorage
    {
        private const string apiKey = "eo9ia55b510q9tr";
        private const string apiSecretKey = "vo6zz2nxo15jxkf";

        private DropboxRestAPI.Models.Core.MetaData _directoryList;
        private DropboxRestAPI.Client _client;
        private string _path = "/";
        private List<DirectoryObject> _itemList = new List<DirectoryObject>();
        public string icone { get; set; }
        public string name { get; set; }
        public Dropbox()
        {
            icone = "Resources/dropbox.png";
            AsyncContext.Run(Connecting);
        }

        public void Connect()
        {
            AsyncContext.Run(Connecting);
        }

        private async Task Connecting()
        {
            var options = new DropboxRestAPI.Options
            {
                ClientId = apiKey,
                ClientSecret = apiSecretKey,
                RedirectUri = "http://localhost/mycloud"
            };


            _client = new DropboxRestAPI.Client(options);
            var authRequestUrl = _client.Core.OAuth2.Authorize("code");

            var connectWindow = new DropBoxConnect(options, authRequestUrl.AbsoluteUri);
            connectWindow.ShowDialog();
            if (!connectWindow.Result)
                return;

            var token = await _client.Core.OAuth2.TokenAsync(connectWindow.authorizeCodeUrl);

            var accountInfo = await _client.Core.Accounts.AccountInfoAsync();
            name = accountInfo.email;

            await GetRemoteDirectory();
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

        public async Task GetRemoteDirectory()
        {
            _directoryList = await _client.Core.Metadata.MetadataAsync(_path, list: true);
        }

        public List<DirectoryObject> listDirectory()
        {
            _itemList.Clear();
            if (_directoryList == null || _directoryList.contents == null)
                return _itemList;
            foreach (var item in _directoryList.contents)
                _itemList.Add(new DirectoryObject(item.Name, (item.is_dir) ? DirectoryObject.objectType.Folder : DirectoryObject.objectType.File)
                    { size = item.size, lastModifDate = item.modified } );
            return (_itemList);
        }

        public void GoToFolder(string folderName)
        {
            foreach (var folder in _itemList)
            {
                if (folder.type == DirectoryObject.objectType.Folder && folder.name == folderName)
                {
                    if (_path != "/")
                        _path += "/";
                    _path += folder.name;
                }
            }
        }

        public void GoBackToParent()
        {
            if (_path == "/")
                return;
            int index = _path.LastIndexOf('/');
            _path = ((index == -1) ? _path : (_path.Substring(0, index)));
        }

        public bool DownloadFile(string file, string downloadPath)
        {
            DownloadFileAsync(_path + file, downloadPath + "\\"+ file);
            return (true);
        }

        public async void DownloadFileAsync(string file, string downloadPath)
        {
            var fileStream = System.IO.File.OpenWrite(downloadPath);
            await _client.Core.Metadata.FilesAsync(file, fileStream);
            fileStream.Close();
        }

        public void UpdateFileAndFolderList()
        {
            AsyncContext.Run(GetRemoteDirectory);
        }

    }
}