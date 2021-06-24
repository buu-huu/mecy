using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyInformation
{
    public class RadarStation
    {
        static List<string> ALL_STATIONS = new List<string>()
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

        public static List<RadarStation> ParseStationsFromString(string stations)
        {
            List<RadarStation> parsedStations = new List<RadarStation>();
            List<string> stationsInString = stations.Split(',').ToList<string>();

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
