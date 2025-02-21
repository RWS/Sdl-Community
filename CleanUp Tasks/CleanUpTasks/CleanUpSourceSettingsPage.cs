using Sdl.Community.CleanUpTasks;
using Sdl.Desktop.IntegrationApi;

namespace SDLCommunityCleanUpTasks
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