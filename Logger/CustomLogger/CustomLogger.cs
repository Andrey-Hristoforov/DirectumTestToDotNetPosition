using System.Diagnostics;
using Newtonsoft.Json;

namespace Customlogging
{
    public class CustomLogger : ILogger
    {
        private const string _defaultLogFilePath = "MyLogFile1.txt";

        private static CustomLogger? _instance;
        private static object syncRoot = new Object();

        private Dictionary<LogMessageSeverity, Action<string>> _severityToAction;

        ILoggerCore _loggerCore;
        ILoggerMessageTemplateProvider _messageTemplate;

        private CustomLogger(ILoggerCore? loggerCore, ILoggerMessageTemplateProvider? messageTemplate, string? logFileName)
        {
            if (loggerCore != null)
                _loggerCore = loggerCore;
            else
                _loggerCore = new NLogLoggerCore(GetPathFromConfigFile() ?? _defaultLogFilePath);

            if (messageTemplate != null)
                _messageTemplate = messageTemplate;
            else
                _messageTemplate = new BlockTemplateProvider();

            _severityToAction = new Dictionary<LogMessageSeverity, Action<string>>()
            {
                { LogMessageSeverity.Debug, _loggerCore.LogDebug },
                { LogMessageSeverity.Info, _loggerCore.LogInfo },
                { LogMessageSeverity.Warning, _loggerCore.LogWarning },
                { LogMessageSeverity.Error, _loggerCore.LogError }
            };
        }

        public static void CustomLoggerInit(ILoggerCore? loggerCore)
        {
            if (loggerCore == null)
                throw new ArgumentNullException("loggerCore was null");
            _instance = new CustomLogger(loggerCore, null, null);
        }

        public static void CustomLoggerInit(ILoggerMessageTemplateProvider? templateProvider)
        {
            if (templateProvider == null)
                throw new ArgumentNullException("templateProvider was null");
            _instance = new CustomLogger(null, templateProvider, null);
        }

        public static void CustomLoggerInit(ILoggerCore? loggerCore, ILoggerMessageTemplateProvider? templateProvider)
        {
            if (loggerCore == null)
                throw new ArgumentNullException("loggerCore was null");
            if (templateProvider == null)
                throw new ArgumentNullException("templateProvider was null");
            _instance = new CustomLogger(loggerCore, templateProvider, null);
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
                        _instance = new CustomLogger(null, null, _defaultLogFilePath);
                }
            }
            return _instance;
        }
        public void LogMessage(string source, string text, LogMessageSeverity severity)
        {
            if (severity == LogMessageSeverity.Debug)
            {
#if DEBUG
                _loggerCore.LogDebug(
                    _messageTemplate
                    .CommonMessageTemplate(
                        DateTime.Now,
                        severity.ToString(),
                        source, text));
                return;
#endif
            }
            _severityToAction[severity](_messageTemplate
                .CommonMessageTemplate(
                DateTime.Now, 
                severity.ToString(), 
                source, text));
        }
        public void LogException(string source, Exception exception, bool isCritical)
        {
            _loggerCore.LogException(_messageTemplate.
                ExceptionMessageTemplate(
                    DateTime.Now,
                    isCritical,
                    source,
                    exception.Message,
                    exception.StackTrace ?? "[StackTraceWasNull]"));
        }

    }
}