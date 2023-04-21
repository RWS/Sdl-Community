using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.SdlDataProtectionSuite
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
				"Trados Data Protection Suite Logs");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TradosDataProtectionSuite",
				FileName = Path.Combine(logDirectoryPath, "SDLDataProtectionSuiteLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*SdlDataProtectionSuite*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}