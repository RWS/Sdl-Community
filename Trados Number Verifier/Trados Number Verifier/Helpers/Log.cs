using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.NumberVerifier.Helpers
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
				"Number Verifier");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "Number Verifier",
				FileName = Path.Combine(logDirectoryPath, "NumberVerifierLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*Sdl.Community.NumberVerifier*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}