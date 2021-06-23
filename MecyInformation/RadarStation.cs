using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecyInformation
{
    class RadarStation
    {
        private string _name;
        private bool _isAvailable;

        public string Name { get => _name; set => _name = value; }
        public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }
    }
}
