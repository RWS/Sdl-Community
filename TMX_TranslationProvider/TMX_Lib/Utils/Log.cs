using System;
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace TMX_Lib.Utils
{
	public sealed class LogDebugTarget : TargetWithLayout
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogDebugTarget" /> class.
		/// </summary>
		/// <remarks>
		/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
		/// </remarks>
		public LogDebugTarget()
		{
			LastMessage = string.Empty;
			Counter = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogDebugTarget" /> class.
		/// </summary>
		/// <remarks>
		/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
		/// </remarks>
		/// <param name="name">Name of the target.</param>
		public LogDebugTarget(string name) : this()
		{
			Name = name;
		}

		/// <summary>
		/// Gets the number of times this target has been called.
		/// </summary>
		/// <docgen category='Debugging Options' order='10' />
		public int Counter { get; private set; }

		/// <summary>
		/// Gets the last message rendered by this target.
		/// </summary>
		/// <docgen category='Debugging Options' order='10' />
		public string LastMessage { get; private set; }

		/// <inheritdoc/>
		protected override void Write(LogEventInfo logEvent)
		{
			Counter++;
			LastMessage = RenderLogEvent(Layout, logEvent);
			Debug.WriteLine(LastMessage);
		}
	}

	public static class LogUtil
	{
		public static string LogFileName => Path.Combine(Util.PluginDirectory, "TMX_lib.txt");
		public static void Setup(bool logToConsole = false)
		{
			var config = new LoggingConfiguration();

			var logDirectoryPath = Util.PluginDirectory;
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "TMX_Lib",
				FileName = Path.Combine(logDirectoryPath, "TMX_lib.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};
			var debugTarget = new LogDebugTarget
			{
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}", 
				Name = "debug",
			};
			var consoleTarget = new ConsoleTarget {
				Layout = "${longdate} ${level} ${message}  ${exception}",
				Name = "console",
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target);
			if (Debugger.IsAttached)
			{
				config.AddTarget(debugTarget);
				config.AddRuleForAllLevels(debugTarget);
			}
			if (logToConsole) {
				config.AddTarget(consoleTarget);
				config.AddRuleForAllLevels(consoleTarget);
			}

			LogManager.Configuration = config;
			//NLog object
			LogManager.ReconfigExistingLoggers();

			var logger = LogManager.GetCurrentClassLogger();
		    logger.Info("Logging initialized");
		    LogManager.Flush();
		}
	}
}
