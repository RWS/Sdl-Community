using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.BackupService.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;

namespace Sdl.Community.TMBackup
{
	[ApplicationInitializer]
	public class TmBackupInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			Log.Setup();
		}
	}

	[RibbonGroup("Plugin_Name", Name = "Plugin_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class TMBackupRibbon : AbstractRibbonGroup
	{
		private static readonly List<Environment.SpecialFolder> _pluginFolderLocations = new List<Environment.SpecialFolder>
		{
		   Environment.SpecialFolder.ApplicationData,
		   Environment.SpecialFolder.LocalApplicationData,
		   Environment.SpecialFolder.CommonApplicationData
		};

		[Action("TMBackup", Name = "Plugin_Name", Icon = "TMBackup_Icon", Description = "Plugin_Name")]
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
            var executableVersion = new StudioVersionService().GetStudioVersion()?.ExecutableVersion?.Major.ToString();
            var executableBetaVersion = new StudioVersionService().GetStudioVersion()?.ExecutableVersion?.Major+"Beta";

            var pluginExePath = GetUnpackedFolder(executableVersion);
            if (string.IsNullOrEmpty(pluginExePath))
            {
                pluginExePath = GetUnpackedFolder(executableBetaVersion);
            }

			if (!string.IsNullOrEmpty(pluginExePath))
			{
				var path = Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe");

				Directory.CreateDirectory(Constants.SdlCommunityPath);
				Directory.CreateDirectory(Constants.DeployPath);

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
		private static string GetUnpackedFolder(string executableVersion)
		{
			foreach (var pluginFolderLocation in _pluginFolderLocations)
			{
				var devUnpackedFolder = $@"{Environment.GetFolderPath(pluginFolderLocation)}\Trados\Trados Studio\{executableVersion}\Plugins\Unpacked\Sdl.Community.TMBackup";
				var productionUnpackedFolder = $@"{Environment.GetFolderPath(pluginFolderLocation)}\Trados\Trados Studio\{executableVersion}\Plugins\Unpacked\{PluginResources.Plugin_Name}";
				
				var unpackedFolder = Directory.Exists(devUnpackedFolder) ? devUnpackedFolder : productionUnpackedFolder;
				if (Directory.Exists(unpackedFolder))
				{
					var pluginExePath = Directory.GetFiles(unpackedFolder, "*.exe").FirstOrDefault();
					return pluginExePath;
				}
			}
			return string.Empty;
		}
	}
}