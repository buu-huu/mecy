using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.UI.Wpf;
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

namespace MecyApplication
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            RefreshMap();

            /* Event Subscriptions */
            SetUpEvents();
        }

        private void SetUpEvents()
        {
            MainViewModel.RefreshMapEvent += RefreshMap;
            mapControl.MouseLeftButtonDown += MapControlOnMouseLeftButtonDown;
        }

        private void HandleMouseDown(Mapsui.Geometries.Point screenPosition)
        {
            if (mapControl.GetMapInfo(screenPosition).Feature != null)
            {
                MessageBox.Show(mapControl.GetMapInfo(screenPosition).Feature.ToString());
            }
        }

        private void MapControlOnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            HandleMouseDown(args.GetPosition(mapControl).ToMapsui());
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
            RefreshMap();
        }

        private void lvMesocyclones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMap();
        }
    }
}
