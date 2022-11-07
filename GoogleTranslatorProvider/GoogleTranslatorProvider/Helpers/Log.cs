using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace GoogleTranslatorProvider.Helpers
{
	public static class Log
	{
		public static void Setup()
		{
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}
			var config = LogManager.Configuration;

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.LogsFolderPath, PluginResources.AppLogFolder);
			if (!Directory.Exists(logDirectoryPath))
			{
				Directory.CreateDirectory(logDirectoryPath);
			}
			var target = new FileTarget
			{
				Name = "MTEnhancedProvider",
				FileName = Path.Combine(logDirectoryPath, PluginResources.LogsFileName),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*MtEnhanced*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}