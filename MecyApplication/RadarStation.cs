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
        private double _latitude;
        private double _longitude;

        public string Name { get => _name; set => _name = value; }
        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }
        public double Longitude { get => _longitude; set => _longitude = value; }

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

                switch (radarCurr.Name)
                {
                    case "ASB":
                        radarCurr.Latitude = 53.564011;
                        radarCurr.Longitude = 6.748292;
                        break;
                    case "BOO":
                        radarCurr.Latitude = 54.004381;
                        radarCurr.Longitude = 10.046899;
                        break;
                    case "DRS":
                        radarCurr.Latitude = 51.124639;
                        radarCurr.Longitude = 13.768639;
                        break;
                    case "EIS":
                        radarCurr.Latitude = 49.540667;
                        radarCurr.Longitude = 12.402788;
                        break;
                    case "ESS":
                        radarCurr.Latitude = 51.405649;
                        radarCurr.Longitude = 6.967111;
                        break;
                    case "FBG":
                        radarCurr.Latitude = 47.873611;
                        radarCurr.Longitude = 8.003611;
                        break;
                    case "FLD":
                        radarCurr.Latitude = 51.311197;
                        radarCurr.Longitude = 8.801998;
                        break;
                    case "HNR":
                        radarCurr.Latitude = 52.460083;
                        radarCurr.Longitude = 9.694533;
                        break;
                    case "ISN":
                        radarCurr.Latitude = 48.174705;
                        radarCurr.Longitude = 12.101779;
                        break;
                    case "MEM":
                        radarCurr.Latitude = 48.042145;
                        radarCurr.Longitude = 10.219222;
                        break;
                    case "NEU":
                        radarCurr.Latitude = 50.500114;
                        radarCurr.Longitude = 11.135034;
                        break;
                    case "NHB":
                        radarCurr.Latitude = 50.109656;
                        radarCurr.Longitude = 6.548328;
                        break;
                    case "OFT":
                        radarCurr.Latitude = 49.984745;
                        radarCurr.Longitude = 8.712933;
                        break;
                    case "PRO":
                        radarCurr.Latitude = 52.648667;
                        radarCurr.Longitude = 13.858212;
                        break;
                    case "ROS":
                        radarCurr.Latitude = 54.175660;
                        radarCurr.Longitude = 12.058076;
                        break;
                    case "TUR":
                        radarCurr.Latitude = 48.585379;
                        radarCurr.Longitude = 9.782675;
                        break;
                    case "UMD":
                        radarCurr.Latitude = 52.160096;
                        radarCurr.Longitude = 11.176091;
                        break;
                }

                parsedStations.Add(radarCurr);
            }
            return parsedStations;
        }
    }
}
