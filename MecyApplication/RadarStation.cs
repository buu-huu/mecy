using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    /// <summary>
    /// Radar station class.
    /// </summary>
    public class RadarStation
    {
        public static List<string> ALL_STATIONS = new List<string>()
        {
            "ASB",
            "BOO",
            "ROS",
            "HNR",
            "UMD",
            "PRO",
            "ESS",
            "FLD",
            "DRS",
            "NEU",
            "NHB",
            "OFT",
            "EIS",
            "TUR",
            "ISN",
            "FBG",
            "MEM"
        };
        private string _name;
        private bool _isAvailable;

        public string Name { get => _name; set => _name = value; }
        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }

        /// <summary>
        /// Parses a string of radar stations to a list of radar stations.
        /// </summary>
        /// <param name="stations">Radar stations</param>
        /// <returns>List of radar stations</returns>
        public static List<RadarStation> ParseStationsFromString(string stations)
        {
            List<RadarStation> parsedStations = new List<RadarStation>();
            List<string> stationsInString = stations.Split(',').ToList<string>(); // We need a string with commaseparated stations

            foreach (string item in stationsInString)
            {
                RadarStation radarCurr = new RadarStation();
                radarCurr.Name = item.ToUpper();
                radarCurr.IsAvailable = item.Any(char.IsUpper);
                parsedStations.Add(radarCurr);
            }
            return parsedStations;
        }
    }
}
