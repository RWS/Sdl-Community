using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace GoogleTranslatorProvider.Helpers
{
	public static class Log
	{
		public static void Setup()
		{
			LogManager.Configuration ??= new LoggingConfiguration();
			var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.LogsFolderPath, PluginResources.AppLogFolder);
			if (!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			var target = new FileTarget
			{
				Name = "GoogleTranslatorProvider",
				FileName = Path.Combine(logPath, PluginResources.LogsFileName),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			var config = LogManager.Configuration;
			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*GoogleTranslatorProvider*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}