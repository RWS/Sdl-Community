using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GoogleTranslatorProvider.Helpers
{
	public static class Log
	{
		private const string LogsFolderPath = "RWS AppStore";
		private const string AppLogFolder = "GoogleTranslatorProvider";
		private const string LogsFileName = "GoogleTPLogs.txt";

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
				Name = "GoogleTranslatorProvider",
				FileName = Path.Combine(logPath, LogsFileName),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			var config = LogManager.Configuration;
			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*GoogleTranslatorProvider*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}