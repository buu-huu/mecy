using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    /// <summary>
    /// Singleton that holds all the configuration settings for the map.
    /// </summary>
    public sealed class MapConfiguration
    {
        /// <summary>
        /// Available tile sources
        /// </summary>
        public enum TileSource
        {
            OpenStreetMap,
            GoogleMaps
        }

        private static MapConfiguration instance = null;
        private static readonly object padlock = new object();

        private TileSource _activeTileSource;
        private bool _showHistoricMesocyclones;
        private bool _showMesocycloneDiameter;
        private bool _showScaleBar;
        private bool _showZoomWidget;
        private bool _showMesocycloneIdLabel;
        private bool _historicMesocyclonesTransparent;
        private bool _autoMoveActiveMeso;
        private bool _showRadars;

        private float _historicMesocyclonesOpacity;

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

        public bool ShowScaleBar
        {
            get
            {
                return _showScaleBar;
            }
            set
            {
                _showScaleBar = value;
            }
        }

        public bool ShowZoomWidget
        {
            get
            {
                return _showZoomWidget;
            }
            set
            {
                _showZoomWidget = value;
            }
        }

        public bool ShowMesocycloneIdLabel
        {
            get
            {
                return _showMesocycloneIdLabel;
            }
            set
            {
                _showMesocycloneIdLabel = value;
            }
        }

        public bool HistoricMesocyclonesTransparent
        {
            get
            {
                return _historicMesocyclonesTransparent;
            }
            set
            {
                _historicMesocyclonesTransparent = value;
            }
        }

        public bool AutoMoveActiveMeso
        {
            get
            {
                return _autoMoveActiveMeso;
            }
            set
            {
                _autoMoveActiveMeso = value;
            }
        }

        public bool ShowRadars
        {
            get
            {
                return _showRadars;
            }
            set
            {
                _showRadars = value;
            }
        }

        public float HistoricMesocyclonesOpacity
        {
            get
            {
                return _historicMesocyclonesOpacity;
            }
            set
            {
                _historicMesocyclonesOpacity = value;
            }
        }


        private MapConfiguration() { }

        /// <summary>
        /// Returns singleton instance of the class.
        /// </summary>
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

        /// <summary>
        /// Creates the default map configuration.
        /// </summary>
        /// <returns>Map configuration</returns>
        public static MapConfiguration CreateDefaultMapConfiguration()
        {
            var mapConfig = Instance;

            mapConfig.ActiveTileSource = TileSource.OpenStreetMap;
            mapConfig.ShowHistoricMesocyclones = true;
            mapConfig.ShowMesocycloneDiameter = true;
            mapConfig.ShowScaleBar = true;
            mapConfig.ShowZoomWidget = true;
            mapConfig.ShowMesocycloneIdLabel = true;
            mapConfig.HistoricMesocyclonesTransparent = true;
            mapConfig.HistoricMesocyclonesOpacity = 0.55f;
            mapConfig.AutoMoveActiveMeso = false;
            mapConfig.ShowRadars = false;

            return mapConfig;
        }
    }
}
