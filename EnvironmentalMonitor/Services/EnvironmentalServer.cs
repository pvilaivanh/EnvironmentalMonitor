using EnvironmentalMonitor.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EnvironmentalMonitor.Services;
using EnvironmentalMonitor.Models;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Represents a server that receives indoor environmental sensor data, retrieves current outdoor weather
    /// information, and generates reports based on both data sources.
    /// </summary>
    /// <remarks>The EnvironmentalServer listens for incoming UDP packets containing sensor data on a
    /// specified port. Upon receiving valid data, it fetches the latest outdoor weather conditions and generates a
    /// report using the provided ReportGenerator. The server operates continuously until manually stopped. This class
    /// is intended for scenarios where real-time monitoring and reporting of environmental conditions are
    /// required.</remarks>
    public class EnvironmentalServer
    {
        private readonly int _port;
        private readonly WeatherService _weatherService = new WeatherService();
        private readonly ReportGenerator _reportGenerator = new ReportGenerator();
        private SensorData? _latestIndoorData;

        public WeatherService WeatherService => _weatherService;
        public ReportGenerator ReportGenerator => _reportGenerator;
        public SensorData? GetLatestIndoorData() => _latestIndoorData;

        /// <summary>
        /// Initializes a new instance of the EnvironmentalServer class that listens on the specified port.
        /// </summary>
        /// <param name="port">The port number on which the server will listen for incoming connections. Must be a valid port number
        /// between 1 and 65535.</param>
        public EnvironmentalServer(int port)
        {
            _port = port;
        }

        /// <summary>
        /// Starts the environmental server and begins listening for incoming UDP packets containing sensor data.
        /// </summary>
        /// <remarks>The server runs continuously, processing incoming sensor data and generating reports
        /// when valid data is received. If invalid data is detected or outdoor weather information is unavailable,
        /// appropriate warnings are logged. This method is intended to be run as a long-lived background
        /// operation.</remarks>
        /// <returns>A task that represents the asynchronous operation of the server. The task completes only if the server is
        /// stopped or an unhandled exception occurs.</returns>
        public async Task Start()
        {
            UdpClient udpServer = new UdpClient(_port); // Listen on the specified port
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // Accept connections from any IP address

            Console.WriteLine($"Starting Environmental Server on port {_port}...");
            LoggerService.LogInfo($"Environmental Server started on port {_port}.");

            // Continuously listen for incoming UDP packets
            while (true)
            {
                // Wait for a UDP packet to be received
                try
                {
                    byte[] data = udpServer.Receive(ref remoteEndPoint); // Receive data from the remote endpoint
                    string json = Encoding.UTF8.GetString(data); // Convert the received byte array to a string using UTF-8 encoding

                    Console.WriteLine($"Received: {json})");
                    LoggerService.LogInfo($"Received data from {remoteEndPoint.Address}: {json}");

                    var sensorData = MessageParser.Parse(json);

                    if (sensorData != null) // Check if the parsed sensor data is valid
                    {
                        Console.WriteLine("Valid indoor data received.");
                        LoggerService.LogInfo("Valid sensor data processed.");

                        var outdoorData = await _weatherService.GetWeatherAsync(); // Fetch the latest outdoor weather data asynchronously

                        if (outdoorData != null) // Check if outdoor weather data was successfully retrieved
                        {
                            _latestIndoorData = sensorData;
                            _reportGenerator.GenerateReport(sensorData, outdoorData);
                        }
                        else
                        {
                            LoggerService.LogWarn("Outdoor weather unavailable. Report not generated.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid sensor data received.");
                        LoggerService.LogWarn("Invalid sensor data detected.");
                    }
                }
                // Catch any exceptions that occur during the server's operation and log them
                catch (Exception ex)
                {
                    Console.WriteLine($"Server error: {ex.Message}");
                    LoggerService.LogError($"Server exception: {ex.Message}");
                }

            }
        }
    }
}
