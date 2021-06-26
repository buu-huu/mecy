using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    public sealed class MapConfiguration
    {
        public enum TileSource
        {
            OpenStreetMap,
            GoogleMaps
        }

        private static MapConfiguration instance = null;
        private static readonly object padlock = new object();

        private static TileSource _activeTileSource;
        private bool _showHistoricMesocyclones;
        private bool _showMesocycloneDiameter;

        public TileSource ActiveTileSource
        {
            get
            {
                return _activeTileSource;
            }
            set
            {
                _activeTileSource = value;
            }
        }

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

        public bool ShowMesocycloneDiameter
        {
            get
            {
                return _showMesocycloneDiameter;
            }
            set
            {
                _showMesocycloneDiameter = value;
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
            mapConfig.ActiveTileSource = TileSource.OpenStreetMap;

            return mapConfig;
        }
    }
}
