using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    public sealed class MapConfiguration
    {
        private static MapConfiguration instance = null;
        private static readonly object padlock = new object();

        private bool _showHistoricMesocyclones = false;

        public bool ShowHistoricMesocyclones
        {
            get
            {
                return _showHistoricMesocyclones;
            }
            set
            {
                _showHistoricMesocyclones = value;
            }
        }

        MapConfiguration() { }

        public static MapConfiguration Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new MapConfiguration();
                    }
                    return instance;
                }
            }
        }

        public static MapConfiguration CreateDefaultMapConfiguration()
        {
            var mapConfig = new MapConfiguration();
            mapConfig.ShowHistoricMesocyclones = false;
            return mapConfig;
        }
    }
}
