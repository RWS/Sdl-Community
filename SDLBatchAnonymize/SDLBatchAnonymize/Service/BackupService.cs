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
		    if (!Directory.Exists(_backupDirectoryPath))
		    {
			    Directory.CreateDirectory(_backupDirectoryPath);
		    }
		    try
		    {
			    var zipPath = Path.Combine(_backupDirectoryPath, $"{projectName}.zip");
				ZipFile.CreateFromDirectory(projectPath,zipPath);
		    }
		    catch (Exception exception)
		    {
				_logger.Error($"{exception.Message}\n {exception.StackTrace}");
			}
		}
    }
}
