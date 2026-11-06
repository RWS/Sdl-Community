using System.Reflection;
using System.Windows.Forms;
using Xunit;
using SettingsDialog = Sdl.Community.Qualitivity.Dialogs.Settings;
using TrackerSettings = Sdl.Community.Structures.Configuration.Settings;

namespace Sdl.Community.Qualitivity.Tests
{
	public class SettingsDialogTests
	{
		private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		[Theory]
		[InlineData("1", 2)]
		[InlineData("2", 2)]
		[InlineData("61", 60)]
		public void InitializeActivityTracking_ShowsIdleMinutesWithinLimits(string stored, int shown)
		{
			var settings = new TrackerSettings();
			settings.GetTrackingProperty("idleTimeOutMinutes").Value = stored;

			using (var dialog = new SettingsDialog { settings = settings })
			{
				var type = typeof(SettingsDialog);
				type.GetMethod("initialize_activity_tracking", Instance).Invoke(dialog, null);

				var minutes = (NumericUpDown)type.GetField("numericUpDown_idleTimeOutMinutes", Instance).GetValue(dialog);
				Assert.Equal(shown, minutes.Value);
			}
		}
	}
}