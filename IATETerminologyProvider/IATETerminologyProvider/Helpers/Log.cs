using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.IATETerminologyProvider.Helpers
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community",
				"IATEProviderLogs");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "IATETerminologyProvider",
				FileName = Path.Combine(logDirectoryPath, "IATEProviderLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*IATETerminologyProvider*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}