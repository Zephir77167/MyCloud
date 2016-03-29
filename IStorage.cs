using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCloud
{
    public interface IStorage
    {
        string icone { get; set; }
        string name { get; set; }
        void Connect();
        void UpdateFileAndFolderList();
        List<DirectoryObject> listDirectory();
        void GoToFolder(string folderName);
        void GoBackToParent();
        bool DownloadFile(string file, string downloadPath);
    }
}
