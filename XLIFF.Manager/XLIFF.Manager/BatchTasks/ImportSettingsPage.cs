using System.Windows;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public class ImportSettingsPage : DefaultSettingsPage<ImportSettingsControl, XliffManagerImportSettings>
	{				
		private ExportSettingsControl _control;

		public override object GetControl()
		{			
			_control = base.GetControl() as ExportSettingsControl;			
			return _control;
		}

		public override bool ValidateInput()
		{
			MessageBox.Show(PluginResources.XLIFFManager_BatchTasks_Import_Description, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
			return false;			
		}
	}


}
