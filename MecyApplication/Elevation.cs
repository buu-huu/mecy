using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    /// <summary>
    /// Elevation class for mesocyclone
    /// </summary>
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

        /// <summary>
        /// Parses a string of elevations to a list of double values.
        /// </summary>
        /// <param name="elevations">String of elevations</param>
        /// <returns>List of elevations</returns>
        public static List<double> ParseElevationsFromString(string elevations)
        {
            List<double> parsedElevations = new List<double>();
            List<string> listElevations = elevations.Split(',').ToList(); // We need the elevations commaseparated

            try
            {
                foreach (string item in listElevations)
                {
                    parsedElevations.Add(Convert.ToDouble(item, CultureInfo.InvariantCulture));
                }
            }
            catch
            {
                // TODO: Logging
                return new List<double>();
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
