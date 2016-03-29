using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ListViewItem = System.Windows.Controls.ListViewItem;


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
            currentCloud.UpdateFileAndFolderList();
            DirectoryRefreshFromCloud(currentCloud);
        }

        public void DirectoryRefreshFromCloud(IStorage cloud)
        {
            if (cloud == null)
                return ;
            
            directoryList.Clear();
            directoryList.Add(new DirectoryObject("..", DirectoryObject.objectType.ReactiveName));
            List<DirectoryObject> itemList = cloud.listDirectory();
            foreach (DirectoryObject item in itemList)
                directoryList.Add(new DirectoryObject(item.name, item.type) { size=item.size, lastModifDate=item.lastModifDate});
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
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    DialogResult result = fbd.ShowDialog();

                    if (currentCloud.DownloadFile(elem.name, fbd.SelectedPath))
                    {
                        System.Windows.Forms.MessageBox.Show(elem.name + " a été téléchargé dans " + fbd.SelectedPath, "Succès", MessageBoxButtons.OK);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(elem.name + "n'a pu être ouvert en raison de son type.", "Erreur", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    if (elem.type == DirectoryObject.objectType.Folder)
                        currentCloud.GoToFolder(elem.name);
                    else if (elem.type == DirectoryObject.objectType.ReactiveName && elem.name == "..")
                        currentCloud.GoBackToParent();
                    else
                        return;
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
