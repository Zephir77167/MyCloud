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
        public ObservableCollection<IStorage> cloudList { get; set; }
        public ObservableCollection<DirectoryObject> directoryList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            cloudList = new ObservableCollection<IStorage>();
            directoryList = new ObservableCollection<DirectoryObject>();
            directoryList.Add(new DirectoryObject("..", DirectoryObject.objectType.ReactiveName));
            directoryList.Add(new DirectoryObject("TestDir", DirectoryObject.objectType.Folder));
            directoryList.Add(new DirectoryObject("TestFichier",DirectoryObject.objectType.File));
            directoryList.Add(new DirectoryObject("porn", DirectoryObject.objectType.File));
            this.DataContext = this;
        }
        private void CloudConnect(object sender, RoutedEventArgs e)
        {
            ConnectWindow loginBox = new ConnectWindow(cloudList);
            loginBox.Show();
            RaisePropertyChanged("cloudList");
        }
        private void CloudDisconnect(object sender, RoutedEventArgs e)
        {

        }
        private void CloudRefresh(object sender, RoutedEventArgs e)
        {
            if (cloudItemList.SelectedItem != null)
                DirectoryRefreshFromCloud(cloudList[cloudItemList.SelectedIndex]);
        }

        public void DirectoryRefreshFromCloud(IStorage cloud)
        {
            directoryList.Clear();
            List<string> folders = cloud.GetFolderList();
            foreach (string folderName in folders)
                directoryList.Add(new DirectoryObject(folderName, DirectoryObject.objectType.Folder));
            List<string> files = cloud.GetFileList();
            foreach (string fileName in files)
                directoryList.Add(new DirectoryObject(fileName, DirectoryObject.objectType.File));
            RaisePropertyChanged("directoryList");
        }

        private void CloudSuppr(object sender, RoutedEventArgs e)
        {

        }

        public class DirectoryObject : INotifyPropertyChanged
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

            public event PropertyChangedEventHandler PropertyChanged;
            private void RaisePropertyChanged(string propName)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

}
