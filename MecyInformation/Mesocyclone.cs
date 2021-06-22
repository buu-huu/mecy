using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MecyInformation
{
    public class Mesocyclone
    {
        private int _id;
        private DateTime _time;
        private double _latitude;
        private double _longitude;
        private double _polarMotion = 0;   // Currently not used. Using Default
        private double _majorAxis = 10;    // Currently not used. Using Default
        private double _minorAxis = 10;    // Currently not used. Using Default
        private int _orientation = 0;      // Currently not used. Using Default
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

        //private List<Elevation> _elevations;

        private double _meanDBZ;
        private double _maxDBZ;
        private double _velocityMax;
        private double _velocityRotationalMax;
        private double _velocityRotationalMean;
        private double _velocityRotationalMaxClosestToGround;

        private int _intensity;

        /* PROPERTIES */
        public int Id { get => _id; set => _id = value; }
        public DateTime Time { get => _time; set => _time = value; }
        public double Latitude { get => _latitude; set => _latitude = value; }
        public double Longitude { get => _longitude; set => _longitude = value; }
        public double PolarMotion { get => _polarMotion; set => _polarMotion = value; }
        public double MajorAxis { get => _majorAxis; set => _majorAxis = value; }
        public double MinorAxis { get => _minorAxis; set => _minorAxis = value; }
        public int Orientation { get => _orientation; set => _orientation = value; }
        public double ShearMean { get => _shearMean; set => _shearMean = value; }
        public double ShearMax { get => _shearMax; set => _shearMax = value; }
        public double MomentumMean { get => _momentumMean; set => _momentumMean = value; }
        public double MomentumMax { get => _momentumMax; set => _momentumMax = value; }
        public double Diameter { get => _diameter; set => _diameter = value; }
        public double DiameterEquivalent { get => _diameterEquivalent; set => _diameterEquivalent = value; }
        public double Top { get => _top; set => _top = value; }
        public double MesoBase { get => _mesoBase; set => _mesoBase = value; }
        public double Echotop { get => _echotop; set => _echotop = value; }
        public double Vil { get => _vil; set => _vil = value; }
        public int ShearVectors { get => _shearVectors; set => _shearVectors = value; }
        public int ShearFeatures { get => _shearFeatures; set => _shearFeatures = value; }
        public double MeanDBZ { get => _meanDBZ; set => _meanDBZ = value; }
        public double MaxDBZ { get => _maxDBZ; set => _maxDBZ = value; }
        public double VelocityMax { get => _velocityMax; set => _velocityMax = value; }
        public double VelocityRotationalMax { get => _velocityRotationalMax; set => _velocityRotationalMax = value; }
        public double VelocityRotationalMean { get => _velocityRotationalMean; set => _velocityRotationalMean = value; }
        public double VelocityRotationalMaxClosestToGround { get => _velocityRotationalMaxClosestToGround; set => _velocityRotationalMaxClosestToGround = value; }
        public int Intensity { get => _intensity; set => _intensity = value; }

        public Mesocyclone()
        {

        }

        public Mesocyclone(int id, string time, double latitude, double longitude, double polarMotion,
                   double majorAxis, double minorAxis, int orientation, double shearMean, double shearMax,
                   double momentumMean, double momentumMax, double diameter, double diameterEquivalent,
                   double top, double mesoBase, double echotop, double vil, int shearVectors, int shearFeatures,
                   double meanDBZ, double maxDBZ, double velocityMax,
                   double velocityRotationalMax, double velocityRotationalMean,
                   double velocityRotationalMaxClosestToGround, int intensity)
        {
            this._id = id;
            this._time = DateTime.Parse(time);
            this._latitude = latitude;
            this._longitude = longitude;
            this._polarMotion = polarMotion;
            this._majorAxis = majorAxis;
            this._minorAxis = minorAxis;
            this._orientation = orientation;
            this._shearMean = shearMean;
            this._shearMax = shearMax;
            this._momentumMean = momentumMean;
            this._momentumMax = momentumMax;
            this._diameter = diameter;
            this._diameterEquivalent = diameterEquivalent;
            this._top = top;
            this._mesoBase = mesoBase;
            this._echotop = echotop;
            this._vil = vil;
            this._shearVectors = shearVectors;
            this._shearFeatures = shearFeatures;
            this._meanDBZ = meanDBZ;
            this._maxDBZ = maxDBZ;
            this._velocityMax = velocityMax;
            this._velocityRotationalMax = velocityRotationalMax;
            this._velocityRotationalMean = velocityRotationalMean;
            this._velocityRotationalMaxClosestToGround = velocityRotationalMaxClosestToGround;
            this._intensity = intensity;
        }

        public override string ToString()
        {
            return "Mesocyclone{" +
                "id=" + _id +
                ", time='" + _time + '\'' +
                ", latitude=" + _latitude +
                ", longitude=" + _longitude +
                ", polarMotion=" + _polarMotion +
                ", majorAxis=" + _majorAxis +
                ", minorAxis=" + _minorAxis +
                ", orientation=" + _orientation +
                ", shearMean=" + _shearMean +
                ", shearMax=" + _shearMax +
                ", momentumMean=" + _momentumMean +
                ", momentumMax=" + _momentumMax +
                ", diameter=" + _diameter +
                ", diameterEquivalent=" + _diameterEquivalent +
                ", top=" + _top +
                ", base=" + _mesoBase +
                ", echotop=" + _echotop +
                ", vil=" + _vil +
                ", shearVectors=" + _shearVectors +
                ", shearFeatures=" + _shearFeatures +
                ", meanDBZ=" + _meanDBZ +
                ", maxDBZ=" + _maxDBZ +
                ", velocityMax=" + _velocityMax +
                ", velocityRotationalMax=" + _velocityRotationalMax +
                ", velocityRotationalMean=" + _velocityRotationalMean +
                ", velocityRotationalMaxClosestToGround=" + _velocityRotationalMaxClosestToGround +
                ", intensity=" + _intensity +
                '}';
        }
    }
}
