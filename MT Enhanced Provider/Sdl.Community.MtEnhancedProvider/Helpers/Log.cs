using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.MtEnhancedProvider.Helpers
{
	public static class Log
	{
		private static readonly Constants Constants = new Constants();

		public static void Setup()
		{
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}
			var config = LogManager.Configuration;

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.SDLCommunity, Constants.SDLMTEnhanced);
			if (!Directory.Exists(logDirectoryPath))
			{
				Directory.CreateDirectory(logDirectoryPath);
			}
			var target = new FileTarget
			{
				Name = "MTEnhancedProvider",
				FileName = Path.Combine(logDirectoryPath, Constants.SDLMTEnhancedLogFile),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*MtEnhanced*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}