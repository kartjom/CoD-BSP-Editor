using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CoD_BSP_Editor
{
    /// <summary>
    /// Logika interakcji dla klasy AboutInfo.xaml
    /// </summary>
    public partial class AboutInfo : Window
    {
        public AboutInfo(string title, string author, string github)
        {
            InitializeComponent();

            this.AppTitle.Text = title;
            this.AppAuthor.Text = $"Made by {author}";

            this.Hyperlink.Inlines.Add(github);
            this.Hyperlink.NavigateUri = new Uri(github);
        }

        private void GitHubNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
        }
    }
}
