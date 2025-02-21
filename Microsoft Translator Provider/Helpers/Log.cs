using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MicrosoftTranslatorProvider.Helpers
{
	public static class Log
	{
		public static void Setup()
		{
			LogManager.Configuration ??= new LoggingConfiguration();
			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.LogsFolderPath, PluginResources.AppLogFolder);
			if (!Directory.Exists(logDirectoryPath))
			{
				Directory.CreateDirectory(logDirectoryPath);
			}

			var target = new FileTarget
			{
				Name = "MicrosoftTranslatorProvider",
				FileName = Path.Combine(logDirectoryPath, PluginResources.LogsFileName),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			LogManager.Configuration.AddTarget(target);
			LogManager.Configuration.AddRuleForAllLevels(target, "*MicrosoftTranslatorProvider*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}