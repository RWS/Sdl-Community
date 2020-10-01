using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Transcreate.BatchTasks
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsControl, SDLTranscreateExportSettings>
	{
		private ExportSettingsControl _control;

		public override object GetControl()
		{
			_control = base.GetControl() as ExportSettingsControl;
			return _control;
		}
	}
}
