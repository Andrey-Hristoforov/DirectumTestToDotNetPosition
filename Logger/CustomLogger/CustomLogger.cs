using System.Diagnostics;
using Newtonsoft.Json;

namespace CustomLogger
{
    public class CustomLogger : ILogger
    {
        private const string _defaultLogFilePath = "MyLogFile1.txt";

        private static CustomLogger? _instance;
        private static object syncRoot = new Object();

        private static Dictionary<LogMessageSeverity, Action<string>>? _severityToAction;

        ILoggerCore _loggerCore;
        private CustomLogger(string logFileName)
        {
            _loggerCore = new NLogLoggerCore(GetPathFromConfigFile() ?? logFileName);
            _severityToAction = new Dictionary<LogMessageSeverity, Action<string>>()
            {
                { LogMessageSeverity.Debug, _loggerCore.LogDebug },
                { LogMessageSeverity.Info, _loggerCore.LogInfo },
                { LogMessageSeverity.Warning, _loggerCore.LogWarning },
                { LogMessageSeverity.Error, _loggerCore.LogError }
            };
        }

        private string? GetPathFromConfigFile()
        {
            if (new DirectoryInfo(Directory.GetCurrentDirectory()).EnumerateFiles().Any(fi => fi.Name == "CustomLoggerConfig.json"))
            {
                var content = File.ReadAllText(Directory.GetCurrentDirectory() + "/CustomLoggerConfig.json");
                var listOfOptions = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(content);
                var result = listOfOptions?.FirstOrDefault(o => o.Key == "logFilePath").Value;
                return result;
            }
            return null;
        }

        public static CustomLogger GetLogger()
        {
            if(_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new CustomLogger(_defaultLogFilePath);
                }
            }
            return _instance;
        }
        public void LogMessage(string source, string text, LogMessageSeverity severity)
        {
            if (severity == LogMessageSeverity.Debug)
            {
#if DEBUG
                _loggerCore.LogDebug(CommonMessageTemplate(DateTime.Now, severity.ToString(), source, text));
                return;
#endif
            }
            _severityToAction[severity](CommonMessageTemplate(DateTime.Now, severity.ToString(), source, text));
        }
        public void LogException(string source, Exception exception, bool isCritical)
        {
            _loggerCore.LogException(ExceptionMessageTemplate(
                DateTime.Now,
                isCritical,
                source,
                exception.Message,
                exception.StackTrace ?? "[StackTraceWasNull]"
                ));
        }

        private string CommonMessageTemplate(DateTime dateTime, string level, string sender, string message)
            => "time: " + dateTime.ToString() + "\n" +
            "level: " + level.ToUpper() + "\n" +
            "sender: " + sender + "\n" +
            "message: " + message + "\n" +
            "------------------------------------------";

        private string ExceptionMessageTemplate(DateTime dateTime, bool isCritical, string sender, string message, string stackTrace)
            => "time: " + dateTime.ToString() + "\n" +
            (isCritical ? "CRITICAL EXCEPTION!!!" : "EXCEPTION") + "\n" +
            "sender: " + sender + "\n" +
            "message: " + message + "\n" +
            "stackTrace: " + stackTrace + "\n" +
            "------------------------------------------";
    }
}