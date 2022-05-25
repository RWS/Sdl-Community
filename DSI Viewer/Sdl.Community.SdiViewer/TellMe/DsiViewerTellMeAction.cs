using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DsiViewer.TellMe
{
	public abstract class DsiViewerAbstractTellMeAction : AbstractTellMeAction
	{
		public override string Category => "DSI Viewer results";
		public override bool IsAvailable => true;
	}
}