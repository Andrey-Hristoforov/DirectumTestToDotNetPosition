using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace CustomLogger
{
    internal interface ILoggerCore
    {
        public void LogDebug(string message);
        public void LogInfo(string message);
        public void LogWarning(string message);
        public void LogError(string message);
        public void LogException(string message);
    }
}
