using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MecyInformation
{
    class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<OpenDataElement> _openDataElements;
        OpenDataElement _selectedElement;

        List<Mesocyclone> _mesocyclones;
        Mesocyclone _selectedMesocyclone;
        
        bool _isDownloading = false;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public MainViewModel()
        {
            ParseData();
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
            DownloadWindow downloadWindow = new DownloadWindow();

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _ = Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    downloadWindow.Show();
                });
                _isDownloading = true;
                OpenDataDownloader.DownloadAllData();
                _isDownloading = false;
            }).ContinueWith(task =>
            {
                ParseData();
                downloadWindow.Close();
            }, scheduler);
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
    }
}
