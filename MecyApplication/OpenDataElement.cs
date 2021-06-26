using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    public class OpenDataElement : INotifyPropertyChanged
    {
        private DateTime _time;
        private List<RadarStation> _radarStations;
        private List<Mesocyclone> _mesocyclones;

        private bool _stationAvailAsb;
        private bool _stationAvailBoo;
        private bool _stationAvailDrs;
        private bool _stationAvailEis;
        private bool _stationAvailEss;
        private bool _stationAvailFbg;
        private bool _stationAvailFld;
        private bool _stationAvailHnr;
        private bool _stationAvailIsn;
        private bool _stationAvailMem;
        private bool _stationAvailNeu;
        private bool _stationAvailNhb;
        private bool _stationAvailOft;
        private bool _stationAvailPro;
        private bool _stationAvailRos;
        private bool _stationAvailTur;
        private bool _stationAvailUmd;

        private int _mesoCountIndicator;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
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
        public List<RadarStation> RadarStations
        {
            get
            {
                return _radarStations;
            }
            set
            {
                _radarStations = value;
                OnPropertyChanged("RadarStations");
            }
        }
        public bool StationAvailAsb
        {
            get
            {
                return _stationAvailAsb;
            }
            set
            {
                _stationAvailAsb = value;
                OnPropertyChanged("StationAvailAsb");
            }
        }
        public bool StationAvailBoo
        {
            get
            {
                return _stationAvailBoo;
            }
            set
            {
                _stationAvailBoo = value;
                OnPropertyChanged("StationAvailBoo");
            }
        }
        public bool StationAvailDrs
        {
            get
            {
                return _stationAvailDrs;
            }
            set
            {
                _stationAvailDrs = value;
                OnPropertyChanged("StationAvailDrs");
            }
        }
        public bool StationAvailEis
        {
            get
            {
                return _stationAvailEis;
            }
            set
            {
                _stationAvailEis = value;
                OnPropertyChanged("StationAvailEis");
            }
        }
        public bool StationAvailEss
        {
            get
            {
                return _stationAvailEss;
            }
            set
            {
                _stationAvailEss = value;
                OnPropertyChanged("StationAvailEss");
            }
        }
        public bool StationAvailFbg
        {
            get
            {
                return _stationAvailFbg;
            }
            set
            {
                _stationAvailFbg = value;
                OnPropertyChanged("StationAvailFbg");
            }
        }
        public bool StationAvailFld
        {
            get
            {
                return _stationAvailFld;
            }
            set
            {
                _stationAvailFld = value;
                OnPropertyChanged("StationAvailFld");
            }
        }
        public bool StationAvailHnr
        {
            get
            {
                return _stationAvailHnr;
            }
            set
            {
                _stationAvailHnr = value;
                OnPropertyChanged("StationAvailHnr");
            }
        }
        public bool StationAvailIsn
        {
            get
            {
                return _stationAvailIsn;
            }
            set
            {
                _stationAvailIsn = value;
                OnPropertyChanged("StationAvailIsn");
            }
        }
        public bool StationAvailMem
        {
            get
            {
                return _stationAvailMem;
            }
            set
            {
                _stationAvailMem = value;
                OnPropertyChanged("StationAvailMem");
            }
        }
        public bool StationAvailNeu
        {
            get
            {
                return _stationAvailNeu;
            }
            set
            {
                _stationAvailNeu = value;
                OnPropertyChanged("StationAvailNeu");
            }
        }
        public bool StationAvailNhb
        {
            get
            {
                return _stationAvailNhb;
            }
            set
            {
                _stationAvailNhb = value;
                OnPropertyChanged("StationAvailNhb");
            }
        }
        public bool StationAvailOft
        {
            get
            {
                return _stationAvailOft;
            }
            set
            {
                _stationAvailOft = value;
                OnPropertyChanged("StationAvailOft");
            }
        }
        public bool StationAvailPro
        {
            get
            {
                return _stationAvailPro;
            }
            set
            {
                _stationAvailPro = value;
                OnPropertyChanged("StationAvailPro");
            }
        }
        public bool StationAvailRos
        {
            get
            {
                return _stationAvailRos;
            }
            set
            {
                _stationAvailRos = value;
                OnPropertyChanged("StationAvailRos");
            }
        }
        public bool StationAvailTur
        {
            get
            {
                return _stationAvailTur;
            }
            set
            {
                _stationAvailTur = value;
                OnPropertyChanged("StationAvailTur");
            }
        }
        public bool StationAvailUmd
        {
            get
            {
                return _stationAvailUmd;
            }
            set
            {
                _stationAvailUmd = value;
                OnPropertyChanged("StationAvailUmd");
            }
        }

        public int MesoCountIndicator
        {
            get
            {
                if (Mesocyclones.Count <= 0)
                {
                    return 0;
                }
                else if (Mesocyclones.Count < 2)
                {
                    return 1;
                }
                else if (Mesocyclones.Count < 4)
                {
                    return 2;
                }
                else if (Mesocyclones.Count < 8)
                {
                    return 3;
                }
                else if (Mesocyclones.Count < 10)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
            set
            {
                _mesoCountIndicator = value;
                OnPropertyChanged("MesoCountIndicator");
            }
        }

        public OpenDataElement() { }
        public OpenDataElement(DateTime time, List<RadarStation> radarStations, List<Mesocyclone> mesocyclones)
        {
            this.Time = time;
            this.RadarStations = radarStations;
            this.Mesocyclones = mesocyclones;

            foreach (var station in radarStations)
            {
                string name = station.Name;
                bool avail = station.IsAvailable;

                switch (name)
                {
                    case "ASB":
                        StationAvailAsb = avail;
                        break;
                    case "BOO":
                        StationAvailBoo = avail;
                        break;
                    case "DRS":
                        StationAvailDrs = avail;
                        break;
                    case "EIS":
                        StationAvailEis = avail;
                        break;
                    case "ESS":
                        StationAvailEss = avail;
                        break;
                    case "FBG":
                        StationAvailFbg = avail;
                        break;
                    case "FLD":
                        StationAvailFld = avail;
                        break;
                    case "HNR":
                        StationAvailHnr = avail;
                        break;
                    case "ISN":
                        StationAvailIsn = avail;
                        break;
                    case "MEM":
                        StationAvailMem = avail;
                        break;
                    case "NEU":
                        StationAvailNeu = avail;
                        break;
                    case "NHB":
                        StationAvailNhb = avail;
                        break;
                    case "OFT":
                        StationAvailOft = avail;
                        break;
                    case "PRO":
                        StationAvailPro = avail;
                        break;
                    case "ROS":
                        StationAvailRos = avail;
                        break;
                    case "TUR":
                        StationAvailTur = avail;
                        break;
                    case "UMD":
                        StationAvailUmd = avail;
                        break;
                }
            }
        }

        public static OpenDataElement GetPreviousOpenDataElement(List<OpenDataElement> elementList, OpenDataElement referenceElement)
        {
            DateTime elementTime = referenceElement.Time;
            DateTime searchTime = elementTime.Subtract(new TimeSpan(0, 5, 0));
            OpenDataElement previousElement = elementList.Find(x => x.Time == searchTime);
            return previousElement;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
