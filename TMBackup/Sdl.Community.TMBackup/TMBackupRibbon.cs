using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.Toolkit.Core;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TMBackup
{
	[RibbonGroup("TM Backup", Name = "TM Backup")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class TMBackupRibbon : AbstractRibbonGroup
	{
		private static readonly List<Environment.SpecialFolder> _pluginFolderLocations = new List<Environment.SpecialFolder>
		{
		   Environment.SpecialFolder.ApplicationData,
		   Environment.SpecialFolder.LocalApplicationData,
		   Environment.SpecialFolder.CommonApplicationData
		};

		[Action("Sdl.Community.TMBackup", Name = "TM Backup", Icon = "TMBackup_Icon", Description = "TM Backup")]
		[ActionLayout(typeof(TMBackupRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class TMBackupAction : AbstractAction
		{
			protected override void Execute()
			{
				MoveBackupFilesAppToDirectory();

				var tmBackupTasksForm = new TMBackupTasksForm();
				tmBackupTasksForm.ShowDialog();
			}
		}

		private static void MoveBackupFilesAppToDirectory()
		{
			var pluginExePath = GetUnpackedFolder();
			if (!string.IsNullOrEmpty(pluginExePath))
			{
				var path = Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe");

				if (!Directory.Exists(Constants.SdlCommunityPath))
				{
					Directory.CreateDirectory(Constants.SdlCommunityPath);
				}

				if (!Directory.Exists(Constants.DeployPath))
				{
					Directory.CreateDirectory(Constants.DeployPath);
				}

				var directoryFiles = new List<string>(Directory.GetFiles(Constants.DeployPath));

				if (!directoryFiles.Contains(path))
				{
					using (var exeStream = new FileStream(pluginExePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						if (exeStream != null)
						{
							var fileStream = File.Create(Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe"), (int)exeStream.Length);
							var assemblyData = new byte[(int)exeStream.Length];
							exeStream.Read(assemblyData, 0, assemblyData.Length);
							fileStream.Write(assemblyData, 0, assemblyData.Length);
							fileStream.Close();
						}
					}
				}
			}
		}

		// Get the Sdl.Community.TMBackup.exe path from the Unpacked folder when plugin is loaded in Studio
		private static string GetUnpackedFolder()
		{
			var executableVersion = new Studio().GetStudioVersion().ExecutableVersion.Major;
			foreach (var pluginFolderLocation in _pluginFolderLocations)
			{
				var unpackedFolder = $@"{Environment.GetFolderPath(pluginFolderLocation)}\SDL\SDL Trados Studio\{executableVersion}\Plugins\Unpacked\{PluginResources.Plugin_Name}";

				if (Directory.Exists(unpackedFolder))
				{
					var pluginExePath = Directory.GetFiles(unpackedFolder, "*.exe").FirstOrDefault().ToString();
					return pluginExePath;
				}
			}
			return string.Empty;
		}
	}
}