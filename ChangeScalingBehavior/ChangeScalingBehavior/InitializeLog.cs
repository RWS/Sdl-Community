using System;
using NLog.Config;
using NLog.Targets;
using NLog;
using System.IO;

namespace ChangeScalingBehavior
{
	internal static class InitializeLog
    {
        public static Logger Retlogger()
        {
            return LogManager.GetLogger("log"); ;
        }
        public static void InitializeLoggingConfiguration()
        {
            var loggingConfiguration = new LoggingConfiguration();
            var fileTarget = new FileTarget();

	        loggingConfiguration.AddTarget("file", fileTarget);
            fileTarget.CreateDirs = true;
            fileTarget.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),@"C:\ErrorHighDPI.log");
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}";

            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            loggingConfiguration.LoggingRules.Add(rule);

            LogManager.Configuration = loggingConfiguration;
        }
    }
}
