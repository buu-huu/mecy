using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using WinSCP;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mesocyclone activeMeso;
        List<OpenDataElement> openDataElements;

        OpenDataElement selectedElement;
        bool isDownloading = false;

        public MainWindow()
        {
            InitializeComponent();

            RefreshView();
            SetUpClocks();
        }

        private void RefreshView()
        {
            openDataElements = XMLParser.ParseAllMesos(OpenDataDownloader.LOCAL_DOWNLOAD_PATH);
            lvTimes.ItemsSource = openDataElements;
            gridDetails.DataContext = activeMeso;
        }

        private void ReDownloadData()
        {
            isDownloading = true;
            OpenDataDownloader.DownloadAllData();
            isDownloading = false;
        }

        private void SetUpClocks()
        {
            DispatcherTimer clockUtc = new DispatcherTimer();
            clockUtc.Interval = TimeSpan.FromSeconds(1);
            clockUtc.Tick += clockUtcTick;
            clockUtc.Start();

            DispatcherTimer clockLocal = new DispatcherTimer();
            clockLocal.Interval = TimeSpan.FromSeconds(1);
            clockLocal.Tick += clockLocalTick;
            clockLocal.Start();
        }

        private void clockUtcTick(object sender, EventArgs e)
        {
            lblClockUtc.Content = "UTC: " + DateTime.UtcNow.ToLongTimeString();
        }

        private void clockLocalTick(object sender, EventArgs e)
        {
            lblClockLocal.Content = "LOC: " + DateTime.Now.ToLongTimeString();
        }

        private void UpdateDetailsPanel()
        {
            activeMeso = (Mesocyclone)lvMesos.SelectedItem;
            gridDetails.DataContext = activeMeso;
        }

        private void lvMesos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDetailsPanel();
        }

        private void lvMesos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvMesos.SelectedItem != null)
            {
                new MapWindow(activeMeso).Show();
            }
        }

        private void lvMesos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult res = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (res.VisualHit.GetType() != typeof(ListBoxItem))
            {
                lvMesos.UnselectAll();
            }
        }

        private void lvTimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTimes.SelectedItem != null)
            {
                selectedElement = (OpenDataElement)lvTimes.SelectedItem;
                lvMesos.ItemsSource = selectedElement.Mesocyclones;
            }
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void tlbButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!isDownloading)
            {
                DownloadWindow downloadWindow = new DownloadWindow();

                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _ = Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate {
                        downloadWindow.Show();
                    });
                    ReDownloadData();
                }).ContinueWith(task =>
                {
                    RefreshView();
                    downloadWindow.Close();
                }, scheduler);
            }
        }
    }
}
