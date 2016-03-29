﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCloud;
using DropNet;

namespace Mycloud
{
    public class Dropbox : IStorage
    {
        private DropNetClient _client;
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

        public void Connect()
        {
            try {
                _client = new DropNetClient("eo9ia55b510q9tr", "eo9ia55b510q9tr");
                _client.GetToken();
                var authorizeUrl = _client.BuildAuthorizeUrl();

                System.Diagnostics.Process.Start(authorizeUrl);
            }
            catch (DropNet.Exceptions.DropboxRestException e) {
            }
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