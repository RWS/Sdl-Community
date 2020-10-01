using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Transcreate.BatchTasks
{
	public class ConvertSettingsPage : DefaultSettingsPage<ConvertSettingsControl, SDLTranscreateConvertSettings>
	{
		private ConvertSettingsControl _control;

		public override object GetControl()
		{
			_control = base.GetControl() as ConvertSettingsControl;
			return _control;
		}
	}
}
