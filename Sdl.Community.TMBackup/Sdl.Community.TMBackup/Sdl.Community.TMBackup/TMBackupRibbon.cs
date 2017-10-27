using System;
using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

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
				TMBackupForm tmBackupForm = new TMBackupForm();
				tmBackupForm.ShowDialog();
			}
		}
	}
}
