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
using System.Windows.Shapes;

namespace MyCloud
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    
    public partial class ConnectWindow : Window
    {
        private List<IStorage> cloudlist;
        public ConnectWindow(List<IStorage> _list)
        {
            InitializeComponent();
            cloudlist = _list;
        }

        private void CloudLogin(object sender, RoutedEventArgs e)
        {
            IStorage cloud;
            login.Text = cloudType.SelectedIndex.ToString();
            if (cloudType.SelectedIndex == 0)
                cloud = new Mycloud.GoogleDrive(password.Password, login.Text);
            else
                cloud = new Mycloud.Dropbox(password.Password, login.Text);
            cloudlist.Add(cloud);
        }
    }
}
