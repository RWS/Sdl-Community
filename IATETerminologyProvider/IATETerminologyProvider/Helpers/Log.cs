using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace IATETerminologyProvider.Helpers
{
	public sealed class Log
	{
		public static Logger Logger;
		private static readonly Lazy<Log> _instance = new Lazy<Log>(() => new Log());

		private Log()
		{
			var config = new LoggingConfiguration();
			var assembly = Assembly.GetExecutingAssembly();
			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL",
				"IATEProviderLogs");
			if (!Directory.Exists(logDirectoryPath))
			{
				Directory.CreateDirectory(logDirectoryPath);
			}
			var target = new FileTarget
			{
				FileName = Path.Combine(logDirectoryPath, "IATEProviderLogs.txt"),
				// Roll over the log every 10 MB
				ArchiveAboveSize = 10000000,
				ArchiveNumbering = ArchiveNumberingMode.Date,

				// Path.combine nor string.format like the {#####}, which is used to replace the date, therefore
				// we need to do basic string concatenation.
				ArchiveFileName = logDirectoryPath + "/" + assembly.GetName().Name + ".log.{#####}.txt"
			};

			config.AddTarget("file", target);
			var rule = new LoggingRule("*", LogLevel.Debug, target);
			config.LoggingRules.Add(rule);
			LogManager.Configuration = config;
			//NLog object
			Logger = LogManager.GetCurrentClassLogger();
		}

		public static Log Instance => _instance.Value;
	}
}