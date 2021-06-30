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
        private string _fullName;
        private string _name;
        private bool _isAvailable;
        private double _latitude;
        private double _longitude;
        private int _antennaHeight;

        public string FullName { get => _fullName; set => _fullName = value; }
        public string Name { get => _name; set => _name = value; }
        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }
        public double Longitude { get => _longitude; set => _longitude = value; }
        public int AntennaHeight { get => _antennaHeight; set => _antennaHeight = value; }

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
                        radarCurr.FullName = "ASR Borkum";
                        radarCurr.Latitude = 53.564011;
                        radarCurr.Longitude = 6.748292;
                        radarCurr.AntennaHeight = 36;
                        break;
                    case "BOO":
                        radarCurr.FullName = "Boostedt";
                        radarCurr.Latitude = 54.004381;
                        radarCurr.Longitude = 10.046899;
                        radarCurr.AntennaHeight = 125;
                        break;
                    case "DRS":
                        radarCurr.FullName = "Dresden";
                        radarCurr.Latitude = 51.124639;
                        radarCurr.Longitude = 13.768639;
                        radarCurr.AntennaHeight = 263;
                        break;
                    case "EIS":
                        radarCurr.FullName = "Eisberg";
                        radarCurr.Latitude = 49.540667;
                        radarCurr.Longitude = 12.402788;
                        radarCurr.AntennaHeight = 799;
                        break;
                    case "ESS":
                        radarCurr.FullName = "Essen";
                        radarCurr.Latitude = 51.405649;
                        radarCurr.Longitude = 6.967111;
                        radarCurr.AntennaHeight = 185;
                        break;
                    case "FBG":
                        radarCurr.FullName = "Feldberg";
                        radarCurr.Latitude = 47.873611;
                        radarCurr.Longitude = 8.003611;
                        radarCurr.AntennaHeight = 1516;
                        break;
                    case "FLD":
                        radarCurr.FullName = "Flechtdorf";
                        radarCurr.Latitude = 51.311197;
                        radarCurr.Longitude = 8.801998;
                        radarCurr.AntennaHeight = 628;
                        break;
                    case "HNR":
                        radarCurr.FullName = "Hannover";
                        radarCurr.Latitude = 52.460083;
                        radarCurr.Longitude = 9.694533;
                        radarCurr.AntennaHeight = 98;
                        break;
                    case "ISN":
                        radarCurr.FullName = "Isen";
                        radarCurr.Latitude = 48.174705;
                        radarCurr.Longitude = 12.101779;
                        radarCurr.AntennaHeight = 678;
                        break;
                    case "MEM":
                        radarCurr.FullName = "Memmingen";
                        radarCurr.Latitude = 48.042145;
                        radarCurr.Longitude = 10.219222;
                        radarCurr.AntennaHeight = 724;
                        break;
                    case "NEU":
                        radarCurr.FullName = "Neuhaus";
                        radarCurr.Latitude = 50.500114;
                        radarCurr.Longitude = 11.135034;
                        radarCurr.AntennaHeight = 880;
                        break;
                    case "NHB":
                        radarCurr.FullName = "Neuheilenbach";
                        radarCurr.Latitude = 50.109656;
                        radarCurr.Longitude = 6.548328;
                        radarCurr.AntennaHeight = 586;
                        break;
                    case "OFT":
                        radarCurr.FullName = "Offenthal";
                        radarCurr.Latitude = 49.984745;
                        radarCurr.Longitude = 8.712933;
                        radarCurr.AntennaHeight = 246;
                        break;
                    case "PRO":
                        radarCurr.FullName = "Prötzel";
                        radarCurr.Latitude = 52.648667;
                        radarCurr.Longitude = 13.858212;
                        radarCurr.AntennaHeight = 194;
                        break;
                    case "ROS":
                        radarCurr.FullName = "Rostock";
                        radarCurr.Latitude = 54.175660;
                        radarCurr.Longitude = 12.058076;
                        radarCurr.AntennaHeight = 37;
                        break;
                    case "TUR":
                        radarCurr.FullName = "Türkheim";
                        radarCurr.Latitude = 48.585379;
                        radarCurr.Longitude = 9.782675;
                        radarCurr.AntennaHeight = 768;
                        break;
                    case "UMD":
                        radarCurr.FullName = "Ummendorf";
                        radarCurr.Latitude = 52.160096;
                        radarCurr.Longitude = 11.176091;
                        radarCurr.AntennaHeight = 185;
                        break;
                }

                parsedStations.Add(radarCurr);
            }
            return parsedStations;
        }
    }
}
