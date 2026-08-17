using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Log
    {
        private static bool _isInitialized;

        public static Logger GetLogger(string name)
        {
            if (!_isInitialized) Log.Setup();
            return LogManager.GetLogger(name);
        }

        private static void Setup()
        {
            _isInitialized = true;
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }
            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.DeepLDataPath, "DeepLLogs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "DeepL",
                FileName = Path.Combine(logDirectoryPath, "DeeplLogs.txt"),
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*DeepL*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}