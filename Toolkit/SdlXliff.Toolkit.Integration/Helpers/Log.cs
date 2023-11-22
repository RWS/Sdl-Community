using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SdlXliff.Toolkit.Integration.Helpers
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Trados AppStore",
				"SdlXliffToolkitLogs");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "DeepL",
				FileName = Path.Combine(logDirectoryPath, "SdlXliffToolkit.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*SDLXLIFF*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}