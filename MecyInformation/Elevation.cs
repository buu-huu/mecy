using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyInformation
{
    public class Elevation
    {
        private string _radarSite;
        private List<double> _elevations;

        public string RadarSite { get => _radarSite; set => _radarSite = value; }
        public List<double> Elevations { get => _elevations; set => _elevations = value; }

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
    }
}
