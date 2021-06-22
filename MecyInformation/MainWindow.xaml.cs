using System;
using System.Collections.Generic;
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
using WinSCP;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mesocyclone activeMeso = null;
        Dictionary<DateTime, List<Mesocyclone>> mesoDict;
        DateTime selectedTime;
        DateTime earliestTime;
        DateTime latestTime;

        public MainWindow()
        {
            InitializeComponent();

            OpenDataDownloader.DownloadAllData();

            mesoDict = XMLParser.ParseAllMesos(OpenDataDownloader.LOCAL_DOWNLOAD_PATH);
            earliestTime = Mesocyclone.GetEarliestDateTime(mesoDict);
            latestTime = Mesocyclone.GetLatestDateTime(mesoDict);
            selectedTime = latestTime;
            txtSelectedTime.Text = selectedTime.ToString();
            lvMesos.ItemsSource = mesoDict[selectedTime];

            gridDetails.DataContext = activeMeso;
            Console.WriteLine(Mesocyclone.GetLatestDateTime(mesoDict));
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

        private void btnTimeForward_Click(object sender, RoutedEventArgs e)
        {
            if (!(sldTime.Value == sldTime.Maximum))
            {
                sldTime.Value++;
                selectedTime = selectedTime.Add(new TimeSpan(0, 5, 0));
                txtSelectedTime.Text = selectedTime.ToString();
                lvMesos.ItemsSource = mesoDict[selectedTime];
            }
        }

        private void btnTimeBack_Click(object sender, RoutedEventArgs e)
        {
            if (!(sldTime.Value == 0))
            {
                sldTime.Value--;
                selectedTime = selectedTime.Subtract(new TimeSpan(0, 5, 0));
                txtSelectedTime.Text = selectedTime.ToString();
                lvMesos.ItemsSource = mesoDict[selectedTime];
            }
        }

        private void sldTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
