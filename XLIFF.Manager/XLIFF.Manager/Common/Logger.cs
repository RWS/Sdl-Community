using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.XLIFF.Manager.Common
{
	public class Logger
	{
		public void Setup(PathInfo pathInfo)
		{
			var config = new LoggingConfiguration();
			var assembly = Assembly.GetExecutingAssembly();
			//var logFullPath = Path.Combine(pathInfo.ApplicationLogsFolderPath, "log." + GetDateToString() + ".txt");

			var target = new FileTarget("XLIFF.Manager.Logs")
			{
				FileName = Path.Combine(pathInfo.ApplicationLogsFolderPath, "log." + GetDateToString() + ".txt"),

				// Roll over the log every 10 MB
				ArchiveAboveSize = 10000000,
				ArchiveNumbering = ArchiveNumberingMode.Date,

				// Path.combine nor string.format like the {#####}, which is used to replace the date, therefore
				// we need to do basic string concatenation.
				ArchiveFileName = pathInfo.ApplicationLogsFolderPath + "/" + assembly.GetName().Name + ".log.{#####}.txt"
			}; 



			config.AddTarget("file", target);
			var ruleDebug = new LoggingRule("*", LogLevel.Debug, target);			
			config.LoggingRules.Add(ruleDebug);


			LogManager.Configuration = config;

			LogManager.GetCurrentClassLogger().Info("Started");
		}

		private static string GetDateToString()
		{
			var now = DateTime.Now;
			var value = now.Year
			            + "" + now.Month.ToString().PadLeft(2, '0')
			            + "" + now.Day.ToString().PadLeft(2, '0')
			            + "" + now.Hour.ToString().PadLeft(2, '0')
			            + "" + now.Minute.ToString().PadLeft(2, '0')
			            + "" + now.Second.ToString().PadLeft(2, '0');

			return value;
		}
	}
}
