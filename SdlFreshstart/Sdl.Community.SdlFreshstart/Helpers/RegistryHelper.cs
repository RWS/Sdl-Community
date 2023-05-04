using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class RegistryHelper : IRegistryHelper
	{
		private const string MultitermRegistryPath = @"Software\Trados\MultiTerm";
		private const string TradosStudioRegistryPath = @"Software\Trados\Trados Studio";
		private RegistryKey BaseKey { get; } = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);

		public async Task BackupKeys(List<LocationDetails> locations)
		{
			foreach (var location in locations)
			{
				var path = $@"{location.BackupFilePath}";

				Directory.CreateDirectory(Path.GetDirectoryName(path));

				var proc = new Process
				{
					StartInfo = { FileName = "regedit.exe", UseShellExecute = false, Verb = "RunAs" }
				};

				try
				{
					await Task.Run(() =>
					{
						proc = Process.Start("regedit",
							$"/e \"{path}\" \"{location.OriginalPath}\" ");
						proc?.WaitForExit();
					});
				}
				catch (Exception)
				{
					proc?.Dispose();
				}
			}
		}

		public void DeleteKeys(List<LocationDetails> locations, bool tradosKeys)
		{
			var errorMessages = new List<string>();
			foreach (var location in locations)
			{
				var subKey = Path.GetFileName(location.OriginalPath);
				var productRegistryPath = location.OriginalPath.Replace(subKey, "");
				var productRegistry = BaseKey.OpenSubKey(@$"Software\{productRegistryPath}\", true);

				try
				{
					productRegistry?.DeleteSubKeyTree(subKey);
				}
				catch (Exception ex)
				{
					errorMessages.Add($"{ex.Message}: {productRegistry}{subKey}");
				}
			}

			ThrowAggregateExceptions(errorMessages);
		}

		public async Task RestoreKeys(List<LocationDetails> pathsToKeys)
		{
			var errorMessages = new List<string>();
			foreach (var pathToKey in pathsToKeys)
			{
				try
				{
					await Task.Run(() =>
					{
						var regeditProcess = Process.Start("regedit.exe", $"/s \"{pathToKey.BackupFilePath}\"");
						regeditProcess?.WaitForExit();
					});
				}
				catch (Exception ex)
				{
					errorMessages.Add($"{ex.Message}: {pathToKey.BackupFilePath}");
				}
			}

			ThrowAggregateExceptions(errorMessages);
		}

		private static void ThrowAggregateExceptions(List<string> errorMessages)
		{
			if (errorMessages.Count > 0)
				throw new Exception(string.Join(Environment.NewLine, errorMessages));
		}
	}
}