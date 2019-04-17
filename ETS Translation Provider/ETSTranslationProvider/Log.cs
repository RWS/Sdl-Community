using System.IO;
using System.Threading;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ETSTranslationProvider
{
	static class Log
    {
        public static Logger Logger { get; private set; }

        static Log()
        {
            // The configuration has to be set here versus a log file as NLog insists a log file be
            // located in the same directory as the executable, which the plugin doesn't have write
            // access to.
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var fileTarget = new FileTarget())
            {
                var config = new LoggingConfiguration();
                config.AddTarget("file", fileTarget);

				var assemblyName = assembly.GetName().Name;

				var configFromFile = PluginConfiguration.CurrentInstance;
				var logFileDirectory = Path.Combine(configFromFile.Directory, "Logs");
				var logFileName = string.Format("{0}.log.txt", assemblyName);

                // Create the directory in case it doesn't exist. Does nothing if it does.
                Directory.CreateDirectory(logFileDirectory);
                fileTarget.FileName = Path.Combine(logFileDirectory, logFileName);

                // Roll over the log every 10 MB
                fileTarget.ArchiveAboveSize = 10000000;
                fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;

                // Path.combine nor string.format like the {#####}, which is used to replace the date, therefore
                // we need to do basic string concatenation.
                fileTarget.ArchiveFileName = logFileDirectory + "/" + assemblyName + ".log.{#####}.txt";

                fileTarget.Layout = "[${longdate}]" +
                                    "${pad:padding=10:inner=${level:uppercase=true}} " +
                                    "${pad:padding=-35:inner=${callsite:className=false:includeNamespace=False:includeSourcePath=False:methodName=true}} " +
                                    "${pad:padding=-45:inner=${message}} " +
                                    "${pad:padding=-55:inner=${exception}}";

                // If there is a configuration file, read the log level from that.
                var targetLogLevel = LogLevel.Info;
                if (configFromFile != null && configFromFile.LogLevel != null)
                    targetLogLevel = LogLevel.FromString(configFromFile.LogLevel);

                var rule = new LoggingRule("*", targetLogLevel, fileTarget);
                config.LoggingRules.Add(rule);
                LogManager.Configuration = config;
            }

            Logger = LogManager.GetCurrentClassLogger();
            Logger.Info("-----------------------------------------------");
            Logger.Info("Starting Log for {0}", assembly.FullName);

            Application.ThreadException += new ThreadExceptionEventHandler(LogUncaughtException);
        }

        static void LogUncaughtException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Trace("");
            Logger.Error(e.Exception, "An uncaught exception was thrown:");
            throw e.Exception;
        }
    }
}