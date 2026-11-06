using System.Windows.Forms;
using Sdl.Community.Qualitivity.Tracking;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class TrackedActionsTests
	{
		[Fact]
		public void PauseTracking_UnpausingWithNoActiveDocument_RunsTimer()
		{
			Tracked.Reset();
			Tracked.TrackingState = Tracked.TimerState.Paused;
			Tracked.ActiveDocument = null;

			using (var timer = new Timer())
			{
				TrackedActions.pause_tracking(null, timer);
			}

			Assert.Equal(Tracked.TimerState.Started, Tracked.TrackingState);
			Assert.True(Tracked.TrackingTimer.IsRunning);
		}
	}
}