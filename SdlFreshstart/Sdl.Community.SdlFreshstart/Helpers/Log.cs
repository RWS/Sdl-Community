using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.SdlFreshstart.Helpers
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
				"TradosFreshstartLogs");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TradosFreshstart",
				FileName = Path.Combine(logDirectoryPath, "TradosFreshstartLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*SdlFreshstart*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}