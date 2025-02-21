using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SDLCopyTags.Helpers
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
				"TradosCopyTags");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "CopyTags",
				FileName = Path.Combine(logDirectoryPath, "TradosCopyTagsLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "**CopyTags*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}
