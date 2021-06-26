using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using Mapsui.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MecyApplication
{
    /// <summary>
    /// Interaktionslogik für MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow(Mesocyclone meso)
        {
            InitializeComponent();
            
            mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone> { meso }, MapConfiguration.Instance);
            gridInformation.DataContext = meso;
        }

        private void mapControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void Test(object sender, MapInfoEventArgs e)
        {

        }
    }
}
