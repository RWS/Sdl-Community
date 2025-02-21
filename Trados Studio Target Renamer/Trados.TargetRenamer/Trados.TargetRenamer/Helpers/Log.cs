using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Trados.TargetRenamer.Helpers
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
			var logDirectoryPath = Constants.AppDataLocation;
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = Constants.PluginName,
				FileName = Path.Combine(logDirectoryPath, $"{Constants.PluginName.Replace(" ", string.Empty)}.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*TargetRename*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}