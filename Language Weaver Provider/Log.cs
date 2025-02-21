using System;
using System.IO;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LanguageWeaverProvider
{
    public static class Log
    {
        public static void Setup()
        {
            LogManager.Configuration ??= new LoggingConfiguration();

            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
                "Language Weaver", "Logs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "LanguageWeaver",
                FileName = Path.Combine(logDirectoryPath, "LanguageWeaverProvider.Logs.txt"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                Encoding = Encoding.UTF8,
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*LanguageWeaverProvider*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}
