using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentalMonitor.Models
{
    /// <summary>
    /// Represents weather information, including temperature, humidity, and a textual description of the current
    /// conditions.
    /// </summary>
    /// <remarks>Temperature values are available in both Fahrenheit and Celsius. The description provides a
    /// human-readable summary of the weather, such as "Clear" or "Partly Cloudy". This class is typically used to
    /// encapsulate weather data retrieved from sensors or external services.</remarks>
    public class WeatherData
    {
        public double TemperatureF { get; set; }
        public double TemperatureC { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
