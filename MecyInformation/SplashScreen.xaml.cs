using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                OpenDataDownloader.DownloadAllData();
            }).ContinueWith(task =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }, scheduler);

            //OpenDataDownloader.DownloadAllData();

            
        }
    }
}
