using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MTEnhancedMicrosoftProvider.Helpers
{
	public static class Log
	{
		public static void Setup()
		{
			if (LogManager.Configuration is null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}

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

			LogManager.Configuration.AddTarget(target);
			LogManager.Configuration.AddRuleForAllLevels(target, "*MtEnhanced*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}