using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.ExportAnalysisReports.Helpers
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
				"TradosExportAnalysisReports");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TradosExportAnalysisReportsLogs",
				FileName = Path.Combine(logDirectoryPath, "TradosExportAnalysisReportsLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*ExportAnalysisReports*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}