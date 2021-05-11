using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.SDLBatchAnonymize
{
	public sealed class Log
	{
		public static void Setup()
		{
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}
			var config = LogManager.Configuration;

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "TradosBatchAnonymizer");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "BatchAnonymize",
				FileName = Path.Combine(logDirectoryPath, "TradosBatchAnonymizer.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*BatchAnonymize*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}