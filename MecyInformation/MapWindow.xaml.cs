using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;
using System.Windows;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();
            mapControl.Map = CreateMap();
        }

        public static Map CreateMap()
        {
            var map = new Map
            {
                Transformation = new MinimalTransformation(),
                CRS = "EPSG:3857",
                BackColor = Color.Gray
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreateMesoLayer());
            return map;
        }

        private static Layer CreateMesoLayer()
        {
            var mesoFeature = new Feature { Geometry = new Mapsui.Geometries.Point(9.153820, 48.698847) };
            mesoFeature.Styles.Add(new LabelStyle
            {
                Text = "M",
                ForeColor = Color.Red
            });
            var features = new Features { mesoFeature };
            var dataSource = new MemoryProvider(features)
            {
                CRS = "EPSG:4326"
            };

            return new Layer
            {
                DataSource = dataSource,
                Name = "MESO Point"
            };
        }
    }
}
