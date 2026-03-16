using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using EnvironmentalMonitor.Models;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Provides access to current weather information for a predefined city using the OpenWeatherMap API.
    /// </summary>
    /// <remarks>This service requires a valid API key and uses a fixed city name to retrieve weather data.
    /// The weather information includes temperature, humidity, and a textual description. The service is intended for
    /// asynchronous use and returns null if the API request fails or an exception occurs during the
    /// operation.</remarks>
    public class WeatherService
    {
        private readonly string _apiKey = "YOUR_API_KEY_HERE";
        private readonly string _city = "CITY_HERE";

        /// <summary>
        /// Asynchronously retrieves the current weather data for the configured city from the OpenWeatherMap API.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to the OpenWeatherMap API using the city and
        /// API key specified in the service. Ensure that the API key is valid and the city name is correctly
        /// configured. Errors and failed requests are logged using <see cref="LoggerService"/>.</remarks>
        /// <returns>An instance of <see cref="WeatherData"/> containing the temperature in Celsius and Fahrenheit, humidity, and
        /// a description of the weather. Returns <see langword="null"/> if the request fails or an error occurs.</returns>
        public async Task<WeatherData?> GetWeatherAsync()
        {
            try // Handle potential exceptions from HTTP requests and JSON parsing
            {
                using HttpClient client = new HttpClient();

                string url = $"https://api.openweathermap.org/data/2.5/weather?q={_city}&appid={_apiKey}&units=metric";

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) // Log the error and return null if the API request was unsuccessful
                {
                    LoggerService.LogError($"Weather API request failed.");
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(json);

                double tempC = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(); // Extract temperature in Celsius from the JSON response
                int humidity = doc.RootElement.GetProperty("main").GetProperty("humidity").GetInt32(); // Extract humidity from the JSON response
                string description = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString() ?? "N/A"; // Extract weather description from the JSON response, defaulting to "N/A" if not available

                return new WeatherData // Create and return a WeatherData object populated with the extracted information
                {
                    TemperatureC = tempC,
                    TemperatureF = (tempC * 9 / 5) + 32,
                    Humidity = humidity,
                    Description = description
                };
            }
            catch (Exception ex) // Log any exceptions that occur during the API request or JSON parsing and return null
            {
                LoggerService.LogError($"Weather API exception: {ex.Message}");
                return null;
            }
        }
    }
}
