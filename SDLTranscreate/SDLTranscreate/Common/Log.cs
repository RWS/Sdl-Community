using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Trados.Transcreate.Common
{
	public sealed class Log
	{
		private static bool _isSetup;
		
		public static void Setup()
		{
			if (_isSetup)
			{
				return;
			}

			_isSetup = true;
			if (LogManager.Configuration == null)
			{
				LogManager.Configuration = new LoggingConfiguration();
			}

			LoggingConfiguration config = LogManager.Configuration;

			string logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Trados AppStore", "Transcreate");

			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "Transcreate",
				FileName = Path.Combine(logDirectoryPath, "TranscreateLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}",

				// Roll over the log every 10 MB
				ArchiveAboveSize = 10000000,
				ArchiveNumbering = ArchiveNumberingMode.Date
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*Transcreate*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}
