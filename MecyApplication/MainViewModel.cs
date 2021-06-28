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
    public class MainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<OpenDataElement> _openDataElements;
        OpenDataElement _selectedElement;

        Mesocyclone _selectedMesocyclone;

        MapConfiguration _currentMapConfiguration = MapConfiguration.CreateDefaultMapConfiguration();

        bool _openDataServerReachable = false;
        bool _isDownloading = false;

        DateTime _timeUtc;
        DateTime _timeLoc;

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
        public ObservableCollection<OpenDataElement> OpenDataElements
        {
            get
            {
                if (_openDataElements == null)
                {
                    _openDataElements = new ObservableCollection<OpenDataElement>();
                }
                return _openDataElements;
            }
            set
            {
                _openDataElements = value;
                OnPropertyChanged("OpenDataElements");
            }
        }
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

        public bool IsDownloading
        {
            get
            {
                return _isDownloading;
            }
            set
            {
                _isDownloading = value;
                OnPropertyChanged("IsDownloading");
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
        #endregion properties

        public MainViewModel()
        {
            ParseData();
            SetupClocks();
            SetupConnectionWatcher();
        }

        private void SetupClocks()
        {
            DispatcherTimer clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClocksTick;
            clockTimer.Start();
        }

        private void SetupConnectionWatcher()
        {
            DispatcherTimer connectionTimer = new DispatcherTimer();
            connectionTimer.Interval = TimeSpan.FromSeconds(5);
            connectionTimer.Tick += ConnectionWatcherTick;
            connectionTimer.Start();
        }

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

        private void ClocksTick(object sender, EventArgs e)
        {
            TimeUtc = DateTime.UtcNow;
            TimeLoc = DateTime.Now;
        }

        private void ParseData()
        {
            var parsedElements = XMLParser.ParseAllMesos(OpenDataDownloader.LOCAL_DOWNLOAD_PATH);
            if (parsedElements.Count > 0)
            {
                OpenDataElements = new ObservableCollection<OpenDataElement>(parsedElements);
            }
            else
            {
                MessageBox.Show(
                        "Can't find data. Try to download data again.",
                        "Mecy",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand DownloadDataCommand
        {
            get
            {
                return new RelayCommand(e => true, this.DownloadData);
            }
        }

        private void DownloadData(object obj)
        {
            if (!IsDownloading)
            {
                DownloadWindow downloadWindow = new DownloadWindow();
                IsDownloading = true;

                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _ = Task.Factory.StartNew(() =>
                {
                    if (OpenDataDownloader.CheckServerConnection())
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            downloadWindow.Show();
                        });
                        OpenDataDownloader.DownloadAllData();
                    }
                    else
                    {
                        MessageBox.Show(
                            "OpenData server not reachable.",
                            "Mecy",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }

                }).ContinueWith(task =>
                {
                    ParseData();
                    IsDownloading = false;
                    downloadWindow.Close();
                }, scheduler);
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(e => true, this.ExitApplication);
            }
        }

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

        public delegate void RefreshMapEventAction();
        public event RefreshMapEventAction RefreshMapEvent;

        public delegate void RefreshMapWidgetsEventAction();
        public event RefreshMapWidgetsEventAction RefreshMapWidgetsEvent;

        public delegate void CenterMapEventAction();
        public event CenterMapEventAction CenterMapEvent;

        public delegate void CenterMapToMesoEventAction();
        public event CenterMapToMesoEventAction CenterMapToMesoEvent;

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

        public void CenterMapToMeso(object obj)
        {
            CenterMapToMesoEvent?.Invoke();
        }
    }
}
