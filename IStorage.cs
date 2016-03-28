using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCloud
{
    interface IStorage
    {
        void Connect(string userName, string password);
        List<string> GetBucketList();
        List<string> GetFolderList();
        List<string> GetFileList();
        void GoToFolder(string folder);
        void GoBackToParent();
        void DownloadFile(string file);
    }
}
