﻿using System;
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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MyCloud
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    
    public partial class ConnectWindow : Window
    {
        private MainWindow parent;
        public ConnectWindow(MainWindow p)
        {
            InitializeComponent();
            parent = p;
        }

        private void CloudLogin(object sender, RoutedEventArgs e)
        {
            IStorage cloud;
            if (cloudType.SelectedIndex == 0)
                cloud = new Mycloud.GoogleDrive();
            else
                cloud = new Mycloud.Dropbox();
            if (cloud != null)
            {
                parent.cloudList.Add(cloud);
                parent.currentCloud = cloud;
                //cloud.UpdateFileAndFolderList();
                parent.DirectoryRefreshFromCloud(cloud);
            }
            this.Close();
        }
    }
}
