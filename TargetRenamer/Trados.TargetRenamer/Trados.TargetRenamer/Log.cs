using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Trados.TargetRenamer
{
	public sealed class Log
    {
        public static void Setup()
        {
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }
            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.RWSAppStoreTeam, PluginResources.TargetRenamer_Name);
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = PluginResources.TargetRenamer_Name,
                FileName = Path.Combine(logDirectoryPath, $"{PluginResources.TargetRenamer_Name}.txt"),
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            config.AddRuleForAllLevels(target, "*TargetRename*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}