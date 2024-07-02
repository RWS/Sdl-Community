using NLog.Config;
using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier
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
                "AntidoteVerifier", "Logs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "AntidoteVerifier",
                FileName = Path.Combine(logDirectoryPath, "AntidoteVerifier.Logs.txt"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                Encoding = Encoding.UTF8,
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*AntidoteVerifier*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}
