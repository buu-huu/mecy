﻿using NmeaParser;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MecyApplication
{
    public class Gps
    {
        public double CurrentLat { get; set; }
        public double CurrentLon { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOpen { get; set; }
        public SerialPortDevice Device { get; set; }
        public Gps()
        {
            ResetGps();

            DispatcherTimer availitilityTimer = new DispatcherTimer();
            availitilityTimer.Interval = TimeSpan.FromSeconds(5);
            availitilityTimer.Tick += AvailabilityCheckerTick;
            availitilityTimer.Start();
        }

        private void ResetGps()
        {
            string portname = "COM4";
            int baudrate = 9600;
            var port = new SerialPort(portname, baudrate);
            Device = new SerialPortDevice(port);
            Device.MessageReceived += device_NmeaMessageReceived;
            Device.OpenAsync();
        }

        private void AvailabilityCheckerTick(object sender, EventArgs e)
        {
            if (Device.IsOpen) IsOpen = true;
            else
            {
                IsOpen = false;
                IsAvailable = false;
                ResetGps();
            }
        }

        private void device_NmeaMessageReceived(object sender, NmeaMessageReceivedEventArgs args)
        {
            // called when a message is received
            if (args.Message is NmeaParser.Messages.Rmc rmc)
            {
                if (Double.IsNaN(rmc.Longitude) || Double.IsNaN(rmc.Latitude))
                {
                    IsAvailable = false;
                }
                else
                {
                    IsAvailable = true;
                    CurrentLon = rmc.Longitude;
                    CurrentLat = rmc.Latitude;
                }
            }
        }
    }
}
