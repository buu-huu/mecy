using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        Mesocyclone meso;

        public MapWindow(Mesocyclone meso)
        {
            InitializeComponent();
            
            this.meso = meso;
            mapControl.Map = MapBuilder.CreateMap(meso);
            gridInformation.DataContext = meso;
        }
    }
}
