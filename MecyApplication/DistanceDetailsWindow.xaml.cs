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

namespace MecyApplication
{
    /// <summary>
    /// Interaktionslogik für DistanceDetailsWindow.xaml
    /// </summary>
    public partial class DistanceDetailsWindow : Window
    {
        public double FromLon { get; set; }
        public double FromLat { get; set; }
        public double ToLon { get; set; }
        public double ToLat { get; set; }
        public double Distance { get; set; }

        public DistanceDetailsWindow(double fromLon, double fromLat, double toLon, double toLat, double distance)
        {
            InitializeComponent();

            FromLon = Math.Round(fromLon, 6);
            FromLat = Math.Round(fromLat, 6);
            ToLon = Math.Round(fromLat, 6);
            ToLat = Math.Round(fromLat, 6);
            Distance = Math.Round(distance, 2);

            gridDetails.DataContext = this;
        }
    }
}
