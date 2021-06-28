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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Point = Mapsui.Geometries.Point;

namespace MecyApplication
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CENTER_LONGITUDE = 10.160549;
        private const double CENTER_LATITUDE = 51.024813;
        private const double RADIUS_EQUATOR = 6378.1;
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

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            this.MainViewModel = mainViewModel;
            this.DataContext = MainViewModel;

            mapControl.Map = CreateMap();
            
            // Event subscriptions
            MainViewModel.RefreshMapEvent += RefreshMap;
        }

        private Map CreateMap()
        {
            var map = new Map();

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreateMesoDiameterLayer());
            map.Layers.Add(CreateMesoHistLayer());
            map.Layers.Add(CreateMesoLayer());
            map.Layers.Add(CreateMesoLayelLayer());

            return map;
        }

        private void RefreshMap()
        {
            ClearMap();
            if (MainViewModel.SelectedElement == null) return;

            if (MainViewModel.CurrentMapConfiguration.ShowMesocycloneDiameter) DrawMesoDiametersToLayer();
            if (MainViewModel.CurrentMapConfiguration.ShowHistoricMesocyclones) DrawMesosHistToLayer();
            if (MainViewModel.CurrentMapConfiguration.ShowMesocycloneIdLabel) DrawMesoLabelsToLayer();

            DrawMesosToLayer();
        }

        private void ClearMap()
        {
            var layerMeso = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLayer");
            var layerMesoDiameter = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoDiameterLayer");
            var layerMesoHist = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoHistLayer");
            var layerMesoLabel = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == "MesoLabelLayer");

            layerMeso.Clear();
            layerMesoDiameter.Clear();
            layerMesoHist.Clear();
            layerMesoLabel.Clear();
            mapControl.RefreshGraphics();
        }

        // -------------------- MESO LAYER --------------------
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

        private Feature CreateMesoFeature(Mesocyclone meso)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(meso.Longitude, meso.Latitude)
            };
            var style = new SymbolStyle();
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
            feature.Styles.Add(style);
            return feature;
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
                layer.Add(CreateMesoFeature(meso));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- MESO DIAMETER LAYER --------------------
        private WritableLayer CreateMesoDiameterLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoDiameterLayer",
                Style = null
            };
            return layer;
        }

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
        private WritableLayer CreateMesoLayelLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoLabelLayer",
                Style = null
            };
            return layer;
        }

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
                    backColor = new Brush(new Color(255, 255, 0));
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
        private WritableLayer CreateMesoHistLayer()
        {
            var layer = new WritableLayer
            {
                Name = "MesoHistLayer",
                Style = null
            };
            return layer;
        }

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
            feature.Styles.Add(style);
            return feature;
        }

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

        // -------------------- EVENT HANDLING --------------------
        private void MapControlOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            var position = args.GetPosition(mapControl).ToMapsui();
            var mapInfo = mapControl.GetMapInfo(position);

            if (mapInfo.Feature != null)
            {
                var layer = (WritableLayer)mapControl.Map.Layers.First(l => l.Name == "MesoLayer");
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(100009.715432, 41.513456),
                };
                feature.Styles.Add(
                    CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_5.png", 0.6));
                layer.Add(feature);
                MessageBox.Show("Yes");
            }
        }

        private void lvOpenDataElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMap();
        }

        private void lvMesocyclones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RefreshMap();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }

        // -------------------- MATH HELPERS --------------------
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



        /*


        private void MapControlOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            var position = args.GetPosition(mapControl).ToMapsui();
            var mapInfo = mapControl.GetMapInfo(position);

            if (mapInfo.Feature != null)
            {
                var layer = (WritableLayer) mapControl.Map.Layers.First(l => l.Name == "MesoLayer");
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(100009.715432, 41.513456),
                };
                feature.Styles.Add(
                    CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_5.png", 0.6));
                layer.Add(feature);
                MessageBox.Show("Yes");
            }
        }

        private static WritableLayer CreateEditLayer()
        {
            return new WritableLayer
            {
                Name = "EditLayer",
                Style = CreateEditLayerStyle(),
                IsMapInfoLayer = true
            };
        }

        private static StyleCollection CreateEditLayerStyle()
        {
            // The edit layer has two styles. That is why it needs to use a StyleCollection.
            // In a future version of Mapsui the ILayer will have a Styles collections just
            // as the IFeature has right now.
            // The first style is the basic style of the features in edit mode.
            // The second style is the way to show a feature is selected.
            return new StyleCollection
            {
                CreateEditLayerBasicStyle(),
                CreateSelectedStyle()
            };
        }

        private static IStyle CreateEditLayerBasicStyle()
        {
            // Note: VectorStyle does not function in the current release Mapsui version.
            // You need the package deployed from the build server.
            var editStyle = new VectorStyle
            {
                Fill = new Brush(EditModeColor),
                Line = new Pen(EditModeColor, 3),
                Outline = new Pen(EditModeColor, 3)
            };
            return editStyle;
        }

        private static readonly Color EditModeColor = new Color(124, 22, 111, 180);
        private static readonly Color PointLayerColor = new Color(240, 240, 240, 240);
        private static readonly Color LineLayerColor = new Color(150, 150, 150, 240);
        private static readonly Color PolygonLayerColor = new Color(20, 20, 20, 240);


        private static readonly SymbolStyle SelectedStyle = new SymbolStyle
        {
            Fill = null,
            Outline = new Pen(Color.Red, 3),
            Line = new Pen(Color.Red, 3)
        };
        private static readonly SymbolStyle DisableStyle = new SymbolStyle { Enabled = false };

        private static IStyle CreateSelectedStyle()
        {
            // The selected style use a ThemeStyle which takes a method to determing the style based
            // on the feature. In this case is checks it the "Selected" field is set to true.
            return new ThemeStyle(f => (bool?)f["Selected"] == true ? SelectedStyle : DisableStyle);
        }

        private static WritableLayer CreatePointLayer()
        {
            var pointLayer = new WritableLayer
            {
                Name = "MesoLayer",
                Style = CreatePointStyle(),
                IsMapInfoLayer = true
            };

            var feature = new Feature
            {
                Geometry = new Mapsui.Geometries.Point(9.715432, 41.513456)
            };
            feature.Styles.Add(CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_5.png", 0.6));

            pointLayer.Add(feature);
            return pointLayer;
        }

        

        private static WritableLayer CreateLineLayer()
        {
            var lineLayer = new WritableLayer
            {
                Name = "LineLayer",
                Style = CreateLineStyle()
            };

            // todo: add data

            return lineLayer;
        }

        private static WritableLayer CreatePolygonLayer()
        {
            var polygonLayer = new WritableLayer
            {
                Name = "PolygonLayer",
                Style = CreatePolygonStyle()
            };

            var wkt = "POLYGON ((1261416.17275404 5360656.05714234, 1261367.50386493 5360614.2556425, 1261353.47050427 5360599.62511755, 1261338.83997932 5360576.03712836, 1261337.34706862 5360570.6626498, 1261375.8641649 5360511.2448036, 1261383.92588273 5360483.17808227, 1261391.98760055 5360485.56673941, 1261393.48051126 5360480.490843, 1261411.99260405 5360487.6568144, 1261430.50469684 5360496.9128608, 1261450.21111819 5360507.06465361, 1261472.00761454 5360525.5767464, 1261488.13105019 5360544.98458561, 1261488.1310502 5360545.28316775, 1261481.26366093 5360549.76189988, 1261489.6239609 5360560.21227484, 1261495.59560374 5360555.13637843, 1261512.91336796 5360573.05130694, 1261535.00844645 5360598.43078898, 1261540.08434286 5360619.03295677, 1261535.90419287 5360621.12303176, 1261526.64814648 5360623.21310675, 1261489.32537876 5360644.41243881, 1261458.27283602 5360661.73020303, 1261438.26783253 5360662.02878517, 1261427.22029328 5360660.23729232, 1261416.17275404 5360656.05714234))";
            var polygon = GeometryFromWKT.Parse(wkt);
            polygonLayer.Add(new Feature { Geometry = polygon });

            return polygonLayer;
        }

        private static IStyle CreatePointStyle()
        {
            return new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(PointLayerColor),
                Line = new Mapsui.Styles.Pen(PointLayerColor, 3),
                Outline = new Pen(Color.Gray, 2)
            };
        }

        private static IStyle CreateLineStyle()
        {
            return new VectorStyle
            {
                Fill = new Brush(LineLayerColor),
                Line = new Pen(LineLayerColor, 3),
                Outline = new Pen(LineLayerColor, 3)
            };
        }
        private static IStyle CreatePolygonStyle()
        {
            return new VectorStyle
            {
                Fill = new Brush(new Color(PolygonLayerColor)),
                Line = new Pen(PolygonLayerColor, 3),
                Outline = new Pen(PolygonLayerColor, 3)
            };
        }

        private void RefreshMap()
        {
            if (MainViewModel.SelectedElement == null)
            {
                mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone>(),
                    null,
                    MainViewModel.CurrentMapConfiguration,
                    null);
                return;
            }

            if (MainViewModel.CurrentMapConfiguration.ShowHistoricMesocyclones)
            {
                OpenDataElement previousElement = OpenDataElement.GetPreviousOpenDataElement(
                    MainViewModel.OpenDataElements.ToList(),
                    MainViewModel.SelectedElement);

                if (previousElement == null)
                {
                    mapControl.Map = MapBuilder.CreateMap(MainViewModel.SelectedElement.Mesocyclones,
                        null,
                        MainViewModel.CurrentMapConfiguration,
                        MainViewModel.SelectedMesocyclone);
                }
                else
                {
                    mapControl.Map = MapBuilder.CreateMap(
                        MainViewModel.SelectedElement.Mesocyclones,
                        previousElement.Mesocyclones,
                        MainViewModel.CurrentMapConfiguration,
                        MainViewModel.SelectedMesocyclone);
                }
            }
            else
            {
                mapControl.Map = MapBuilder.CreateMap(MainViewModel.SelectedElement.Mesocyclones,
                    null,
                    MainViewModel.CurrentMapConfiguration,
                    MainViewModel.SelectedMesocyclone);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void lvOpenDataElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RefreshMap();
        }

        private void lvMesocyclones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RefreshMap();
        }
    */
        }
    }
