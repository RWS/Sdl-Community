using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Transcreate.BatchTasks
{
	public class ImportSettingsPage : DefaultSettingsPage<ImportSettingsControl, SDLTranscreateImportSettings>
	{				
		private ImportSettingsControl _control;

		public override object GetControl()
		{			
			_control = base.GetControl() as ImportSettingsControl;			
			return _control;
		}
	}
}
