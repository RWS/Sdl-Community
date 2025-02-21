using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.StyleSheetVerifier
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "StyleSheetVerifier");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "ETS",
				FileName = Path.Combine(logDirectoryPath, "StyleSheetVerifierLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*StyleSheetVerifier*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}