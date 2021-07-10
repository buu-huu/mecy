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
    /// Interaktionslogik für LiveWindow.xaml
    /// </summary>
    public partial class LiveWindow : Window
    {
        private const double MESO_ICON_SCALE = 0.6;

        private const string NAME_MESO_LAYER = "MesoLayer";
        private const string NAME_MESO_HIST_LAYER = "MesoHistLayer";
        private const string NAME_MESO_DIAMETER_LAYER = "MesoDiameterLayer";
        private const string NAME_MESO_LABEL_LAYER = "MesoLabelLayer";
        private const string NAME_RADAR_DIAMETER_LAYER = "RadarDiameterLayer";
        private const string NAME_RADAR_LABEL_LAYER = "RadarLabelLayer";
        private const string NAME_MEASURING_LAYER = "MeasuringLayer";

        private const double CENTER_LONGITUDE = 10.160549;
        private const double CENTER_LATITUDE = 51.024813;
        private const double RADIUS_EQUATOR = 6378.1;
        private const string GOOGLE_MAPS_TILE_URL = "http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}";

        private bool _isMeasuringDistance = false;
        private double _measuringStartLongitude = 0;
        private double _measuringStartLatitude = 0;

        private LiveViewModel _liveViewModel;

        public LiveViewModel LiveViewModel
        {
            get
            {
                return _liveViewModel;
            }
            set
            {
                _liveViewModel = value;
            }
        }

        /// <summary>
        /// Constructor, that holds an instance of the MainViewModel.
        /// </summary>
        /// <param name="mainViewModel">MainViewModel of the application.</param>
        public LiveWindow(LiveViewModel liveViewModel)
        {
            InitializeComponent();

            this.LiveViewModel = liveViewModel;
            this.DataContext = LiveViewModel;

            mapControl.Map = CreateMap();
            RefreshMap();

            // Event subscriptions
            LiveViewModel.RefreshMapEvent += RefreshMap;
            LiveViewModel.RefreshMapWidgetsEvent += RefreshMapWithWidgets;
            LiveViewModel.CenterMapEvent += CenterMap;
            LiveViewModel.CenterMapToMesoEvent += CenterMapToMeso;
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
                BackColor = Color.Gray,
                Limiter = new ViewportLimiterKeepWithin()
                {
                    PanLimits = new BoundingBox(FromLongLat(-170, 86), FromLongLat(180, -86))
                }
            };

            // Tile Source
            if (LiveViewModel.CurrentMapConfiguration.ActiveTileSource == MapConfiguration.TileSource.OpenStreetMap) map.Layers.Add(OpenStreetMap.CreateTileLayer());
            else if (LiveViewModel.CurrentMapConfiguration.ActiveTileSource == MapConfiguration.TileSource.GoogleMaps) map.Layers.Add(new TileLayer(CreateGoogleTileSource(GOOGLE_MAPS_TILE_URL)));

            // Layers
            map.Layers.Add(CreateRadarDiameterLayer());
            map.Layers.Add(CreateRadarLabelLayer());
            map.Layers.Add(CreateMesoDiameterLayer());
            map.Layers.Add(CreateMesoHistLayer());
            map.Layers.Add(CreateMesoLabelLayer());
            map.Layers.Add(CreateMesoLayer());
            map.Layers.Add(CreateMeasuringLayer());

            // Widgets
            if (LiveViewModel.CurrentMapConfiguration.ShowScaleBar) map.Widgets.Add(new ScaleBarWidget(map));
            if (LiveViewModel.CurrentMapConfiguration.ShowZoomWidget) map.Widgets.Add(new ZoomInOutWidget());

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
            if (LiveViewModel.SelectedElement == null) return;

            // Layers
            if (LiveViewModel.CurrentMapConfiguration.ShowRadarLabels) DrawRadarLabelsToLayer();
            if (LiveViewModel.CurrentMapConfiguration.ShowRadarDiameters) DrawRadarDiametersToMap();
            if (LiveViewModel.CurrentMapConfiguration.ShowMesocycloneIdLabel) DrawMesoLabelsToLayer();
            if (LiveViewModel.CurrentMapConfiguration.ShowMesocycloneDiameter) DrawMesoDiametersToLayer();
            //if (LiveViewModel.CurrentMapConfiguration.ShowHistoricMesocyclones) DrawMesosHistToLayer();
            DrawMesosToLayer();
        }

        /// <summary>
        /// Refreshes the map and the widgets.
        /// </summary>
        private void RefreshMapWithWidgets()
        {
            mapControl.Map = CreateMap();
            RefreshMap();
        }

        /// <summary>
        /// Clears the map.
        /// </summary>
        private void ClearMap()
        {
            var layerMeso = (MemoryLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_LAYER);
            var layerMesoDiameter = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_DIAMETER_LAYER);
            var layerMesoHist = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_HIST_LAYER);
            var layerMesoLabel = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_LABEL_LAYER);
            var layerRadarDiameter = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_RADAR_DIAMETER_LAYER);
            var layerRadarLabel = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_RADAR_LABEL_LAYER);

            layerMeso.DataSource = null;
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
            var selectedMeso = LiveViewModel.SelectedMesocyclone;
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
        private MemoryLayer CreateMesoLayer()
        {
            var layer = new MemoryLayer
            {
                Name = NAME_MESO_LAYER,
                Style = null,
                IsMapInfoLayer = true
            };
            return layer;
        }

        /// <summary>
        /// Creates a mesocyclone feature
        /// and loads the correct image file.
        /// </summary>
        /// <param name="meso">Mesocyclone</param>
        /// <returns>Mesocyclone feature</returns>
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
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_1.png", MESO_ICON_SCALE);
                    break;
                case 2:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_2.png", MESO_ICON_SCALE);
                    break;
                case 3:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_3.png", MESO_ICON_SCALE);
                    break;
                case 4:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_4.png", MESO_ICON_SCALE);
                    break;
                case 5:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_5.png", MESO_ICON_SCALE);
                    break;
            }
            feature.Styles.Add(style);
            return feature;
        }

        /// <summary>
        /// Creates a highlightes mesocyclone feature.
        /// </summary>
        /// <param name="meso"></param>
        /// <returns>Selected mesocyclone feature</returns>
        private Feature CreateMesoFeatureSelected(Mesocyclone meso)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(meso.Longitude, meso.Latitude)
            };
            var style = new SymbolStyle();
            switch (meso.Intensity)
            {
                case 1:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_1.png", MESO_ICON_SCALE);
                    break;
                case 2:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_2.png", MESO_ICON_SCALE);
                    break;
                case 3:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_3.png", MESO_ICON_SCALE);
                    break;
                case 4:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_4.png", MESO_ICON_SCALE);
                    break;
                case 5:
                    style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_selected_5.png", MESO_ICON_SCALE);
                    break;
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
            var layer = (MemoryLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_LAYER);
            mapControl.RefreshGraphics();
            if (LiveViewModel.SelectedElement == null || LiveViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }

            var features = new Features();
            var selectedMeso = LiveViewModel.SelectedMesocyclone;
            foreach (var meso in LiveViewModel.SelectedElement.Mesocyclones)
            {
                if (selectedMeso != meso) features.Add(CreateMesoFeature(meso));
            }
            if (selectedMeso != null) features.Add(CreateMesoFeatureSelected(selectedMeso));
            layer.DataSource = new MemoryProvider(features);
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
                Name = NAME_MESO_DIAMETER_LAYER,
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
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_DIAMETER_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();
            if (LiveViewModel.SelectedElement == null || LiveViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }
            var style = new VectorStyle
            {
                Fill = new Brush(new Color(255, 97, 247, 70)),
                Outline = new Pen
                {
                    Color = new Color(121, 97, 255, 220),
                    Width = 2,
                    PenStyle = PenStyle.LongDashDot,
                    PenStrokeCap = PenStrokeCap.Butt
                }
            };
            foreach (var meso in LiveViewModel.SelectedElement.Mesocyclones)
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
        private WritableLayer CreateMesoLabelLayer()
        {
            var layer = new WritableLayer
            {
                Name = NAME_MESO_LABEL_LAYER,
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
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_LABEL_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();
            if (LiveViewModel.SelectedElement == null || LiveViewModel.SelectedElement.Mesocyclones == null)
            {
                return;
            }

            foreach (var meso in LiveViewModel.SelectedElement.Mesocyclones)
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
                Name = NAME_MESO_HIST_LAYER,
                Style = null
            };
            return layer;
        }

        /// <summary>
        /// Creates a historic mesocyclone feature.
        /// </summary>
        /// <param name="meso">Historic mesocyclone</param>
        /// <returns>Historic mesocyclone feature</returns>
        //private Feature CreateMesoHistFeature(Mesocyclone meso)
        //{
        //    var feature = new Feature
        //    {
        //        Geometry = FromLongLat(meso.Longitude, meso.Latitude)
        //    };
        //    var style = new SymbolStyle();
        //    switch (meso.Intensity)
        //    {
        //        case 1:
        //            style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_1.png", 0.6);
        //            break;
        //        case 2:
        //            style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_2.png", 0.6);
        //            break;
        //        case 3:
        //            style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_3.png", 0.6);
        //            break;
        //        case 4:
        //            style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_4.png", 0.6);
        //            break;
        //        case 5:
        //            style = CreatePngStyle("MecyApplication.Resources.meso_icon_map_hist_5.png", 0.6);
        //            break;
        //    }
        //    if (LiveViewModel.CurrentMapConfiguration.HistoricMesocyclonesTransparent)
        //    {
        //        style.Opacity = LiveViewModel.CurrentMapConfiguration.HistoricMesocyclonesOpacity;
        //    }
        //    feature.Styles.Add(style);
        //    return feature;
        //}

        /// <summary>
        /// Draws historic mesocyclones to layer.
        /// </summary>
        //private void DrawMesosHistToLayer()
        //{
        //    var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MESO_HIST_LAYER);
        //    layer.Clear();
        //    mapControl.RefreshGraphics();
        //    if (LiveViewModel.SelectedElement == null || LiveViewModel.SelectedElement.Mesocyclones == null)
        //    {
        //        return;
        //    }

        //    OpenDataElement previousElement = LiveViewModel.PreviousElement;

        //    if (previousElement == null) return;
        //    foreach (var meso in previousElement.Mesocyclones)
        //    {
        //        layer.Add(CreateMesoHistFeature(meso));
        //    }
        //    mapControl.RefreshGraphics();
        //}

        // -------------------- RADAR LABEL LAYER --------------------
        /// <summary>
        /// Creates a radar label layer.
        /// </summary>
        /// <returns>Radar label layer</returns>
        private WritableLayer CreateRadarLabelLayer()
        {
            var layer = new WritableLayer
            {
                Name = NAME_RADAR_LABEL_LAYER,
                Style = null,
                IsMapInfoLayer = true
            };
            return layer;
        }

        /// <summary>
        /// Creates a radar label feature.
        /// </summary>
        /// <param name="station">Radar station</param>
        /// <returns>Radar label feature</returns>
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

        /// <summary>
        /// Draws the radar labels to the layer.
        /// </summary>
        private void DrawRadarLabelsToLayer()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_RADAR_LABEL_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();
            if (LiveViewModel.SelectedElement == null)
            {
                return;
            }

            foreach (var station in LiveViewModel.SelectedElement.RadarStations)
            {
                layer.Add(CreateRadarLabelFeature(station));
            }
            mapControl.RefreshGraphics();
        }

        // -------------------- RADAR DIAMETER LAYER --------------------
        /// <summary>
        /// Creates a radar diameter layer.
        /// </summary>
        /// <returns>Radar diameter layer</returns>
        private WritableLayer CreateRadarDiameterLayer()
        {
            var layer = new WritableLayer
            {
                Name = NAME_RADAR_DIAMETER_LAYER,
                Style = null
            };
            return layer;
        }

        /// <summary>
        /// Creates a radar diameter polygon.
        /// </summary>
        /// <param name="radar">Radar station</param>
        /// <returns>Radar diameter polygon</returns>
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

        /// <summary>
        /// Draws the radar diameters to the map.
        /// </summary>
        private void DrawRadarDiametersToMap()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_RADAR_DIAMETER_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();
            if (LiveViewModel.SelectedElement == null)
            {
                return;
            }

            foreach (var radar in LiveViewModel.SelectedElement.RadarStations)
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

        // -------------------- MEASURING --------------------
        /// <summary>
        /// Creates a radar diameter layer.
        /// </summary>
        /// <returns>Radar diameter layer</returns>
        private WritableLayer CreateMeasuringLayer()
        {
            var layer = new WritableLayer
            {
                Name = NAME_MEASURING_LAYER,
                Style = null
            };
            return layer;
        }

        private Feature CreateMeasuringLabel(double longitude, double latitude)
        {
            var feature = new Feature
            {
                Geometry = FromLongLat(longitude, latitude)
            };
            LabelStyle style = new LabelStyle
            {
                Offset = new Offset(0.0, 0.0, true),
                Text = String.Format("Lon: {0} Lat: {1}", Math.Round(longitude, 6), Math.Round(latitude, 6)),
                BackColor = new Brush(Color.Gray)
            };
            feature.Styles.Add(style);
            return feature;
        }

        /// <summary>
        /// Draws the radar diameters to the map.
        /// </summary>
        private void DrawMeasuringLabel()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MEASURING_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();

            layer.Add(CreateMeasuringLabel(_measuringStartLongitude, _measuringStartLatitude));
            mapControl.RefreshGraphics();
        }

        /// <summary>
        /// Resets the necessary variables and layer of measuring.
        /// </summary>
        private void ResetMeasuring()
        {
            var layer = (WritableLayer)mapControl.Map.Layers.First(i => i.Name == NAME_MEASURING_LAYER);
            layer.Clear();
            mapControl.RefreshGraphics();
            _measuringStartLongitude = 0;
            _measuringStartLatitude = 0;
            _isMeasuringDistance = false;
            LiveViewModel.CurrentMapConfiguration.CurrentlyMeasuringDistance = false;
        }

        // -------------------- EVENT HANDLING --------------------
        /// <summary>
        /// Remove the toolbar overflow here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        /// <summary>
        /// Handles the click on the map.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        public void HandleMapClick(object sender, MapInfoEventArgs args)
        {
            var mapInfo = args.MapInfo;
            Point pos = mapControl.Viewport.ScreenToWorld(mapInfo.ScreenPosition);
            Feature feature = (Feature)mapInfo.Feature;

            // Select meso
            var meso = GetBestMesocyclone(feature);
            if (meso != null)
            {
                LiveViewModel.SelectedMesocyclone = meso;
                ResetMeasuring();
                RefreshMap();
                return;
            }

            // If no meso found, let's have a look, if there is a radar station instead...
            var station = GetBestRadarStation(feature);
            if (station != null && args.NumTaps == 2) // Only open details window if double clicked
            {
                new RadarStationDetailsWindow(station).Show();
                ResetMeasuring();
                RefreshMap();
                return;
            }

            // No feature clicked, okay, then deselect selected meso...
            LiveViewModel.SelectedMesocyclone = null;
            RefreshMap();

            // Handle measuring if no feature found
            if (LiveViewModel.CurrentMapConfiguration.CurrentlyMeasuringDistance == false) return;
            if (LiveViewModel.SelectedElement == null)
            {
                LiveViewModel.CurrentMapConfiguration.CurrentlyMeasuringDistance = false;
                MessageBox.Show("Please select an element before measuring.");
                return;
            }
            var coordinates = SphericalMercator.ToLonLat(pos.X, pos.Y);

            if (!_isMeasuringDistance)
            {
                _isMeasuringDistance = true;
                _measuringStartLongitude = coordinates.X;
                _measuringStartLatitude = coordinates.Y;
                DrawMeasuringLabel();
            }
            else
            {
                _isMeasuringDistance = false;
                var measuringEndLongitude = coordinates.X;
                var measuringEndLatitude = coordinates.Y;
                var distance = GetDistanceBetweenCoordinates(
                    _measuringStartLongitude, measuringEndLongitude, _measuringStartLatitude, measuringEndLatitude);
                new DistanceDetailsWindow(_measuringStartLongitude, _measuringStartLatitude, measuringEndLongitude, measuringEndLatitude, distance).Show();

                ResetMeasuring();
                _measuringStartLongitude = 0;
                _measuringStartLatitude = 0;
                RefreshMap();
            }
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
            if (LiveViewModel.SelectedElement.Mesocyclones.Count == 0) return null;

            Point point = (Point)feature.Geometry;
            Point pos = SphericalMercator.ToLonLat(point.X, point.Y);
            double longitude = Math.Round(pos.X, 6, MidpointRounding.AwayFromZero);
            double latitude = Math.Round(pos.Y, 6, MidpointRounding.AwayFromZero);

            Mesocyclone bestMeso = LiveViewModel.SelectedElement.Mesocyclones[0];
            double diffBest = 1000;

            // Just remember the meso in bestMeso, if the coordinate distances are smaller than the
            // previous ones
            foreach (var meso in LiveViewModel.SelectedElement.Mesocyclones)
            {
                double longMeso = Math.Round(meso.Longitude, 6, MidpointRounding.AwayFromZero);
                double latMeso = Math.Round(meso.Latitude, 6, MidpointRounding.AwayFromZero);

                double diffAbsoluteLongitude = Math.Abs(longMeso - longitude);
                double diffAbsoluteLatitude = Math.Abs(latMeso - latitude);
                if ((diffAbsoluteLatitude + diffAbsoluteLatitude) < diffBest)
                {
                    diffBest = diffAbsoluteLongitude + diffAbsoluteLatitude;
                    bestMeso = meso;
                }
            }
            if (diffBest > 0.00001) return null;    // If difference is too big, return with null
            else return bestMeso;
        }

        /// <summary>
        /// Returns the nearest radar station instance to a feature.
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>Nearest radar station</returns>
        private RadarStation GetBestRadarStation(Feature feature)
        {
            if (feature == null) return null;

            Point point = (Point)feature.Geometry;
            Point pos = SphericalMercator.ToLonLat(point.X, point.Y);
            double longitude = Math.Round(pos.X, 6, MidpointRounding.AwayFromZero);
            double latitude = Math.Round(pos.Y, 6, MidpointRounding.AwayFromZero);

            RadarStation bestStation = LiveViewModel.SelectedElement.RadarStations[0];
            double diffBest = 1000; // Random high number

            foreach (var station in LiveViewModel.SelectedElement.RadarStations)
            {
                double longMeso = Math.Round(station.Longitude, 6, MidpointRounding.AwayFromZero);
                double latMeso = Math.Round(station.Latitude, 6, MidpointRounding.AwayFromZero);

                double diffAbsoluteLongitude = Math.Abs(longMeso - longitude);
                double diffAbsoluteLatitude = Math.Abs(latMeso - latitude);
                if ((diffAbsoluteLatitude + diffAbsoluteLatitude) < diffBest)
                {
                    diffBest = diffAbsoluteLongitude + diffAbsoluteLatitude;
                    bestStation = station;
                }
            }
            if (diffBest > 0.00001) return null;
            else return bestStation;
        }

        /// <summary>
        /// Returns the distance between two coordinates in km.
        /// </summary>
        /// <param name="lon1">Longitude point 1</param>
        /// <param name="lon2">Longitude point 2</param>
        /// <param name="lat1">Latitude point 1</param>
        /// <param name="lat2">Latitude point 2</param>
        /// <returns>Distance in km</returns>
        private double GetDistanceBetweenCoordinates(double lon1, double lon2, double lat1, double lat2)
        {
            var distanceRadiansLat = ConvertDegreesToRadians(lat2 - lat1);
            var distanceRadiansLon = ConvertDegreesToRadians(lon2 - lon1);

            lat1 = ConvertDegreesToRadians(lat1);
            lat2 = ConvertDegreesToRadians(lat2);

            var a = Math.Sin(distanceRadiansLat / 2) * Math.Sin(distanceRadiansLat / 2) +
                Math.Sin(distanceRadiansLon / 2) * Math.Sin(distanceRadiansLon / 2) *
                Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return RADIUS_EQUATOR * c;
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
