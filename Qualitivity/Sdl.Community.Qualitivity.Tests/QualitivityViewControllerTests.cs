using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Desktop.IntegrationApi;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class QualitivityViewControllerTests
	{
		[Fact]
		public void Initialize_CalledByStudioAfterStartup_KeepsLiveTracking()
		{
			Tracked.TrackingState = Tracked.TimerState.Started;
			var controller = new StudioInitializedController { IsInitialized = true };

			var error = Record.Exception(controller.InitializeFromStudio);

			Assert.Equal(Tracked.TimerState.Started, Tracked.TrackingState);
			Assert.Null(error);
		}

		private sealed class StudioInitializedController : QualitivityViewController
		{
			public void InitializeFromStudio() => Initialize((IViewContext)null);
		}
	}
}