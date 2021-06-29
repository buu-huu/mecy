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

namespace MecyApplication
{
    /// <summary>
    /// Handles the splash screen.
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts here, when the content of the splash screen is rendered.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Thread.Sleep(1500);
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _ = Task.Factory.StartNew(() =>
            {
                if (OpenDataDownloader.CheckServerConnection())
                {
                    //OpenDataDownloader.DownloadAllData();
                }
                else
                {
                    MessageBox.Show(
                        "OpenData server not reachable. Using existing data.",
                        "Mecy",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }).ContinueWith(task =>
            {
                MainViewModel mainViewModel = new MainViewModel();
                MainWindow mainWindow = new MainWindow(mainViewModel);

                mainWindow.Show();
                this.Close();
            }, scheduler);            
        }
    }
}
