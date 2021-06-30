using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = Mapsui.Geometries.Point;

namespace MecyApplication
{
    /// <summary>
    /// MainWindow logic, that mainly handles map and event stuff.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CENTER_LONGITUDE = 10.160549;
        private const double CENTER_LATITUDE = 51.024813;
        private const double RADIUS_EQUATOR = 6378.1;
        private const string GOOGLE_MAPS_TILE_URL = "http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}";

        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel
        {
            get
            {
                return _mainViewModel;
            }
            set
            {
                _mainViewModel = value;
            }
        }

        /// <summary>
        /// Constructor, that holds an instance of the MainViewModel.
        /// </summary>
        /// <param name="mainViewModel">MainViewModel of the application.</param>
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            this.MainViewModel = mainViewModel;
            this.DataContext = MainViewModel;

            mapControl.Map = CreateMap();
            RefreshMap();

            // Event subscriptions
            MainViewModel.RefreshMapEvent += RefreshMap;
            MainViewModel.RefreshMapWidgetsEvent += RefreshMapWithWidgets;
            MainViewModel.CenterMapEvent += CenterMap;
            MainViewModel.CenterMapToMesoEvent += CenterMapToMeso;
            mapControl.Info += HandleMapClick;
        }        

        /// <summary>
        /// Creates a fresh map with all layers. According to some MapConfigurations, widgets and tilesources
        /// get set.
        /// </summary>
        /// <returns>Created map</returns>
        private Map CreateMap()
        {
            var map = new Map
            {
                Transformation = new MinimalTransformation(),
                CRS = "EPSG:3857",
                BackColor = Color.Gray
            };

            // Tile Source
            if (MainViewModel.CurrentMapConfiguration.ActiveTileSource == MapConfiguration.TileSource.OpenStreetMap) map.Layers.Add(OpenStreetMap.CreateTileLayer());
            else if (MainViewModel.CurrentMapConfiguration.ActiveTileSource == MapConfiguration.TileSource.GoogleMaps) map.Layers.Add(new TileLayer(CreateGoogleTileSource(GOOGLE_MAPS_TILE_URL)));

            // Layers
            map.Layers.Add(CreateRadarDiameterLayer());
            map.Layers.Add(CreateRadarLabelLayer());
            map.Layers.Add(CreateMesoDiameterLayer());
            map.Layers.Add(CreateMesoLayelLayer());
            map.Layers.Add(CreateMesoHistLayer());
            map.Layers.Add(CreateMesoLayer());

            // Widgets
            if (MainViewModel.CurrentMapConfiguration.ShowScaleBar) map.Widgets.Add(new ScaleBarWidget(map));
            if (MainViewModel.CurrentMapConfiguration.ShowZoomWidget) map.Widgets.Add(new ZoomInOutWidget());

            // Center Map
            map.Home = n => n.NavigateTo(FromLongLat(CENTER_LONGITUDE, CENTER_LATITUDE), map.Resolutions[6]);

            return map;
        }

        /// <summary>
        /// Refreshes the map.
        /// </summary>
        private void RefreshMap()
        {
            ClearMap();

            if (MainViewModel.SelectedElement == null) return;

            // Layers
            if (MainViewModel.CurrentMapConfiguration.ShowRadars)
            {
                DrawRadarDiametersToMap();
                DrawRadarLabelsToLayer();
            }
            if (MainViewModel.CurrentMapConfiguration.ShowMesocycloneIdLabel) DrawMesoLabelsToLayer();
            if (MainViewModel.CurrentMapConfiguration.ShowMesocycloneDiameter) DrawMesoDiametersToLayer();
            if (MainViewModel.CurrentMapConfiguration.ShowHistoricMesocyclones) DrawMesosHistToLayer();

            DrawMesosToLayer();
        }

        /// <summary>
        /// Refreshes the map and the widgets.
        /// </summary>
        private void RefreshMapWithWidgets()
        {
            var map = CreateMap();
            mapControl.Map = map;

            RefreshMap();
        }

        /// <summary>
        /// Clears the map.
        /// </summary>
        private void ClearMap()
        {
            var layerMeso = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLayer");
            var layerMesoDiameter = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoDiameterLayer");
            var layerMesoHist = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoHistLayer");
            var layerMesoLabel = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLabelLayer");
            var layerRadarDiameter = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "RadarDiameterLayer");
            var layerRadarLabel = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "RadarLabelLayer");

            layerMeso.Clear();
            layerMesoDiameter.Clear();
            layerMesoHist.Clear();
            layerMesoLabel.Clear();
            layerRadarDiameter.Clear();
            layerRadarLabel.Clear();
            mapControl.RefreshGraphics();
        }

        /// <summary>
        /// Centers the map to the standard position.
        /// </summary>
        private void CenterMap()
        {
            mapControl.Map = CreateMap();
            RefreshMap();
        }

        /// <summary>
        /// Centers the map to the selected mesocyclone.
        /// </summary>
        private void CenterMapToMeso()
        {
            var map = CreateMap();
            var selectedMeso = MainViewModel.SelectedMesocyclone;

            if (selectedMeso == null) return;

            map.Home = n => n.NavigateTo(FromLongLat(selectedMeso.Longitude, selectedMeso.Latitude), map.Resolutions[8]);
            mapControl.Map = map;
            RefreshMap();
        }

        // -------------------- MESO LAYER --------------------
        /// <summary>
        /// Creates a mesocyclone layer.
        /// </summary>
        /// <returns>Mesocyclone layer</returns>
        private WritableLayer CreateMesoLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoLayer",
                Style = null,
                IsMapInfoLayer = true
            };
            return layer;
        }

        /// <summary>
        /// Creates a mesocyclone feature. Checks, if the mesocyclone is the currently selected mesocyclone
        /// and loads the correct image file.
        /// </summary>
        /// <param name="meso">Mesocyclone</param>
        /// <param name="selectedMeso">Currently selected mesocyclone</param>
        /// <returns>Mesocyclone feature</returns>
        private Feature CreateMesoFeature(Mesocyclone meso, Mesocyclone selectedMeso)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(meso.Longitude, meso.Latitude)
            };
            var style = new SymbolStyle();
            if (meso == selectedMeso)
            {
                switch (meso.Intensity)
                {
                    case 1:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_1.png", 0.6);
                        break;
                    case 2:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_2.png", 0.6);
                        break;
                    case 3:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_3.png", 0.6);
                        break;
                    case 4:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_4.png", 0.6);
                        break;
                    case 5:
                        style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_5.png", 0.6);
                        break;
                }
            }
            else
            {
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
            }
            feature.Styles.Add(style);
            return feature;
        }

        /// <summary>
        /// Creates a PNG symbol style.
        /// </summary>
        /// <param name="embeddedResourcePath">Path of the embedded resource</param>
        /// <param name="scale">Scale of the image</param>
        /// <returns></returns>
        private static SymbolStyle CreatePngStyle(string embeddedResourcePath, double scale)
        {
            var bitmapId = GetBitmapIdForEmbeddedResource(embeddedResourcePath);
            return new SymbolStyle { BitmapId = bitmapId, SymbolScale = scale, SymbolOffset = new Offset(0.0, 0.5, true) };
        }

        /// <summary>
        /// Returns the bitmap ID for an embedded resource.
        /// </summary>
        /// <param name="imagePath">Path of the image</param>
        /// <returns>ID of the embedded resource</returns>
        private static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            var assembly = typeof(MainWindow).GetTypeInfo().Assembly;
            var image = assembly.GetManifestResourceStream(imagePath);
            return BitmapRegistry.Instance.Register(image);
        }

        /// <summary>
        /// Draws mesocyclones of the currently selected opendata element to the mesocyclone layer.
        /// </summary>
        private void DrawMesosToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null || MainViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }

            foreach (var meso in MainViewModel.SelectedElement.Mesocyclones)
            {
                layer.Add(CreateMesoFeature(meso, MainViewModel.SelectedMesocyclone));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- MESO DIAMETER LAYER --------------------
        /// <summary>
        /// Creates a mesocyclone diameter layer.
        /// </summary>
        /// <returns>Mesocyclone diameter layer</returns>
        private WritableLayer CreateMesoDiameterLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoDiameterLayer",
                Style = null
            };
            return layer;
        }

        /// <summary>
        /// Creates a mesocyclone diameter polygon.
        /// </summary>
        /// <param name="meso">Mesocyclone</param>
        /// <returns>Mesocyclone diameter polygon</returns>
        private Polygon CreateMesoDiameterPolygon(Mesocyclone meso)
        {
            var polygon = new Polygon();

            var mesoCenterLon = meso.Longitude;
            var mesoCenterLat = meso.Latitude;
            double radius = meso.Diameter / 2;

            for (int step = 0; step <= 360; step += 5)
            {
                polygon.ExteriorRing.Vertices.Add(
                    GetDistancePoint(mesoCenterLon, mesoCenterLat, step, radius));
            }
            return polygon;
        }

        /// <summary>
        /// Draws mesocyclone diameters to layer.
        /// </summary>
        private void DrawMesoDiametersToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoDiameterLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null || MainViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }
            var style = new VectorStyle
            {
                Fill = new Brush(new Color(255, 97, 247, 50)),
                Outline = new Pen
                {
                    Color = new Color(121, 97, 255, 200),
                    Width = 2,
                    PenStyle = PenStyle.LongDashDot,
                    PenStrokeCap = PenStrokeCap.Butt
                }
            };
            foreach (var meso in MainViewModel.SelectedElement.Mesocyclones)
            {
                layer.Add(new Feature
                {
                    Geometry = CreateMesoDiameterPolygon(meso)
                });
                layer.Style = style;
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- MESO LABEL LAYER --------------------
        /// <summary>
        /// Creates a mesocyclone label layer.
        /// </summary>
        /// <returns>Mesocyclone label layer</returns>
        private WritableLayer CreateMesoLayelLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoLabelLayer",
                Style = null
            };
            return layer;
        }

        /// <summary>
        /// Creates a mesocyclone label feature.
        /// </summary>
        /// <param name="meso">Mesocyclone</param>
        /// <returns>Mesocyclone label feature</returns>
        private Feature CreateMesoLabelFeature(Mesocyclone meso)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(meso.Longitude, meso.Latitude)
            };
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
                    backColor = new Brush(new Color(255, 0, 255));
                    break;
                default:
                    backColor = new Brush(Color.White);
                    break;
            }
            feature.Styles.Add(new LabelStyle
            {
                Offset = new Offset(0.0, -6.5, true),
                Text = meso.Id.ToString(),
                BackColor = backColor,
                ForeColor = Color.Black
            });
            return feature;
        }

        /// <summary>
        /// Draws mesocyclone labels to layer.
        /// </summary>
        private void DrawMesoLabelsToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLabelLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null || MainViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }

            foreach (var meso in MainViewModel.SelectedElement.Mesocyclones)
            {
                layer.Add(CreateMesoLabelFeature(meso));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- MESO HIST LAYER --------------------
        /// <summary>
        /// Creates a historic mesocyclone layer.
        /// </summary>
        /// <returns>Historic mesocyclone layer</returns>
        private WritableLayer CreateMesoHistLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoHistLayer",
                Style = null
            };
            return layer;
        }

        /// <summary>
        /// Creates a historic mesocyclone feature.
        /// </summary>
        /// <param name="meso">Historic mesocyclone</param>
        /// <returns>Historic mesocyclone feature</returns>
        private Feature CreateMesoHistFeature(Mesocyclone meso)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(meso.Longitude, meso.Latitude)
            };
            var style = new SymbolStyle();
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
            if (MainViewModel.CurrentMapConfiguration.HistoricMesocyclonesTransparent)
            {
                style.Opacity = MainViewModel.CurrentMapConfiguration.HistoricMesocyclonesOpacity;
            }
            feature.Styles.Add(style);
            return feature;
        }

        /// <summary>
        /// Draws historic mesocyclones to layer.
        /// </summary>
        private void DrawMesosHistToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoHistLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null || MainViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }

            OpenDataElement previousElement = OpenDataElement.GetPreviousOpenDataElement(
                MainViewModel.OpenDataElements.ToList(),
                MainViewModel.SelectedElement);
            if (previousElement == null) return;

            foreach (var meso in previousElement.Mesocyclones)
            {
                layer.Add(CreateMesoHistFeature(meso));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- RADAR LABEL LAYER --------------------
        private WritableLayer CreateRadarLabelLayer()
        {
            var layer = new WritableLayer
            {
                Name = "RadarLabelLayer",
                Style = null
            };
            return layer;
        }

        private Feature CreateRadarLabelFeature(RadarStation station)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(station.Longitude, station.Latitude)
            };
            LabelStyle style = new LabelStyle
            {
                Offset = new Offset(0.0, 0.0, true),
                Text = station.Name
            };
            if (station.IsAvailable)
            {
                style.ForeColor = Color.Green;
                style.BackColor = new Brush(Color.White);
            }
            else
            {
                style.ForeColor = Color.White;
                style.BackColor = new Brush(Color.Red);
            }
            feature.Styles.Add(style);
            return feature;
        }

        private void DrawRadarLabelsToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "RadarLabelLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null)
            {
                return;
            }

            foreach (var station in MainViewModel.SelectedElement.RadarStations)
            {
                layer.Add(CreateRadarLabelFeature(station));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- RADAR DIAMETER LAYER --------------------
        private WritableLayer CreateRadarDiameterLayer()
        {
            var layer = new WritableLayer
            {
                Name = "RadarDiameterLayer",
                Style = null
            };
            return layer;
        }

        private Polygon CreateRadarDiameterPolygon(RadarStation radar)
        {
            var polygon = new Polygon();

            var radarCenterLon = radar.Longitude;
            var radarCenterLat = radar.Latitude;
            double radius = 150;

            for (int step = 0; step <= 360; step += 5)
            {
                polygon.ExteriorRing.Vertices.Add(
                    GetDistancePoint(radarCenterLon, radarCenterLat, step, radius));
            }
            return polygon;
        }

        private void DrawRadarDiametersToMap()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "RadarDiameterLayer");
            layer.Clear();
            mapControl.RefreshGraphics();
            if (MainViewModel.SelectedElement == null)
            {
                return;
            }
            
            foreach (var radar in MainViewModel.SelectedElement.RadarStations)
            {
                
                var style = new VectorStyle
                {
                    Fill = new Brush(new Color(84, 84, 255, 10)),
                    Outline = new Pen
                    {
                        Color = new Color(23, 23, 255, 100),
                        Width = 2,
                        PenStyle = PenStyle.ShortDash,
                        PenStrokeCap = PenStrokeCap.Round
                    }
                };

                layer.Add(new Feature
                {
                    Geometry = CreateRadarDiameterPolygon(radar)
                });
                layer.Style = style;
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- EVENT HANDLING --------------------
        /// <summary>
        /// Handles the click on the map.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        public void HandleMapClick(object sender, MapInfoEventArgs args)
        {
            var mapInfo = args.MapInfo;

            // Select clicked mesocyclone
            var meso = GetBestMesocyclone((Feature)mapInfo.Feature);
            if (meso != null) MainViewModel.SelectedMesocyclone = meso;
        }

        /// <summary>
        /// Handles selection changes on <c>lvOpenDataElements</c>
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void lvOpenDataElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMap();
        }

        /// <summary>
        /// Handles selection changes on <c>lvMesocyclones</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvMesocyclones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMap();
        }

        /// <summary>
        /// Handles window close event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Environment.Exit(0); // Close whole application. Not only the window.
        }

        // -------------------- GOOGLE MAPS TILE DOWNLOAD --------------------
        /// <summary>
        /// Creates a tilesource for google maps.
        /// </summary>
        /// <param name="urlFormatter">URL formatter</param>
        /// <returns>Tilesource</returns>
        private static ITileSource CreateGoogleTileSource(string urlFormatter)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), urlFormatter, new[] { "0", "1", "2", "3" },
                tileFetcher: FetchGoogleTile);
        }

        /// <summary>
        /// Creates a byte array of google maps tiles.
        /// </summary>
        /// <param name="uri">URI</param>
        /// <returns>Tilebytes</returns>
        private static byte[] FetchGoogleTile(Uri uri)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://maps.google.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", @"Mozilla / 5.0(Windows; U; Windows NT 6.0; en - US; rv: 1.9.1.7) Gecko / 20091221 Firefox / 3.5.7");

            return httpClient.GetByteArrayAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        // -------------------- MATH HELPERS --------------------
        /// <summary>
        /// Creates a point from a given coordinate, rotation and distance.
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="distance">Distance</param>
        /// <returns>Distance point</returns>
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

        /// <summary>
        /// Returns the nearest mesocyclone instance to a feature.
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>Nearest mesocyclone</returns>
        private Mesocyclone GetBestMesocyclone(Feature feature)
        {
            if (feature == null) return null;

            Point point = (Point)feature.Geometry;
            Point pos = SphericalMercator.ToLonLat(point.X, point.Y);
            double longitude = Math.Round(pos.X, 6, MidpointRounding.AwayFromZero);
            double latitude = Math.Round(pos.Y, 6, MidpointRounding.AwayFromZero);

            Mesocyclone bestMeso = MainViewModel.SelectedElement.Mesocyclones[0];
            double diffBestLong = 1000;
            double diffBestLat = 1000;

            foreach (var meso in MainViewModel.SelectedElement.Mesocyclones)
            {
                double longMeso = Math.Round(meso.Longitude, 6, MidpointRounding.AwayFromZero);
                double latMeso = Math.Round(meso.Latitude, 6, MidpointRounding.AwayFromZero);

                if (Math.Abs(longMeso - longitude) < diffBestLong || Math.Abs(latMeso - latitude) < diffBestLat)
                {
                    diffBestLong = Math.Abs(longMeso - longitude);
                    diffBestLat = Math.Abs(latMeso - latitude);
                    bestMeso = meso;
                }
            }
            return bestMeso;
        }

        /// <summary>
        /// Creates an Mapsui point from a coordinate.
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        /// <returns>Point</returns>
        private static Point FromLongLat(double longitude, double latitude)
        {
            return SphericalMercator.FromLonLat(longitude, latitude);
        }

        /// <summary>
        /// Converts a radians value to degrees value
        /// </summary>
        /// <param name="radians">Radians</param>
        /// <returns>Degrees value</returns>
        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return degrees;
        }

        /// <summary>
        /// Converts a degrees value to radians value
        /// </summary>
        /// <param name="degrees">Degrees</param>
        /// <returns>Radians value</returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return radians;
        }
    }
}
