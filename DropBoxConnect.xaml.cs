using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyCloud
{

    public partial class DropBoxConnect : Window
    {
        private string RedirectUrl;

        private string oauth2State;

        public DropBoxConnect(DropboxRestAPI.Options argv, string authorizeUrl)
        {
            InitializeComponent();
            RedirectUrl = argv.RedirectUri;
            this.Browser.Navigate(authorizeUrl);
        }

        public string authorizeCodeUrl { get; private set; }

        public bool Result { get; private set; }

        private void BrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (!e.Uri.ToString().StartsWith(RedirectUrl, StringComparison.OrdinalIgnoreCase))
                return;
            try
            {
                this.authorizeCodeUrl = e.Uri.ToString().Substring(RedirectUrl.Length + 6);
                this.Result = true;
                this.Hide();
            }
            catch (ArgumentException){}
            finally
            {
                e.Cancel = true;
                this.Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
