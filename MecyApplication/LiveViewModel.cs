﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MecyApplication
{
    public class LiveViewModel : INotifyPropertyChanged
    {
        OpenDataElement _selectedElement;
        Mesocyclone _selectedMesocyclone;

        MapConfiguration _currentMapConfiguration = MapConfiguration.CreateDefaultMapConfiguration();

        bool _openDataServerReachable = false;

        DateTime _timeUtc;
        DateTime _timeLoc;

        DateTime _lastDownloadTime;

        #region properties
        public OpenDataElement SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
        }
        public Mesocyclone SelectedMesocyclone
        {
            get
            {
                return _selectedMesocyclone;
            }
            set
            {
                _selectedMesocyclone = value;
                OnPropertyChanged("SelectedMesocyclone");
            }
        }

        public MapConfiguration CurrentMapConfiguration
        {
            get
            {
                return _currentMapConfiguration;
            }
            set
            {
                _currentMapConfiguration = value;
                OnPropertyChanged("CurrentMapConfiguration");
            }
        }

        public bool OpenDataServerReachable
        {
            get
            {
                return _openDataServerReachable;
            }
            set
            {
                _openDataServerReachable = value;
                OnPropertyChanged("OpenDataServerReachable");
            }
        }
        public DateTime TimeUtc
        {
            get
            {
                return _timeUtc;
            }
            set
            {
                _timeUtc = value;
                OnPropertyChanged("TimeUtc");
            }
        }
        public DateTime TimeLoc
        {
            get
            {
                return _timeLoc;
            }
            set
            {
                _timeLoc = value;
                OnPropertyChanged("TimeLoc");
            }
        }
        public DateTime LastDownloadTime
        {
            get
            {
                return _lastDownloadTime;
            }
            set
            {
                _lastDownloadTime = value;
                OnPropertyChanged("LastDownloadTime");
            }
        }
        #endregion properties

        /// <summary>
        /// Constructor that prepares everything to work.
        /// </summary>
        public LiveViewModel()
        {
            SetupClocks();
            SetupConnectionWatcher();
            SetupAutoDownloader();

            DownloadData(this);
        }

        /// <summary>
        /// Sets up clocks with timers.
        /// </summary>
        private void SetupClocks()
        {
            DispatcherTimer clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClocksTick;
            clockTimer.Start();
        }

        /// <summary>
        /// Sets up the watcher, that checks for an existing connection to the opendata server.
        /// </summary>
        private void SetupConnectionWatcher()
        {
            DispatcherTimer connectionTimer = new DispatcherTimer();
            connectionTimer.Interval = TimeSpan.FromSeconds(5);
            connectionTimer.Tick += ConnectionWatcherTick;
            connectionTimer.Start();
        }

        /// <summary>
        /// Sets up timer for automatic downloading of latest mesocyclone.
        /// </summary>
        private void SetupAutoDownloader()
        {
            DispatcherTimer downloadTimer = new DispatcherTimer();
            downloadTimer.Interval = TimeSpan.FromSeconds(30);
            downloadTimer.Tick += AutoDownloaderTick;
            downloadTimer.Start();
        }

        /// <summary>
        /// Tick for the connection watcher.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void ConnectionWatcherTick(object sender, EventArgs e)
        {
            if (OpenDataDownloader.CheckServerConnection())
            {
                OpenDataServerReachable = true;
            }
            else
            {
                OpenDataServerReachable = false;
            }
        }

        /// <summary>
        /// Tick for auto downloader.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void AutoDownloaderTick(object sender, EventArgs e)
        {
            DownloadData(this);
        }

        /// <summary>
        /// Ticks for the clocks.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void ClocksTick(object sender, EventArgs e)
        {
            TimeUtc = DateTime.UtcNow;
            TimeLoc = DateTime.Now;
        }

        /// <summary>
        /// Parses all opendata elements.
        /// </summary>
        private void ParseData()
        {
            SelectedElement = XMLParser.ParseMesoFileLatest(
                OpenDataDownloader.LOCAL_DOWNLOAD_PATH_LATEST_MESO + OpenDataDownloader.LATEST_MESO_NAME);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // -------------------- COMMANDS --------------------
        public ICommand DownloadDataCommand
        {
            get
            {
                return new RelayCommand(e => true, this.DownloadData);
            }
        }

        /// <summary>
        /// Downloads current data from opendata server.
        /// </summary>
        /// <param name="obj">Object</param>
        private void DownloadData(object obj)
        {
            if (OpenDataDownloader.CheckServerConnection())
            {
                OpenDataDownloader.DownloadLatestMeso();
                ParseData();
                RefreshMapAndMapConfiguration(this);
                LastDownloadTime = DateTime.UtcNow;
            }
            else MessageBox.Show("OpenData server not reachable!");
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(e => true, this.ExitApplication);
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="obj">Object</param>
        private void ExitApplication(object obj)
        {
            Environment.Exit(0);
        }

        public ICommand ShowAboutWindowCommand
        {
            get
            {
                return new RelayCommand(e => true, this.ShowAboutWindow);
            }
        }

        /// <summary>
        /// Shows the about window.
        /// </summary>
        /// <param name="obj">Object</param>
        private void ShowAboutWindow(object obj)
        {
            new AboutWindow().Show();
        }

        public ICommand SelectOpenStreetMapStyleCommand
        {
            get
            {
                return new RelayCommand(e => true, this.SelectOpenStreetMapStyle);
            }
        }

        // -------------------- DELEGATES --------------------
        public delegate void RefreshMapEventAction();
        public event RefreshMapEventAction RefreshMapEvent;

        public delegate void RefreshMapWidgetsEventAction();
        public event RefreshMapWidgetsEventAction RefreshMapWidgetsEvent;

        public delegate void CenterMapEventAction();
        public event CenterMapEventAction CenterMapEvent;

        public delegate void CenterMapToMesoEventAction();
        public event CenterMapToMesoEventAction CenterMapToMesoEvent;

        /// <summary>
        /// Selects the Open Street Map style
        /// </summary>
        /// <param name="obj">Object</param>
        private void SelectOpenStreetMapStyle(object obj)
        {
            CurrentMapConfiguration.ActiveTileSource = MapConfiguration.TileSource.OpenStreetMap;
            RefreshMapWidgetsEvent?.Invoke();
        }

        public ICommand SelectGoogleMapsStyleCommand
        {
            get
            {
                return new RelayCommand(e => true, this.SelectGoogleMapsStyle);
            }
        }

        /// <summary>
        /// Select the Google Maps style
        /// </summary>
        /// <param name="obj">Object</param>
        private void SelectGoogleMapsStyle(object obj)
        {
            CurrentMapConfiguration.ActiveTileSource = MapConfiguration.TileSource.GoogleMaps;
            RefreshMapWidgetsEvent?.Invoke();
        }

        public ICommand RefreshMapAndMapConfigurationCommand
        {
            get
            {
                return new RelayCommand(e => true, this.RefreshMapAndMapConfiguration);
            }
        }

        /// <summary>
        /// Fires the event to refresh the map and the current map configuration.
        /// </summary>
        /// <param name="obj">Object</param>
        private void RefreshMapAndMapConfiguration(object obj)
        {
            RefreshMapEvent?.Invoke();
        }

        public ICommand RefreshMapWidgetsCommand
        {
            get
            {
                return new RelayCommand(e => true, this.RefreshMapWidgets);
            }
        }

        /// <summary>
        /// Fires the event to refresh the map and its widgets.
        /// </summary>
        /// <param name="obj">Object</param>
        private void RefreshMapWidgets(object obj)
        {
            RefreshMapWidgetsEvent?.Invoke();
        }

        public ICommand CenterMapCommand
        {
            get
            {
                return new RelayCommand(e => true, this.CenterMap);
            }
        }

        /// <summary>
        /// Fires the event to center the map.
        /// </summary>
        /// <param name="obj">Object</param>
        public void CenterMap(object obj)
        {
            CenterMapEvent?.Invoke();
        }

        public ICommand CenterMapToMesoCommand
        {
            get
            {
                return new RelayCommand(e => true, this.CenterMapToMeso);
            }
        }

        /// <summary>
        /// Fires the event to center the map to a mesocyclone.
        /// </summary>
        /// <param name="obj">Object</param>
        public void CenterMapToMeso(object obj)
        {
            CenterMapToMesoEvent?.Invoke();
        }
    }
}
