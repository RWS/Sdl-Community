using System;
using System.IO;
using System.IO.Compression;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
    public class BackupService
    {
	    private readonly string _backupDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "SDLBatchAnonymizer","Projects Backup");
	    public static readonly Log Log = Log.Instance;

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
				Log.Logger.Error($"{exception.Message}\n {exception.StackTrace}");
			}
		}
    }
}
