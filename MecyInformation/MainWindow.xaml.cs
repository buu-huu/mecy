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
            mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone>());
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void lvOpenDataElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainViewModel.SelectedElement != null)
            {
                var selectedElement = MainViewModel.SelectedElement;
                mapControl.Map = MapBuilder.CreateMap(selectedElement.Mesocyclones);
            }
            else
            {
                mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone>());
            }
        }
    }
}
