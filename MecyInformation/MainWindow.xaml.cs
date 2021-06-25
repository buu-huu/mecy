using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinSCP;

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        Mesocyclone activeMeso;
        List<OpenDataElement> openDataElements;

        OpenDataElement selectedElement;
        bool isDownloading = false;
        */

        MainViewModel mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            this.mainViewModel = mainViewModel;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void lvOpenDataElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvOpenDataElements.SelectedItem != null)
            {
                var selectedElement = (OpenDataElement)lvOpenDataElements.SelectedItem;
                mapControl.Map = MapBuilder.CreateMap(selectedElement.Mesocyclones);
            }
            else
            {
                mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone>());
            }
        }

        /*
        private void lvMesos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ResetStationAvailability()
        {
            lblRadarAsb.Background = new SolidColorBrush();
            lblRadarBoo.Background = new SolidColorBrush();
            lblRadarDrs.Background = new SolidColorBrush();
            lblRadarEis.Background = new SolidColorBrush();
            lblRadarEss.Background = new SolidColorBrush();
            lblRadarFbg.Background = new SolidColorBrush();
            lblRadarFld.Background = new SolidColorBrush();
            lblRadarHnr.Background = new SolidColorBrush();
            lblRadarIsn.Background = new SolidColorBrush();
            lblRadarMem.Background = new SolidColorBrush();
            lblRadarNeu.Background = new SolidColorBrush();
            lblRadarNhb.Background = new SolidColorBrush();
            lblRadarOft.Background = new SolidColorBrush();
            lblRadarPro.Background = new SolidColorBrush();
            lblRadarRos.Background = new SolidColorBrush();
            lblRadarTur.Background = new SolidColorBrush();
            lblRadarUmd.Background = new SolidColorBrush();
        }

        private void RefreshStationAvailability()
        {
            foreach (RadarStation station in selectedElement.RadarStations)
            {
                SolidColorBrush brushGreen = (SolidColorBrush)new BrushConverter().ConvertFrom("#40ff69");
                SolidColorBrush brushRed = (SolidColorBrush)new BrushConverter().ConvertFrom("#f23333");
                switch (station.Name)
                {
                    case "ASB":
                        if (station.IsAvailable)
                        {
                            lblRadarAsb.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarAsb.Background = brushRed;
                        }
                        break;
                    case "BOO":
                        if (station.IsAvailable)
                        {
                            lblRadarBoo.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarBoo.Background = brushRed;
                        }
                        break;
                    case "DRS":
                        if (station.IsAvailable)
                        {
                            lblRadarDrs.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarDrs.Background = brushRed;
                        }
                        break;
                    case "EIS":
                        if (station.IsAvailable)
                        {
                            lblRadarEis.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarEis.Background = brushRed;
                        }
                        break;
                    case "ESS":
                        if (station.IsAvailable)
                        {
                            lblRadarEss.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarEss.Background = brushRed;
                        }
                        break;
                    case "FBG":
                        if (station.IsAvailable)
                        {
                            lblRadarFbg.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarFbg.Background = brushRed;
                        }
                        break;
                    case "FLD":
                        if (station.IsAvailable)
                        {
                            lblRadarFld.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarFld.Background = brushRed;
                        }
                        break;
                    case "HNR":
                        if (station.IsAvailable)
                        {
                            lblRadarHnr.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarHnr.Background = brushRed;
                        }
                        break;
                    case "ISN":
                        if (station.IsAvailable)
                        {
                            lblRadarIsn.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarIsn.Background = brushRed;
                        }
                        break;
                    case "MEM":
                        if (station.IsAvailable)
                        {
                            lblRadarMem.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarMem.Background = brushRed;
                        }
                        break;
                    case "NEU":
                        if (station.IsAvailable)
                        {
                            lblRadarNeu.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarNeu.Background = brushRed;
                        }
                        break;
                    case "NHB":
                        if (station.IsAvailable)
                        {
                            lblRadarNhb.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarNhb.Background = brushRed;
                        }
                        break;
                    case "OFT":
                        if (station.IsAvailable)
                        {
                            lblRadarOft.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarOft.Background = brushRed;
                        }
                        break;
                    case "PRO":
                        if (station.IsAvailable)
                        {
                            lblRadarPro.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarPro.Background = brushRed;
                        }
                        break;
                    case "ROS":
                        if (station.IsAvailable)
                        {
                            lblRadarRos.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarRos.Background = brushRed;
                        }
                        break;
                    case "TUR":
                        if (station.IsAvailable)
                        {
                            lblRadarTur.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarTur.Background = brushRed;
                        }
                        break;
                    case "UMD":
                        if (station.IsAvailable)
                        {
                            lblRadarUmd.Background = brushGreen;
                        }
                        else
                        {
                            lblRadarUmd.Background = brushRed;
                        }
                        break;
                }
            }
        }

        private void RefreshView()
        {
            openDataElements = XMLParser.ParseAllMesos(OpenDataDownloader.LOCAL_DOWNLOAD_PATH);
            lvTimes.ItemsSource = openDataElements;
            gridDetails.DataContext = activeMeso;
            gridRadarAvailability.DataContext = selectedElement;
        }

        private void ReDownloadData()
        {
            isDownloading = true;
            OpenDataDownloader.DownloadAllData();
            isDownloading = false;
        }

        private void SetUpClocks()
        {
            DispatcherTimer clockUtc = new DispatcherTimer();
            clockUtc.Interval = TimeSpan.FromSeconds(1);
            clockUtc.Tick += clockUtcTick;
            clockUtc.Start();

            DispatcherTimer clockLocal = new DispatcherTimer();
            clockLocal.Interval = TimeSpan.FromSeconds(1);
            clockLocal.Tick += clockLocalTick;
            clockLocal.Start();
        }

        private void clockUtcTick(object sender, EventArgs e)
        {
            lblClockUtc.Content = "UTC: " + DateTime.UtcNow.ToLongTimeString();
        }

        private void clockLocalTick(object sender, EventArgs e)
        {
            lblClockLocal.Content = "LOC: " + DateTime.Now.ToLongTimeString();
        }

        private void UpdateDetailsPanel()
        {
            activeMeso = (Mesocyclone)lvMesos.SelectedItem;
            gridDetails.DataContext = activeMeso;

            if (activeMeso != null)
            {
                lvElevations.ItemsSource = activeMeso.Elevations;
            }
            else
            {
                lvElevations.ItemsSource = new List<double>();
            }
        }

        private void lvMesos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDetailsPanel();
        }

        private void lvMesos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvMesos.SelectedItem != null)
            {
                new MapWindow(activeMeso).Show();
            }
        }

        private void lvMesos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult res = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (res.VisualHit.GetType() != typeof(ListBoxItem))
            {
                lvMesos.UnselectAll();
            }
        }

        private void lvTimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTimes.SelectedItem != null)
            {
                selectedElement = (OpenDataElement)lvTimes.SelectedItem;
                lvMesos.ItemsSource = selectedElement.Mesocyclones;
                RefreshStationAvailability();

                mapControl.Map = CreateMap(selectedElement);
            }
            else
            {
                ResetStationAvailability();
                mapControl.Map = CreateMap(null);
            }
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void tlbButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!isDownloading)
            {
                DownloadWindow downloadWindow = new DownloadWindow();

                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _ = Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate {
                        downloadWindow.Show();
                    });
                    ReDownloadData();
                }).ContinueWith(task =>
                {
                    RefreshView();
                    downloadWindow.Close();
                }, scheduler);
            }
        }

        public Map CreateMap(OpenDataElement element)
        {
            if (element == null)
            {
                var map = new Map
                {
                    Transformation = new MinimalTransformation(),
                    CRS = "EPSG:3857",
                    BackColor = Mapsui.Styles.Color.Gray
                };
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
                return map;
            }
            else
            {
                var map = new Map
                {
                    Transformation = new MinimalTransformation(),
                    CRS = "EPSG:3857",
                    BackColor = Mapsui.Styles.Color.Gray
                };
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
                map.Layers.Add(CreateMesoLayer(element));
                return map;
            }
        }

        private Layer CreateMesoLayer(OpenDataElement openDataElement)
        {
            var features = new Features();

            foreach (var meso in openDataElement.Mesocyclones)
            {
                var mesoFeature = new Feature
                {
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
        */
    }
}
