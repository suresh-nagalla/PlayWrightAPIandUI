namespace UIAutomation.Helpers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class Logger
    {
        private static readonly object LockObj = new object();
        private static Logger? _instance;
        private readonly string _logDirectory;
        private readonly string _logFilePath;

        private Logger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            _logFilePath = Path.Combine(_logDirectory, $"UITest_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            Console.WriteLine($"Log Path : {_logFilePath}");
            Console.Out.Flush();  // Ensures that the log path is flushed to the console immediately
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        _instance ??= new Logger();
                    }
                }
                return _instance;
            }
        }

        public void Info(string message, [CallerMemberName] string callerName = "")
        {
            LogMessage("INFO", message, callerName);
        }

        public void Warning(string message, [CallerMemberName] string callerName = "")
        {
            LogMessage("WARNING", message, callerName);
        }

        public void Error(string message, Exception? ex = null, [CallerMemberName] string callerName = "")
        {
            string errorMessage = ex != null
                ? $"{message}. Exception: {ex.Message}. StackTrace: {ex.StackTrace}"
                : message;

            LogMessage("ERROR", errorMessage, callerName);
        }

        private void LogMessage(string level, string message, string callerName)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] [{callerName}] - {message}";

            // Write to log file
            lock (LockObj)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }

            // Print log entry to console
            Console.WriteLine(logEntry);
        }
    }
}
