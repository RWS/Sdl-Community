using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GoogleTranslatorProvider.Helpers
{
	public static class Log
	{
		// Modify this to Constants.DefaultDownloadableLocation or similar
		private const string LogsFolderPath = "Trados AppStore";
		private const string AppLogFolder = "GoogleCloudTranslationProvider";
		private const string LogsFileName = "GoogleCloudTPLogs.txt";

		public static void Setup()
		{
			LogManager.Configuration ??= new LoggingConfiguration();
			var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LogsFolderPath, AppLogFolder);
			if (!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			var target = new FileTarget
			{
				Name = "GoogleCloudTranslationProvider",
				FileName = Path.Combine(logPath, LogsFileName),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			var config = LogManager.Configuration;
			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*GoogleCloudTranslationProvider*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}