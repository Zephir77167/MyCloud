using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace MyCloud
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IStorage currentCloud { get; set; }
        public ObservableCollection<IStorage> cloudList { get; set; }
        public ObservableCollection<DirectoryObject> directoryList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            cloudList = new ObservableCollection<IStorage>();
            directoryList = new ObservableCollection<DirectoryObject>();
            this.DataContext = this;
        }
        private void CloudConnect(object sender, RoutedEventArgs e)
        {
            ConnectWindow loginBox = new ConnectWindow(this);
            loginBox.Show();
        }
        private void CloudDisconnect(object sender, RoutedEventArgs e)
        {

        }
        private void CloudRefresh(object sender, RoutedEventArgs e)
        {
            DirectoryRefreshFromCloud(currentCloud);
        }

        public void DirectoryRefreshFromCloud(IStorage cloud)
        {
            if (cloud == null)
                return ;
            cloud.UpdateFileAndFolderList();
            directoryList.Clear();
            directoryList.Add(new DirectoryObject("..", DirectoryObject.objectType.ReactiveName));
            List<string> folders = cloud.GetFolderList();
            foreach (string folderName in folders)
                directoryList.Add(new DirectoryObject(folderName, DirectoryObject.objectType.Folder));
            List<string> files = cloud.GetFileList();
            foreach (string fileName in files)
                directoryList.Add(new DirectoryObject(fileName, DirectoryObject.objectType.File));
        }

        private void CloudSuppr(object sender, RoutedEventArgs e)
        {

        }

        private void directoryItem_clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                var elem = item.Content as DirectoryObject;
                if (elem.type == DirectoryObject.objectType.File)
                    currentCloud.DownloadFile(elem.name);
                else
                {
                    if (elem.type == DirectoryObject.objectType.Folder)
                        currentCloud.GoToFolder(elem.name);
                    else if (elem.type == DirectoryObject.objectType.ReactiveName && elem.name == "..")
                        currentCloud.GoBackToParent();
                    else
                        return ;
                    DirectoryRefreshFromCloud(currentCloud);
                }
            }
        }

        private void cloudItem_clicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                currentCloud = item.Content as IStorage;
                DirectoryRefreshFromCloud(currentCloud);
            }
        }

        public class DirectoryObject
        {
            public enum objectType { Folder = 0, File, ReactiveName };
            public DirectoryObject(string n, objectType t)
            {
                type = t;
                name = n;
                var icones = new List<string>
                {
                    "Resources/folder.png",
                    "Resources/file.png",
                    "Resources/sys.png"
                };
                
                icone = icones[(int)type];
            }

            public string name { get; set; }
            public objectType type { get; set; }
            public string size { get; set; }
            public string lastModifDate { get; set; }
            public string icone { get; set; }
        }
    }

}
