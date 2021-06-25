using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MecyInformation
{
    class MainViewModel : INotifyPropertyChanged
    {
        ObservableCollection<OpenDataElement> _openDataElements;
        OpenDataElement _selectedElement;

        List<Mesocyclone> _mesocyclones;
        Mesocyclone _selectedMesocyclone;
        
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
        public List<Mesocyclone> Mesocyclones
        {
            get
            {
                return _mesocyclones;
            }
            set
            {
                _mesocyclones = value;
                OnPropertyChanged("Mesocyclones");
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
        }

        private void SetupClocks()
        {
            DispatcherTimer clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClocksTick;
            clockTimer.Start();
        }

        private void ClocksTick(object sender, EventArgs e)
        {
            TimeUtc = DateTime.UtcNow;
            TimeLoc = DateTime.Now;
        }

        private void ParseData()
        {
            OpenDataElements = new ObservableCollection<OpenDataElement>(XMLParser.ParseAllMesos(OpenDataDownloader.LOCAL_DOWNLOAD_PATH));
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

                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _ = Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        downloadWindow.Show();
                    });
                    IsDownloading = true;
                    OpenDataDownloader.DownloadAllData();
                }).ContinueWith(task =>
                {
                    ParseData();
                    IsDownloading = false;
                    downloadWindow.Close();
                }, scheduler);
            }
        }

        public ICommand OpenMesoMapWindowCommand
        {
            get
            {
                return new RelayCommand(e => true, this.OpenMesoMapWindow);
            }
        }

        private void OpenMesoMapWindow(object obj)
        {
            if (SelectedMesocyclone != null)
            {
                new MapWindow(SelectedMesocyclone).Show();
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
    }
}
