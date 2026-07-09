using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace VerifyFilesAuditReport.Logging
{
    public static class Log
    {
        private static bool _isInitialized;

        public static Logger GetLogger(string name)
        {
            if (!_isInitialized)
                Log.Setup();
            return LogManager.GetLogger(name);
        }

        private static void Setup()
        {
            _isInitialized = true;
            LogManager.Configuration ??= new LoggingConfiguration();
            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Trados AppStore\Verify Files - Audit Report", "Logs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "Verify Files - Audit Report",
                FileName = Path.Combine(logDirectoryPath, "Logs.txt"),
                Layout = "${longdate} | ${level:uppercase=true:padding=-5} | ${logger} | ${message}${onexception: | ${exception:format=tostring}}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*Verify*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}