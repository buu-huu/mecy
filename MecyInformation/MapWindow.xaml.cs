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
            var mesoFeature = new Feature {
                Geometry = new Mapsui.Geometries.Point(meso.Longitude, meso.Latitude),
            };

            SymbolStyle style = new SymbolStyle();
            switch (meso.Intensity)
            {
                case 1:
                    style = CreatePngStyle("MecyInformation.Resources.meso_icon_map_1.png", 0.6);
                    break;
                case 2:
                    style = CreatePngStyle("MecyInformation.Resources.meso_icon_map_2.png", 0.6);
                    break;
                case 3:
                    style = CreatePngStyle("MecyInformation.Resources.meso_icon_map_3.png", 0.6);
                    break;
                case 4:
                    style = CreatePngStyle("MecyInformation.Resources.meso_icon_map_4.png", 0.6);
                    break;
                case 5:
                    style = CreatePngStyle("MecyInformation.Resources.meso_icon_map_5.png", 0.6);
                    break;
            }

            var features = new Features { mesoFeature };
            var dataSource = new MemoryProvider(features)
            {
                CRS = "EPSG:4326"
            };

            return new Layer
            {
                DataSource = dataSource,
                Name = "Meso Point",
                Style = style
            };
        }

        private static SymbolStyle CreatePngStyle(string embeddedResourcePath, double scale)
        {
            var bitmapId = GetBitmapIdForEmbeddedResource(embeddedResourcePath);
            return new SymbolStyle { BitmapId = bitmapId, SymbolScale = scale, SymbolOffset = new Offset(0.0, 0.5, true) };
        }

        private static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            var assembly = typeof(MapWindow).GetTypeInfo().Assembly;
            var image = assembly.GetManifestResourceStream(imagePath);
            return BitmapRegistry.Instance.Register(image);
        }
    }
}
