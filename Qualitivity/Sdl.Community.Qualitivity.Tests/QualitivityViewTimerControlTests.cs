using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Timers;
using Sdl.Community.Qualitivity.Tracking;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class QualitivityViewTimerControlTests
	{
		private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		[Fact]
		public void Constructor_WhileTracking_KeepsTracking()
		{
			GivenTracker(Tracked.TimerState.Started);

			using (new QualitivityViewTimerControl())
			{
				Assert.Equal(Tracked.TimerState.Started, Tracked.TrackingState);
			}
		}

		[Theory]
		[InlineData(Tracked.TimerState.None, true, false)]
		[InlineData(Tracked.TimerState.Stopped, true, false)]
		[InlineData(Tracked.TimerState.Started, false, true)]
		public void FirstUpdate_SetsButtonsFromTrackingState(Tracked.TimerState state, bool startEnabled, bool stopEnabled)
		{
			GivenTracker(state);

			using (var control = new QualitivityViewTimerControl())
			{
				typeof(QualitivityViewTimerControl).GetMethod("update_control_4ActivityViewerArea", Instance).Invoke(control, null);

				Assert.Equal(startEnabled, Button(control, "toolStripButton_start").Enabled);
				Assert.Equal(stopEnabled, Button(control, "toolStripButton_stop").Enabled);
			}
		}

		private static void GivenTracker(Tracked.TimerState state)
		{
			Tracked.Reset();
			Tracked.ActiveDocument = null;
			Tracked.TrackingState = state;
		}

		private static ToolStripButton Button(QualitivityViewTimerControl control, string name) =>
			(ToolStripButton)typeof(QualitivityViewTimerControl).GetField(name, Instance).GetValue(control);
	}
}
