using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using MyCloud;

namespace Mycloud
{
    public class Dropbox : IStorage
    {
        private DropboxClient _credentials;
        private string _path = string.Empty;
        private List<string> _folders = new List<string>();
        private List<string> _files = new List<string>();

        public void Connect()
        {
            //TODO: get token from client
            _credentials = new DropboxClient("YOUR ACCESS TOKEN");
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

        async Task ListFolderIn() // string.empty to list in root
        {
            var list = await _credentials.Files.ListFolderAsync(_path);

            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                _folders.Add(item.Name);
                // tmp
                Console.WriteLine("D  {0}/", item.Name);
            }
        }

        public List<string> GetFileList()
        {
            _files.Clear();

            //var task = Task.Run((Func<Task>)Dropbox.ListFilesIn);
            //task.Wait();

            return (_files);
        }

        async Task ListFilesIn() // string.Empty to list in root
        {
            var list = await _credentials.Files.ListFolderAsync(_path);

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                _files.Add(item.Name);
                // tmp
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }

        public void GoToFolder(string folder)
        {
            _path += "/" + folder;
        }

        public void GoBackToParent()
        {
            int index = _path.LastIndexOf('/');

            _path = ((index == -1) ? (string.Empty) : (_path.Substring(0, index)));
        }

        public void DownloadFile(string file)
        {
            //var task = Task.Run((Func<Task>)Dropbox.Download);
            //task.Wait();
        }

        async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                // tmp
                Console.WriteLine(await response.GetContentAsStringAsync());
            }
        }
    }
}