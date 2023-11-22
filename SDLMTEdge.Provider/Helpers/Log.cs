using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace Sdl.Community.MTEdge.Provider.Helpers
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
			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "SDLMachineTranslationEdge");
			Directory.CreateDirectory(logDirectoryPath);
			var target = new FileTarget
			{
				Name = "ETS",
				FileName = Path.Combine(logDirectoryPath, "SDLMachineTranslationEdgeLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*MTEdge*");
			LogManager.ReconfigExistingLoggers();
		}
	}
}