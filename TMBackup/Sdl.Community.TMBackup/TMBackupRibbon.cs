using Microsoft.Win32;
using Sdl.Community.BackupService.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sdl.Community.TMBackup
{
	[RibbonGroup("TM Backup", Name = "TM Backup", Description = "TM Backup", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TMBackupRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.TMBackup", Name = "TM Backup", Icon = "TMBackup_Icon", Description = "TM Backup")]
		[ActionLayout(typeof(TMBackupRibbon), 20, DisplayType.Large)]
		public class TMBackupAction : AbstractAction
		{
			protected override void Execute()
			{
			    MoveBackupFilesAppToDirectory();

				TMBackupTasksForm tmBackupTasksForm = new TMBackupTasksForm();
				tmBackupTasksForm.ShowDialog();				
			}
		}
		
		private static void MoveBackupFilesAppToDirectory()
		{
			string path = Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe");

			if(!Directory.Exists(Constants.SdlCommunityPath))
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
				var executingAssembly = Assembly.GetExecutingAssembly();
				var exeStream = executingAssembly.GetManifestResourceStream("Sdl.Community.TMBackup.BackupFilesExe.Sdl.Community.BackupFiles.exe");
				using (exeStream)
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
}
