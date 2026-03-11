using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EnvironmentalMonitor.Services
{
    /// <summary>
    /// Provides static methods for logging informational, warning, and error messages to daily log files organized by
    /// date.
    /// </summary>
    /// <remarks>The LoggerService automatically creates a log directory if it does not exist and stores log
    /// files in subdirectories named by date. Each log entry is timestamped and prefixed with its severity level. This
    /// service is intended for general-purpose application logging and is thread-safe for typical usage
    /// scenarios.</remarks>
    public class LoggerService
    {
        private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(),"..", "..", "..", "Logs");

        /// <summary>
        /// Initializes static members of the LoggerService class and ensures that the log directory exists before any
        /// logging operations occur.
        /// </summary>
        /// <remarks>If the directory specified by LogDirectory does not exist, it is created during
        /// static initialization. This guarantees that logging can proceed without directory-related errors.</remarks>
        static LoggerService()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        /// <summary>
        /// Gets the full file path for the log file corresponding to the current date, ensuring that the required
        /// directory structure exists.
        /// </summary>
        /// <remarks>If the directory for the log file does not exist, it is created automatically before
        /// returning the file path. The log file is organized by date to facilitate daily logging.</remarks>
        /// <returns>The complete path to the log file for the current date, named in the format 'log_yyyy-MM-dd.txt'.</returns>
        private static string GetLogFilePath()
        {
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            string fullPath = Path.Combine(LogDirectory, $"log_{dateFolder}.txt");

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            string fileName = $"log_{DateTime.Now.ToString("yyyy-MM-dd")}.txt";

            return Path.Combine(fullPath, fileName);
        }

        /// <summary>
        /// Appends a log entry with the specified severity level and message to the log file, including a timestamp.
        /// </summary>
        /// <remarks>The log entry is written to the file path returned by the GetLogFilePath method.
        /// Ensure that the log file is accessible and writable to avoid exceptions during logging.</remarks>
        /// <param name="level">The severity level of the log entry, such as "Info", "Warning", or "Error". This value is included in the
        /// log output to indicate the importance or type of the event.</param>
        /// <param name="message">The message to be logged, providing details or context about the event being recorded.</param>
        private static void Write(string level, string message)
        {
            string logEntryy = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            string filePath = GetLogFilePath();

            File.AppendAllText(filePath, logEntryy + Environment.NewLine, Encoding.UTF8);
        }

        /// <summary>
        /// Logs an informational message with an 'INFO' prefix to the output stream.
        /// </summary>
        /// <param name="message">The message to log. This should provide context or details about the informational event being recorded.</param>
        public static void LogInfo(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Logs a warning message to the output with a 'WARNING' prefix.
        /// </summary>
        /// <param name="message">The message to log as a warning. This should provide context about the warning being issued.</param>
        public static void LogWarn(string message)
        {
            Write("WARNING", message);
        }

        /// <summary>
        /// Logs an error message to the output with an 'ERROR' prefix.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message) 
        {
            Write("ERROR", message);
        }
    }
}
