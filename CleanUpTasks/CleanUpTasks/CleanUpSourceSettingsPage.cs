using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.CleanUpTasks
{
	public class CleanUpSourceSettingsPage : DefaultSettingsPage<CleanUpSourceSettingsControl, CleanUpSourceSettings>
	{
		public override void Save()
		{
			base.Save();
			var control = GetControl() as CleanUpSourceSettingsControl;
			control?.SaveSettings();
		}
	}
}