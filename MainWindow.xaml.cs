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


using System.IO;
using System.Collections.ObjectModel;


namespace MyCloud
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            directoryExplorer.Items.Add(
                new DirectoryObject()
                {
                    name = "dossier",
                    isDirectory = true,
                    Items = new ObservableCollection<DirectoryObject>()
                    {
                        new DirectoryObject() {name = "fichier1", isDirectory = false},
                        new DirectoryObject() {name = "porn", isDirectory = false}
                    }
                }
            );
        }
    }
    public class DirectoryObject
    {
        public DirectoryObject()
        {
            this.Items = new ObservableCollection<DirectoryObject>();
            if (this.isDirectory)
            {
                this.icone = "Resources/folder.png";
            }
            else {
                this.icone = "Resources/folder.png";
            }
        }

        public bool isDirectory { get; set; }
        public string name { get; set; }
        private string icone { get; set; }

        public ObservableCollection<DirectoryObject> Items { get; set; }
    }

}
