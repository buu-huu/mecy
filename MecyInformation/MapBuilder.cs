﻿using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MecyInformation
{
    public static class MapBuilder
    {
        private const string GOOGLE_MAPS_TILE_URL = "http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}";
        private static TileSource _selectedTileSource = TileSource.OpenStreetMap;

        public static TileSource SelectedTileSource { get => _selectedTileSource; set => _selectedTileSource = value; }

        public enum TileSource
        {
            OpenStreetMap,
            GoogleMaps
        }

        public static Map CreateMap(List<Mesocyclone> mesocyclones)
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

            if (SelectedTileSource == TileSource.OpenStreetMap)
            {
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
            }
            else if (SelectedTileSource == TileSource.GoogleMaps)
            {
                map.Layers.Add(new TileLayer(CreateGoogleTileSource(GOOGLE_MAPS_TILE_URL)));
            }
            map.Layers.Add(CreateMesoLayer(mesocyclones));
            return map;
        }

        private static Layer CreateMesoLayer(List<Mesocyclone> mesocyclones)
        {
            var features = new Features();

            foreach (var meso in mesocyclones)
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
    }
}
