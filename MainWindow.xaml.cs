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
        private List<IStorage> cloudList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            cloudList = new List<IStorage>();
            cloudItemList.ItemsSource = cloudList;
        }
        private void CloudConnect(object sender, RoutedEventArgs e)
        {
            ConnectWindow loginBox = new ConnectWindow(cloudList);
            loginBox.Show();
        }

        private void CloudDisconnect(object sender, RoutedEventArgs e)
        {

        }
        private void CloudRefresh(object sender, RoutedEventArgs e)
        {

        }
        private void CloudSuppr(object sender, RoutedEventArgs e)
        {

        }

    }

}
