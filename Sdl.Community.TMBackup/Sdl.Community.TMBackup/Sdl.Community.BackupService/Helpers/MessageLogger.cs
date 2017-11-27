using System;
using System.IO;
using System.Text;

namespace Sdl.Community.BackupService.Helpers
{
	public class MessageLogger
	{
		public static void LogFileMessage(string exceptionMessage)
		{
			string path = Path.Combine(Constants.DeployPath, "Log.txt");

			if (!File.Exists(path))
			{
				CreateLogFileMessage(path, exceptionMessage);
			}
			else
			{
				File.Delete(path);
				CreateLogFileMessage(path, exceptionMessage);
			}
		}

		private static void CreateLogFileMessage(string path, string exceptionMessage)
		{
			// Create the file and write information.
			using (FileStream fs = File.Create(path))
			{
				Byte[] info = new UTF8Encoding(true).GetBytes(string.Concat(Constants.InformativeErrorMessage, Environment.NewLine, exceptionMessage));
				fs.Write(info, 0, info.Length);
			}
		}		
	}
}