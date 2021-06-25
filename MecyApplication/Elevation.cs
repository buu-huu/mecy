using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    public class Elevation : INotifyPropertyChanged
    {
        private string _radarSite;
        private List<double> _elevations;

        public string RadarSite
        {
            get
            {
                return _radarSite;
            }
            set
            {
                _radarSite = value;
                OnPropertyChanged("RadarSite");
            }
        }
        public List<double> Elevations
        {
            get
            {
                return _elevations;
            }
            set
            {
                _elevations = value;
                OnPropertyChanged("Elevations");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<double> ParseElevationsFromString(string elevations)
        {
            List<double> parsedElevations = new List<double>();
            List<string> listElevations = elevations.Split(',').ToList();

            foreach (string item in listElevations)
            {
                parsedElevations.Add(Convert.ToDouble(item, CultureInfo.InvariantCulture));
            }
            return parsedElevations;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
