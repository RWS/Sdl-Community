using System;
using System.IO;
using System.Text;
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

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
				"IATETerminologyProvider", "Logs");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "IATETerminologyProvider",
				FileName = Path.Combine(logDirectoryPath, "IATEProviderLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}",
				Encoding = Encoding.UTF8
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*IATETerminologyProvider*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}