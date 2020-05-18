using System.Windows;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager.Actions
{

	[Action("XLIFFManager_ExportToXLIFF_Action", typeof(XLIFFManagerViewController), 
		Name = "XLIFFManager_ExportToXLIFF_Name", 
		Icon = "ExportTo", 
		Description = "XLIFFManager_ExportToXLIFF_Description")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 6, DisplayType.Large)]
	public class ExportToXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export);
			wizardService.ShowWizard();
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}

	[Action("XLIFFManager_FilesContextMenu_ExportToXLIFF_Action", typeof(FilesController), 
		Name = "XLIFFManager_ContextMenu_ExportToXLIFF_Name", 
		Icon = "ExportTo", 
		Description = "XLIFFManager_ContextMenu_ExportToXLIFF_Description")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 8, DisplayType.Default, "", true)]
	public class XLIFFManagerFilesContextMenuExportToXLIFFAction : AbstractAction
	{
		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export);
			wizardService.ShowWizard();
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}

	[Action("XLIFFManager_ProjectsContextMenu_ExportToXLIFF_Action", typeof(ProjectsController),
		Name = "XLIFFManager_ContextMenu_ExportToXLIFF_Name",
		Icon = "ExportTo",
		Description = "XLIFFManager_ContextMenu_ExportToXLIFF_Description")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 8, DisplayType.Default, "", true)]
	public class XLIFFManagerProjectsContextMenuExportToXLIFFAction : AbstractAction
	{
		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export);
			wizardService.ShowWizard();
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}
}
