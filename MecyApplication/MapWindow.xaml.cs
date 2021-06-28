﻿using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MecyApplication
{
    /// <summary>
    /// Interaktionslogik für MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow(Mesocyclone meso)
        {
            InitializeComponent();
            
            mapControl.Map = MapBuilder.CreateMap(new List<Mesocyclone> { meso }, null, MapConfiguration.Instance, null);
            gridInformation.DataContext = meso;
        }
    }
}
