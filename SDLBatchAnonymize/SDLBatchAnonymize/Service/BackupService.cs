using System;
using System.IO;
using System.IO.Compression;
using NLog;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class BackupService
	{
		private readonly string _backupDirectoryPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.AppStoreFolder,
				PluginResources.Plugin_Name, "Projects Backup");

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public void BackupProject(string projectPath, string projectName)
		{
			_logger.Info(PluginResources.LogMessage_BackingUpProject);
			if (!Directory.Exists(_backupDirectoryPath))
			{
				Directory.CreateDirectory(_backupDirectoryPath);
			}
			try
			{
				var zipPath = Path.Combine(_backupDirectoryPath, $"{projectName}_{GetDateTimeToString()}.zip");
				ZipFile.CreateFromDirectory(projectPath, zipPath);
				_logger.Info($"Backed up at {zipPath}");
			}
			catch (Exception exception)
			{
				_logger.Error($"{exception.Message}\n {exception.StackTrace}");
			}
		}

		private static string GetDateTimeToString()
		{
			var dateTime = DateTime.Now;
			var value = dateTime.Year +
						dateTime.Month.ToString().PadLeft(2, '0') +
						dateTime.Day.ToString().PadLeft(2, '0') +
						"-" +
						dateTime.Hour.ToString().PadLeft(2, '0') +
						dateTime.Minute.ToString().PadLeft(2, '0') +
						dateTime.Second.ToString().PadLeft(2, '0');
			return value;
		}
	}
}