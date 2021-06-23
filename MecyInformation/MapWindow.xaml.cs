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
        Mesocyclone meso;

        public MapWindow(Mesocyclone meso)
        {
            InitializeComponent();
            this.meso = meso;
            mapControl.Map = CreateMap();
            gridInformation.DataContext = meso;
        }

        public Map CreateMap()
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

        private Layer CreateMesoLayer()
        {
            var mesoFeature = new Feature { Geometry = new Mapsui.Geometries.Point(meso.Longitude, meso.Latitude) };
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
