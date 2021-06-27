using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MecyApplication
{
    public static class MapBuilder
    {
        private const double CENTER_LONGITUDE = 10.160549;
        private const double CENTER_LATITUDE  = 51.024813;
        private const double RADIUS_EQUATOR   = 6378.1;

        private const string GOOGLE_MAPS_TILE_URL = "http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}";
        
        public static Map CreateMap(List<Mesocyclone> mesocyclones, List<Mesocyclone> historicMesocyclones, MapConfiguration mapConfiguration)
        {
            if (mesocyclones == null)
            {
                return new Map();
            }
            var map = new Map
            {
                Transformation = new MinimalTransformation(),
                CRS = "EPSG:3857",
                BackColor = Color.Gray
            };
            
            /* Widgets */
            if (mapConfiguration.ShowScaleBar)
            {
                map.Widgets.Add(new ScaleBarWidget(map));
            }
            if (mapConfiguration.ShowZoomWidget)
            {
                map.Widgets.Add(new ZoomInOutWidget());
            }

            /* Layers */
            if (mapConfiguration.ActiveTileSource == MapConfiguration.TileSource.OpenStreetMap)
            {
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
            }
            else if (mapConfiguration.ActiveTileSource == MapConfiguration.TileSource.GoogleMaps)
            {
                map.Layers.Add(new TileLayer(CreateGoogleTileSource(GOOGLE_MAPS_TILE_URL)));
            }

            if (mapConfiguration.ShowMesocycloneDiameter)
            {
                map.Layers.Add(CreateMesoDiameterLayer(mesocyclones));
            }
            if (historicMesocyclones != null)
            {
                map.Layers.Add(CreateHistoricMesoLayer(historicMesocyclones, mapConfiguration));
            }
            map.Layers.Add(CreateMesoLayer(mesocyclones));
            map.Layers.Add(CreateMesoLabelLayer(mesocyclones));

            /* Center Map */
            if (mesocyclones.Count == 1)
            {
                map.Home = n => n.NavigateTo(FromLongLat(mesocyclones[0].Longitude, mesocyclones[0].Latitude), map.Resolutions[7]);
            }
            else
            {
                map.Home = n => n.NavigateTo(FromLongLat(CENTER_LONGITUDE, CENTER_LATITUDE), map.Resolutions[6]);
            }
            return map;
        }

        private static Layer CreateMesoLayer(List<Mesocyclone> mesocyclones)
        {
            var features = new Features();

            foreach (var meso in mesocyclones)
            {
                var mesoFeature = new Feature
                {
                    Geometry = new Point(meso.Longitude, meso.Latitude)
                };
                SymbolStyle style = new SymbolStyle();
                switch (meso.Intensity)
                {
                    case 1:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_1.png", 0.6);
                        break;
                    case 2:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_2.png", 0.6);
                        break;
                    case 3:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_3.png", 0.6);
                        break;
                    case 4:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_4.png", 0.6);
                        break;
                    case 5:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_5.png", 0.6);
                        break;
                }
                mesoFeature.Styles.Add(style);
                features.Add(mesoFeature);
            }
            

            var dataSource = new MemoryProvider(features)
            {
                CRS = "EPSG:4326"
            };

            return new Layer
            {
                DataSource = dataSource,
                Name = "Mesocyclones",
                Style = null
            };
        }

        private static Layer CreateHistoricMesoLayer(List<Mesocyclone> mesocyclones, MapConfiguration mapConfiguration)
        {
            var features = new Features();

            foreach (var meso in mesocyclones)
            {
                var mesoFeature = new Feature
                {
                    Geometry = new Point(meso.Longitude, meso.Latitude),
                };
                SymbolStyle style = new SymbolStyle();
                switch (meso.Intensity)
                {
                    case 1:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_1.png", 0.6);
                        break;
                    case 2:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_2.png", 0.6);
                        break;
                    case 3:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_3.png", 0.6);
                        break;
                    case 4:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_4.png", 0.6);
                        break;
                    case 5:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_5.png", 0.6);
                        break;
                }
                if (mapConfiguration.HistoricMesocyclonesTransparent)
                {
                    style.Opacity = mapConfiguration.HistoricMesocyclonesOpacity;
                }
                mesoFeature.Styles.Add(style);
                features.Add(mesoFeature);
            }

            var dataSource = new MemoryProvider(features)
            {
                CRS = "EPSG:4326"
            };

            return new Layer
            {
                DataSource = dataSource,
                Name = "Meso Point",
                Style = null
            };
        }

        private static Layer CreateMesoDiameterLayer(List<Mesocyclone> mesocyclones)
        {
            var features = new Features();
            return new Layer("Diameter Layer")
            {
                DataSource = new MemoryProvider(CreateDiameterCircles(mesocyclones)),
                Style = new VectorStyle
                {
                    Fill = new Brush(new Color(255, 97, 247, 50)),
                    Outline = new Pen
                    {
                        Color = new Color(121, 97, 255, 200),
                        Width = 2,
                        PenStyle = PenStyle.LongDashDot,
                        PenStrokeCap = PenStrokeCap.Butt
                    }
                }
            };
        }

        private static Layer CreateMesoLabelLayer(List<Mesocyclone> mesocyclones)
        {
            var features = new Features();
            foreach (Mesocyclone meso in mesocyclones)
            {
                Brush backColor;
                switch (meso.Intensity)
                {
                    case 1:
                        backColor = new Brush(new Color(0, 255, 0));
                        break;
                    case 2:
                        backColor = new Brush(new Color(255, 255, 0));
                        break;
                    case 3:
                        backColor = new Brush(new Color(241, 130, 36));
                        break;
                    case 4:
                        backColor = new Brush(new Color(255, 0, 0));
                        break;
                    case 5:
                        backColor = new Brush(new Color(255, 255, 0));
                        break;
                    default:
                        backColor = new Brush(Color.White);
                        break;
                }

                var mesoLabel = new Feature { Geometry = FromLongLat(meso.Longitude, meso.Latitude) };
                mesoLabel.Styles.Add(new LabelStyle
                {
                    Offset = new Offset(0.0, -6.5, true),
                    Text = meso.Id.ToString(),
                    BackColor = backColor,
                    ForeColor = Color.Black
                });
                features.Add(mesoLabel);
            }
            return new Layer("Meso Label Layer")
            {
                DataSource = new MemoryProvider(features),
                Style = null
            };
        }

        private static List<Polygon> CreateDiameterCircles(List<Mesocyclone> mesocyclones)
        {
            var result = new List<Polygon>();
            foreach (var meso in mesocyclones)
            {
                var polygon = new Polygon();
                var mesoCenterLon = meso.Longitude;
                var mesoCenterLat = meso.Latitude;
                double radius = meso.Diameter/2;

                for (int step = 0; step <= 360; step += 5)
                {
                    polygon.ExteriorRing.Vertices.Add(
                        GetDistancePoint(mesoCenterLon, mesoCenterLat, step, radius));
                }
                result.Add(polygon);
            }
            return result;
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

        private static ITileSource CreateGoogleTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, new[] { "0", "1", "2", "3" },
                tileFetcher: FetchGoogleTile);
        }

        private static byte[] FetchGoogleTile(Uri arg)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://maps.google.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");

            return httpClient.GetByteArrayAsync(arg).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /* Math Helpers */
        private static Point GetDistancePoint(double longitude, double latitude, double rotation, double distance)
        {
            double brng = ConvertDegreesToRadians(rotation); // In radians! 90° == 1.57

            double latitudeRad = ConvertDegreesToRadians(latitude);
            double longitudeRad = ConvertDegreesToRadians(longitude);

            double latitudeResult = Math.Asin(Math.Sin(latitudeRad) * Math.Cos(distance / RADIUS_EQUATOR) +
                Math.Cos(latitudeRad) * Math.Sin(distance / RADIUS_EQUATOR) * Math.Cos(brng));

            double longitudeResult = longitudeRad + Math.Atan2(Math.Sin(brng) * Math.Sin(distance / RADIUS_EQUATOR) * Math.Cos(latitudeRad),
                Math.Cos(distance / RADIUS_EQUATOR) - Math.Sin(latitudeRad) * Math.Sin(latitudeResult));

            latitudeResult = ConvertRadiansToDegrees(latitudeResult);
            longitudeResult = ConvertRadiansToDegrees(longitudeResult);
            return FromLongLat(longitudeResult, latitudeResult);
        }

        private static Point FromLongLat(double longitude, double latitude)
        {
            return SphericalMercator.FromLonLat(longitude, latitude);
        }

        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return degrees;
        }

        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return radians;
        }
    }
}
