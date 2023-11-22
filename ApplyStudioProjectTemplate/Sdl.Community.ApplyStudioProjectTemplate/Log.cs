using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.ApplyStudioProjectTemplate
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
				"Apply Studio Project Template", "Logs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "ApplyStudioProjectTemplate",
                FileName = Path.Combine(logDirectoryPath, "ApplyStudioProjectTemplateLogs.txt"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*ApplyStudioProjectTemplate*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}
