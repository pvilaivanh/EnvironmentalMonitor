using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EnvironmentalMonitor.Models;

namespace EnvironmentalMonitor.Core
{
    /// <summary>
    /// Provides functionality to parse JSON strings into SensorData objects, validating temperature and humidity values
    /// against defined ranges.
    /// </summary>
    /// <remarks>The MessageParser class is intended for converting JSON representations of sensor data into
    /// strongly typed SensorData instances. It ensures that the input JSON uses the expected property names and that
    /// the resulting values for temperature and humidity fall within acceptable limits. If the input is invalid or the
    /// values are out of range, parsing will fail and null will be returned. This class is typically used in scenarios
    /// where sensor data is received in JSON format and must be validated before further processing.</remarks>
    public class MessageParser
    {
        /// <summary>
        /// Parses a JSON string and returns a corresponding SensorData object if the data is valid.
        /// </summary>
        /// <remarks>The method normalizes property names in the input JSON to match expected casing and
        /// validates that temperature and humidity values fall within supported ranges. If the input is malformed or
        /// any value is out of range, the method returns null.</remarks>
        /// <param name="json">A JSON-formatted string that represents sensor data. The string must include temperature and humidity values
        /// using either 'TemperatureC', 'TemperatureF', or 'Humidity' property names.</param>
        /// <returns>A SensorData object if the JSON is successfully parsed and all values are within valid ranges; otherwise,
        /// null.</returns>
        public static SensorData? Parse(string json)
        {
            // Normalize property names to match expected casing
            try
            {
                // Replace property names in the JSON string to ensure they match the expected casing for deserialization
                json = json.Replace("TemperatureC", "\"temperatureC\"");
                json = json.Replace("TemperatureF", "\"temperatureF\"");
                json = json.Replace("Humidity", "\"humidity\"");

                
                var options = new JsonSerializerOptions // Configure the JSON serializer to ignore case when matching property names
                {
                    PropertyNameCaseInsensitive = true
                };  

				var data = JsonSerializer.Deserialize<SensorData>(json, options); // Attempt to deserialize the JSON string into a SensorData object using the configured options

                if (data == null) // If deserialization fails and returns null, return null to indicate parsing failure
                {
                    return null;
                }

                if (data.TemperatureC < -45 || data.TemperatureC > 65)
                {
                    return null; 
                }
                
                if (data.Humidity < 0 || data.Humidity > 100)
                {
                    return null;
                }

                if (data.TemperatureF < -50 || data.TemperatureF > 150)
                {
                    return null;
                }

                return data;
            }
            // Catch any exceptions that occur during parsing and return null to indicate failure
            catch
            {
                
                return null;
            }
        }
    }
}
