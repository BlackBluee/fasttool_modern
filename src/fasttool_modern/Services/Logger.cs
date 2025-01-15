using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
namespace fasttool_modern.Services
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
        private readonly string _logDirectory;
        private readonly string _logFilePath;
        private Logger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";
            _logFilePath = Path.Combine(_logDirectory, fileName);
            File.WriteAllText(_logFilePath, "[]");
        }
        public static Logger Instance => _instance.Value;
        public void Log(string type, string message, Exception exception = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                Type = type,
                Message = message,
                Exception = exception?.ToString()
            };
            string jsonString = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            AppendLogEntry(jsonString);
        }
        public void LogInfo(string message)
        {
            Log("INFO", message);
        }
        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }
        public void LogError(string message, Exception exception = null)
        {
            Log("ERROR", message, exception);
        }
        private void AppendLogEntry(string jsonString)
        {
            string currentContent = File.ReadAllText(_logFilePath);
            var logEntries = JsonSerializer.Deserialize<dynamic[]>(currentContent);
            var updatedLogEntries = new List<dynamic>(logEntries ?? Array.Empty<dynamic>()) { JsonSerializer.Deserialize<dynamic>(jsonString) };
            string updatedContent = JsonSerializer.Serialize(updatedLogEntries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_logFilePath, updatedContent);
        }
    }
}