using System;
using System.Collections.Generic;
using System.Text;

namespace EnvironmentalMonitor.Models
{
    /// <summary>
    /// Represents environmental sensor readings, including temperature and humidity data.
    /// </summary>
    /// <remarks>The temperature is available in both Fahrenheit and Celsius to support different measurement
    /// preferences. This class is typically used to encapsulate a single set of readings from an environmental
    /// monitoring sensor.</remarks>
    public class SensorData
    {
        public double TemperatureF { get; set; }
        public double TemperatureC { get; set; }
        public double Humidity { get; set; }

    }
}
