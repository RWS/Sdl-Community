using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.BackupService.Helpers
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
				"Trados TM Backup");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TradosTMBackupLogs",
				FileName = Path.Combine(logDirectoryPath, "TradosTMBackupLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*Sdl.Community.BackupService*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}
