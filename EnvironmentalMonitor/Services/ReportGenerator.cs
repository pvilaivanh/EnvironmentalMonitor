using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EnvironmentalMonitor.Models;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Provides functionality to generate environmental reports based on indoor sensor data and outdoor weather data.
    /// </summary>
    /// <remarks>Reports are saved in a directory named "Reports", organized by date. Each report includes a
    /// human-readable text file and a JSON file containing sensor data and temperature changes. The generator maintains
    /// the last indoor sensor data to calculate temperature changes for subsequent reports.</remarks>
    public class ReportGenerator
    {
        private static readonly string ReportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Reports");

        private SensorData? _lastIndoorData;

        /// <summary>
        /// Initializes a new instance of the ReportGenerator class and ensures that the report directory exists by
        /// creating it if it does not.
        /// </summary>
        /// <remarks>This constructor checks for the existence of the specified report directory and
        /// creates it if necessary. Ensure that the application has the required permissions to create directories in
        /// the specified location.</remarks>
        public ReportGenerator()
        {
            if (!Directory.Exists(ReportDirectory))
            {
                Directory.CreateDirectory(ReportDirectory);
            }
        }

        /// <summary>
        /// Generates a report containing indoor and outdoor sensor data, saving the results in both human-readable and
        /// JSON formats.
        /// </summary>
        /// <remarks>The report is saved in a date-based directory under the configured report location.
        /// The method logs the report generation and updates the last recorded indoor data for future
        /// comparisons.</remarks>
        /// <param name="indoor">The indoor sensor data to include in the report. Must not be null.</param>
        /// <param name="outdoor">The outdoor weather data to include in the report. Must not be null.</param>
        public void GenerateReport(SensorData indoor, WeatherData outdoor)
        {
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            string fullPath = Path.Combine(ReportDirectory, dateFolder);

            if (!Directory.Exists(fullPath)) // Ensure the date-specific directory exists
            {
                Directory.CreateDirectory(fullPath);
            }

            string timestamp = DateTime.Now.ToString("HH-mm-ss");
            string readableFile = Path.Combine(fullPath, $"Report_{timestamp}.txt");
            string jsonFile = Path.Combine(fullPath, $"Report_{timestamp}.json");

            double indoorChange = 0;

            if (_lastIndoorData != null) // Check if there is previous indoor data
            {
                indoorChange = indoor.TemperatureF - _lastIndoorData.TemperatureF;
            }

            string readableReport = BuildReadableReport(indoor, outdoor, indoorChange);

            File.WriteAllText(readableFile, readableReport, Encoding.UTF8);

            var jsonReport = new // Anonymous type to hold the report data for JSON serialization
            {
                Created = DateTime.Now,
                Indoor = indoor,
                Outdoor = outdoor,
                IndoorTempChange = indoorChange
            };

            // Serialize the report to JSON with indentation for readability
            string jsonString = JsonSerializer.Serialize(jsonReport, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(jsonFile, jsonString, Encoding.UTF8);

            LoggerService.LogInfo("Report generated successfully.");

            _lastIndoorData = indoor;
        }

        /// <summary>
        /// Generates a formatted environmental report summarizing current indoor and outdoor conditions.
        /// </summary>
        /// <remarks>The report includes the current date and time of creation, and it is formatted for
        /// readability.</remarks>
        /// <param name="indoor">The indoor sensor data containing temperature and humidity information.</param>
        /// <param name="outdoor">The outdoor weather data including temperature, humidity, and a description of the weather conditions.</param>
        /// <param name="change">The change in indoor temperature since the last report, measured in degrees Fahrenheit.</param>
        /// <returns>A string containing the formatted environmental report with current indoor and outdoor conditions.</returns>
        private string BuildReadableReport(SensorData indoor, WeatherData outdoor, double change)
        {
            return $"""
                    Environmental Report
                    =====================
                    Created: {DateTime.Now}

                    Indoor Temperature (F): {indoor.TemperatureF:F2}
                    Indoor Humidity: {indoor.Humidity:F2}%

                    Outdoor Temperature (F): {outdoor.TemperatureF:F2}
                    Outdoor Humidity: {outdoor.Humidity}%
                    Condition: {outdoor.Description}

                    Indoor Temp Change Since Last Report: {change:F2} F
                    """;
        }
    }
}
