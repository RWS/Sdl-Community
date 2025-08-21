using System.Drawing;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Qualitivity.TellMe.Actions
{
	public class StopTrackingAction : AbstractTellMeAction
	{
		public StopTrackingAction() => Name = $"{PluginResources.Plugin_Name} Stop Tracking";

		public override string Category =>
			string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.QualitivityStopTimer_Icon;
		public override bool IsAvailable => Tracked.TrackingState != Tracked.TimerState.Stopped;

		public override void Execute()
		{
            ApplicationContext.QualitivityViewController.Activate();
			ApplicationContext.QualitivityViewController.StopTimeTracking();
		}
	}
}