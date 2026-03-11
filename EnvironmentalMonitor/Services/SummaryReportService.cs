using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Provides functionality for generating summary reports from JSON report files within a specified date range.
    /// </summary>
    /// <remarks>This service processes report files located in the "Reports" directory, calculating indoor
    /// temperature changes over a given period and producing both human-readable and JSON summary files. It logs
    /// warnings if the report directory is missing or if no reports are found in the specified range. Use this service
    /// to automate summary report generation for environmental monitoring data.</remarks>
    public class SummaryReportService
    {
        private readonly string _reportDirectory = "Reports";

        /// <summary>
        /// Generates a summary report based on JSON files created within the specified date range.
        /// </summary>
        /// <remarks>If the report directory does not exist, a warning is logged and the method exits. If
        /// no reports are found within the specified date range, a warning is logged. The summary includes the
        /// temperature change from the first to the last report in the selected range.</remarks>
        /// <param name="startDate">The start date of the range for which to generate the summary report. This date is inclusive.</param>
        /// <param name="endDate">The end date of the range for which to generate the summary report. This date is inclusive.</param>
        public void GenerateSummary(DateTime startDate, DateTime endDate)
        {
            if (!Directory.Exists(_reportDirectory))
            {
                LoggerService.LogWarn($"Report directory not found.");
                return;
            }

            var files = Directory.GetFiles(_reportDirectory, "*.json");

            var selectReports = new List<JsonElement>();

            foreach (var file in files) // Loop through each JSON file in the report directory
            {
                string content = File.ReadAllText(file);
                using JsonDocument doc = JsonDocument.Parse(content);

                DateTime created = doc.RootElement.GetProperty("Created").GetDateTime();

                if (created >= startDate && created <= endDate) // Check if the report's creation date falls within the specified range
                {
                    selectReports.Add(doc.RootElement);
                }
            }

            if (selectReports.Count == 0) // If no reports were found in the specified date range, log a warning and exit
            {
                LoggerService.LogWarn($"No reports found in specified range.");
                return;
            }

            double firstTemp = selectReports.First() // Get the indoor temperature from the first report in the selected range
                .GetProperty("Indoor")
                .GetProperty("TemperatureF")
                .GetDouble();

            double lastTemp = selectReports.Last() // Get the indoor temperature from the last report in the selected range
                .GetProperty("Indoor")
                .GetProperty("TemperatureF")
                .GetDouble();

            double change = lastTemp - firstTemp; // Calculate the change in indoor temperature over the selected period

            CreateSummaryFile(startDate, endDate, change, selectReports.Count);
        }

        /// <summary>
        /// Generates a summary report for the specified period, including the indoor temperature change and the number
        /// of reports, and saves the results in both human-readable text and JSON formats.
        /// </summary>
        /// <remarks>The generated files are saved in the 'Reports' directory with unique names based on
        /// the current timestamp. The text file provides a readable summary, while the JSON file contains a structured
        /// representation of the same data. An informational log entry is created upon successful report
        /// generation.</remarks>
        /// <param name="start">The start date and time of the reporting period.</param>
        /// <param name="end">The end date and time of the reporting period.</param>
        /// <param name="tempChange">The change in indoor temperature, measured in degrees, during the reporting period.</param>
        /// <param name="reportCount">The total number of individual reports included in the summary.</param>
        public void CreateSummaryFile(DateTime start, DateTime end, double tempChange, int reportCount)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            
            string readablePath = Path.Combine("Reports", $"Summary_{timestamp}.txt");
            string jsonPath = Path.Combine("Reports", $"Summary_{timestamp}.json");

            string readable = $"Summary Report\n" +
                                     $"Start Date: {start}\n" +
                                     $"End Date: {end}\n" +
                                     $"Reports Included: {reportCount}\n" +
                                     $"Indoor Temperature Change: {tempChange}";

            File.WriteAllText(readablePath, readable);

            var jsonSummary = new // Anonymous type to hold the summary data for JSON serialization
            {
                StartDate = start,
                EndDate = end,
                ReportsIncluded = reportCount,
                IndoorTemperatureChange = tempChange
            };

            string jsonString = JsonSerializer.Serialize(jsonSummary, new JsonSerializerOptions // Configure the JSON serializer to produce indented output for better readability
            { 
                WriteIndented = true 
            });

            File.WriteAllText(jsonPath, jsonString);

            LoggerService.LogInfo($"Summary report generated.");
        }
    }
}
