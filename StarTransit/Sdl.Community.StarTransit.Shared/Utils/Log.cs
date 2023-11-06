using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.StarTransit.Shared.Utils
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "StarTransit");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "StarTransitLogs",
				FileName = Path.Combine(logDirectoryPath, "StarTransitLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*StarTransit*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}