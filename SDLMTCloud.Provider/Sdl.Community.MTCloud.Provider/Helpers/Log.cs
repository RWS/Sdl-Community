using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.MTCloud.Provider.Helpers
{
	public sealed class Log
	{
		public static void Setup()
		{
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}

			var config = LogManager.Configuration;

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.TradosAppStore, Constants.SDLMachineTranslationCloud);

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "SDLMTCloud",
				FileName = Path.Combine(logDirectoryPath, "LanguageWeaverCloudLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*MTCloud*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}