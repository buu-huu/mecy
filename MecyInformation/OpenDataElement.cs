using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyInformation
{
    class OpenDataElement
    {
        private DateTime _time;
        private List<RadarStation> _radarStations;
        private List<Mesocyclone> _mesocyclones;

        public DateTime Time { get => _time; set => _time = value; }
        public List<Mesocyclone> Mesocyclones { get => _mesocyclones; set => _mesocyclones = value; }
        internal List<RadarStation> RadarStations { get => _radarStations; set => _radarStations = value; }

        public OpenDataElement() { }
        public OpenDataElement(DateTime time, List<RadarStation> radarStations, List<Mesocyclone> mesocyclones)
        {
            this._time = time;
            this._radarStations = radarStations;
            this._mesocyclones = mesocyclones;
        }
    }
}
