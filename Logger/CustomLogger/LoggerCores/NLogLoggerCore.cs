using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLogger
{
    internal class NLogLoggerCore : ILoggerCore
    {
        private const string _layout = "${message}";
        private Logger _logger;
        public NLogLoggerCore(string logFileName)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = new NLog.Targets.FileTarget();
            logFile.FileName = logFileName;
            logFile.Name = "f";
            logFile.Layout = _layout;
            config.LoggingRules.Add(
                new NLog.Config.LoggingRule("*", LogLevel.Debug, logFile)
                );
            LogManager.Configuration = config;

            _logger = LogManager.GetCurrentClassLogger();
        }
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }
        public void LogInfo(string message)
        {
            _logger.Info(message);
        }
        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }
        public void LogError(string message)
        {
            _logger.Error(message);
        }
        public void LogException(string message)
        {
            _logger.Fatal(message);
        }
    }
}
