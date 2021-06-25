using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MecyApplication
{
    public class Mesocyclone : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _time;
        private double _latitude;
        private double _longitude;
        private double _polarMotion = 0;    // Currently not used. Using Default
        private double _majorAxis;          // Currently not used. Equals diameter
        private double _minorAxis;          // Currently not used. Equals diameter
        private int _orientation = 0;       // Currently not used. Using Default
        private double _shearMean;
        private double _shearMax;
        private double _momentumMean;
        private double _momentumMax;
        private double _diameter;
        private double _diameterEquivalent;
        private double _top;
        private double _mesoBase;
        private double _echotop;
        private double _vil;
        private int _shearVectors;
        private int _shearFeatures;

        private List<Elevation> _elevations;

        private double _meanDBZ;
        private double _maxDBZ;
        private double _velocityMax;
        private double _velocityRotationalMax;
        private double _velocityRotationalMean;
        private double _velocityRotationalMaxClosestToGround;

        private int _intensity;

        public event PropertyChangedEventHandler PropertyChanged;

        #region properties
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
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
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        public double PolarMotion
        {
            get
            {
                return _polarMotion;
            }
            set
            {
                _polarMotion = value;
                OnPropertyChanged("PolarMotion");
            }
        }
        public double MajorAxis
        {
            get
            {
                return _majorAxis;
            }
            set
            {
                _majorAxis = value;
                OnPropertyChanged("MajorAxis");
            }
        }
        public double MinorAxis
        {
            get
            {
                return _minorAxis;
            }
            set
            {
                _minorAxis = value;
                OnPropertyChanged("MinorAxis");
            }
        }
        public int Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                OnPropertyChanged("Orientation");
            }
        }
        public double ShearMean
        {
            get
            {
                return _shearMean;
            }
            set
            {
                _shearMean = value;
                OnPropertyChanged("ShearMean");
            }
        }
        public double ShearMax
        {
            get
            {
                return _shearMax;
            }
            set
            {
                _shearMax = value;
                OnPropertyChanged("ShearMax");
            }
        }
        public double MomentumMean
        {
            get
            {
                return _momentumMean;
            }
            set
            {
                _momentumMean = value;
                OnPropertyChanged("MomentumMean");
            }
        }
        public double MomentumMax
        {
            get
            {
                return _momentumMax;
            }
            set
            {
                _momentumMax = value;
                OnPropertyChanged("MomentumMax");
            }
        }
        public double Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                _diameter = value;
                OnPropertyChanged("Diameter");
            }
        }
        public double DiameterEquivalent
        {
            get
            {
                return _diameterEquivalent;
            }
            set
            {
                _diameterEquivalent = value;
                OnPropertyChanged("DiameterEquivalent");
            }
        }
        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
                OnPropertyChanged("Top");
            }
        }
        public double MesoBase
        {
            get
            {
                return _mesoBase;
            }
            set
            {
                _mesoBase = value;
                OnPropertyChanged("MesoBase");
            }
        }
        public double Echotop
        {
            get
            {
                return _echotop;
            }
            set
            {
                _echotop = value;
                OnPropertyChanged("Echotop");
            }
        }
        public double Vil
        {
            get
            {
                return _vil;
            }
            set
            {
                _vil = value;
                OnPropertyChanged("Vil");
            }
        }
        public int ShearVectors
        {
            get
            {
                return _shearVectors;
            }
            set
            {
                _shearVectors = value;
                OnPropertyChanged("ShearVectors");
            }
        }
        public int ShearFeatures
        {
            get
            {
                return _shearFeatures;
            }
            set
            {
                _shearFeatures = value;
                OnPropertyChanged("ShearFeatures");
            }
        }
        public List<Elevation> Elevations
        {
            get
            {
                return _elevations;
            }
            set
            {
                _elevations = value;
                OnPropertyChanged("Elevations");
            }
        }
        public double MeanDBZ
        {
            get
            {
                return _meanDBZ;
            }
            set
            {
                _meanDBZ = value;
                OnPropertyChanged("MeanDBZ");
            }
        }
        public double MaxDBZ
        {
            get
            {
                return _maxDBZ;
            }
            set
            {
                _maxDBZ = value;
                OnPropertyChanged("MaxDBZ");
            }
        }
        public double VelocityMax
        {
            get
            {
                return _velocityMax;
            }
            set
            {
                _velocityMax = value;
                OnPropertyChanged("VelocityMax");
            }
        }
        public double VelocityRotationalMax
        {
            get
            {
                return _velocityRotationalMax;
            }
            set
            {
                _velocityRotationalMax = value;
                OnPropertyChanged("VelocityRotationalMax");
            }
        }
        public double VelocityRotationalMean
        {
            get
            {
                return _velocityRotationalMean;
            }
            set
            {
                _velocityRotationalMean = value;
                OnPropertyChanged("VelocityRotationalMean");
            }
        }
        public double VelocityRotationalMaxClosestToGround
        {
            get
            {
                return _velocityRotationalMaxClosestToGround;
            }
            set
            {
                _velocityRotationalMaxClosestToGround = value;
                OnPropertyChanged("VelocityRotationalMaxClosestToGround");
            }
        }
        public int Intensity
        {
            get
            {
                return _intensity;
            }
            set
            {
                _intensity = value;
                OnPropertyChanged("Intensity");
            }
        }
        #endregion properties

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return "Mesocyclone{" +
                "id=" + Id +
                ", time='" + Time + '\'' +
                ", latitude=" + Latitude +
                ", longitude=" + Longitude +
                ", polarMotion=" + PolarMotion +
                ", majorAxis=" + MajorAxis +
                ", minorAxis=" + MinorAxis +
                ", orientation=" + Orientation +
                ", shearMean=" + ShearMean +
                ", shearMax=" + ShearMax +
                ", momentumMean=" + MomentumMean +
                ", momentumMax=" + MomentumMax +
                ", diameter=" + Diameter +
                ", diameterEquivalent=" + DiameterEquivalent +
                ", top=" + Top +
                ", base=" + MesoBase +
                ", echotop=" + Echotop +
                ", vil=" + Vil +
                ", shearVectors=" + ShearVectors +
                ", shearFeatures=" + ShearFeatures +
                ", meanDBZ=" + MeanDBZ +
                ", maxDBZ=" + MaxDBZ +
                ", velocityMax=" + VelocityMax +
                ", velocityRotationalMax=" + VelocityRotationalMax +
                ", velocityRotationalMean=" + VelocityRotationalMean +
                ", velocityRotationalMaxClosestToGround=" + VelocityRotationalMaxClosestToGround +
                ", intensity=" + Intensity +
                '}';
        }
    }
}
