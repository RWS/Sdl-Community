using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace Sdl.Community.TMBackup
{
	// To do: change the plugin location to the Welcome view
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
				TMBackupForm tmBackupForm = new TMBackupForm();
				tmBackupForm.ShowDialog();
			}
		}
	}

	// To do: change the plugin location to the TM Maintenance
	[RibbonGroup("TM Backup", Name = "TM Backup", Description = "TM Backup", ContextByType = typeof(FilesController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class TMBackupRibbon2 : AbstractRibbonGroup
	{
		[Action("Sdl.Community.TMBackup2", Name = "TM Backup", Icon = "TMBackup_Icon", Description = "TM Backup")]
		[ActionLayout(typeof(TMBackupRibbon2), 20, DisplayType.Large)]
		public class TMBackupAction2 : AbstractAction
		{
			protected override void Execute()
			{
				TMBackupForm tmBackupForm = new TMBackupForm();
				tmBackupForm.ShowDialog();
			}
		}
	}
}
