using System;
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace TMX_Lib.Utils
{
	public static class LogUtil
	{
		public static void Setup()
		{
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}

			var config = LogManager.Configuration;
			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "TMX_lib");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TMX_Lib",
				FileName = Path.Combine(logDirectoryPath, "TMX_lib.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};
			var debugTarget = new DebugTarget
			{
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}", 
				Name = "debug",
			};

			config.AddTarget(target);
			if (Debugger.IsAttached)
				config.AddTarget(debugTarget);
			config.AddRuleForAllLevels(target, "*Sdl.Community.BackupService*");

			//NLog object
			LogManager.ReconfigExistingLoggers();
		}
	}
}
