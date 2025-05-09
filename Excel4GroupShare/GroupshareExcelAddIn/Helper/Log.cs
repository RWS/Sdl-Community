using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;

namespace GroupshareExcelAddIn.Helper
{
    public static class Log
    {
        private static bool _isInitialized;

        public static Logger GetLogger(string name)
        {
            if (!_isInitialized) Log.Setup();
            return LogManager.GetLogger(name);
        }

        public static void Setup()
        {
            _isInitialized = true;
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }
            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
                "Excel4GS");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "Excel4GS",
                FileName = Path.Combine(logDirectoryPath, "Excel4GSLogs.txt"),
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*GroupshareExcel*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}