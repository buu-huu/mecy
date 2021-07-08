using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    /// <summary>
    /// Singleton that holds all the configuration settings for the map.
    /// </summary>
    public sealed class MapConfiguration : INotifyPropertyChanged
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
        private bool _showRadarLabels;
        private bool _showRadarDiameters;
        private bool _currentlyMeasuringDistance;

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
                OnPropertyChanged("ActiveTileSource");
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
                OnPropertyChanged("ShowHistoricMesocyclones");
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
                OnPropertyChanged("ShowMesocycloneDiameter");
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
                OnPropertyChanged("ShowScaleBar");
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
                OnPropertyChanged("ShowZoomWidget");
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
                OnPropertyChanged("ShowMesocycloneIdLabel");
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
                OnPropertyChanged("HistoricMesocyclonesTransparent");
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
                OnPropertyChanged("AutoMoveActiveMeso");
            }
        }

        public bool ShowRadarLabels
        {
            get
            {
                return _showRadarLabels;
            }
            set
            {
                _showRadarLabels = value;
                OnPropertyChanged("ShowRadarLabels");
            }
        }

        public bool ShowRadarDiameters
        {
            get
            {
                return _showRadarDiameters;
            }
            set
            {
                _showRadarDiameters = value;
                OnPropertyChanged("ShowRadarDiameters");
            }
        }

        public bool CurrentlyMeasuringDistance
        {
            get
            {
                return _currentlyMeasuringDistance;
            }
            set
            {
                _currentlyMeasuringDistance = value;
                OnPropertyChanged("CurrentlyMeasuringDistance");
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
                OnPropertyChanged("HistoricMesocyclonesOpacity");
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
            mapConfig.HistoricMesocyclonesOpacity = 0.7f;
            mapConfig.AutoMoveActiveMeso = false;
            mapConfig.ShowRadarLabels = true;
            mapConfig.ShowRadarDiameters = false;
            mapConfig.CurrentlyMeasuringDistance = false;

            return mapConfig;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
