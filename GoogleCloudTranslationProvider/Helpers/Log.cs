using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GoogleCloudTranslationProvider.Helpers
{
	public static class Log
	{
		private const string AppLogFolder = "Logs";
		private const string LogsFileName = "GoogleCloudTPLogs.txt";

		public static void Setup()
		{
			LogManager.Configuration ??= new LoggingConfiguration();
			var logPath = Path.Combine(Constants.AppDataFolder, AppLogFolder);
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