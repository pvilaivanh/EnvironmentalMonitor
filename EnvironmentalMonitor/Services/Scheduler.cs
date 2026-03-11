using System;
using System.Collections.Generic;
using System.Text;
using EnvironmentalMonitor.Models;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Schedules and generates weather reports at specified intervals using indoor and outdoor sensor data.
    /// </summary>
    /// <remarks>The scheduler runs indefinitely, generating reports every specified interval. It retrieves
    /// indoor data through a provided function and outdoor weather data from a weather service. If either data source
    /// is unavailable, it logs a warning and skips report generation for that interval.</remarks>
    public class Scheduler
    {
        private readonly int _intervalMinutes;
        private readonly WeatherService _weatherService;
        private readonly ReportGenerator _reportGenerator;
        private Func<SensorData?> _getIndoorData;

        /// <summary>
        /// Initializes a new instance of the Scheduler class to execute scheduled tasks at a specified interval using
        /// provided services and sensor data.
        /// </summary>
        /// <remarks>The scheduler uses the specified interval and provided services to perform its
        /// operations. Ensure that all parameters are valid and not null to avoid runtime exceptions.</remarks>
        /// <param name="intervalMinutes">The interval, in minutes, at which scheduled tasks are executed. Must be a positive integer.</param>
        /// <param name="weatherService">An instance of WeatherService used to retrieve weather data for scheduling decisions.</param>
        /// <param name="reportGenerator">An instance of ReportGenerator responsible for generating reports based on scheduled tasks.</param>
        /// <param name="getIndoorData">A function that retrieves indoor sensor data. May return null if no data is available.</param>
        public Scheduler(int intervalMinutes, WeatherService weatherService, ReportGenerator reportGenerator, Func<SensorData?> getIndoorData)
        {
            _intervalMinutes = intervalMinutes;
            _weatherService = weatherService;
            _reportGenerator = reportGenerator;
            _getIndoorData = getIndoorData;
		}

        /// <summary>
        /// Starts the report generation scheduler, which generates reports at specified intervals based on indoor and
        /// outdoor data.
        /// </summary>
        /// <remarks>The scheduler runs continuously, generating reports every configured interval. If
        /// indoor or outdoor data is unavailable, the report generation for that cycle is skipped and a warning is
        /// logged. Any exceptions encountered during execution are logged as errors.</remarks>
        /// <returns>A task that represents the asynchronous operation of the scheduler.</returns>
        public async Task StartAsync()
        {
            Console.WriteLine($"Scheduler started. Generating reports every {_intervalMinutes} minutes.");
            LoggerService.LogInfo($"Scheduler started.");

            while (true) // Run indefinitely until the application is stopped
            {
                await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes)); // Wait for the specified interval before generating the next report

                try // Attempt to generate a report using indoor and outdoor data
                {
                    var indoorData = _getIndoorData();

                    if (indoorData == null) // If no indoor data is available, log a warning and skip report generation for this cycle  
                    {
                        LoggerService.LogWarn("No indoor sensor data available. Skipping report generation.");
                        continue;
                    }

                    var outdoorData = await _weatherService.GetWeatherAsync(); // Retrieve outdoor weather data asynchronously

                    if (outdoorData == null) // If no outdoor data is available, log a warning and skip report generation for this cycle
                    {
                        LoggerService.LogWarn("Failed to retrieve outdoor weather data. Skipping report generation.");
                        continue;
                    }

                    _reportGenerator.GenerateReport(indoorData, outdoorData); // Generate a report using the retrieved indoor and outdoor data
                }
                catch (Exception ex) // If any exceptions occur during report generation, log the error message
                {
                    LoggerService.LogError($"Scheduler exception: {ex.Message}");

                }
            }
		}
    }
}
