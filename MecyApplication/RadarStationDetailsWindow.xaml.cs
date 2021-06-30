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
    /// Interaktionslogik für RadarStationDetailsWindow.xaml
    /// </summary>
    public partial class RadarStationDetailsWindow : Window
    {
        public RadarStation Station { get; set; }
        public RadarStationDetailsWindow(RadarStation station)
        {
            InitializeComponent();
            Station = station;
            gridDetails.DataContext = this;

            this.Title = "Details | " + Station.Name;
        }
    }
}
