using System.Windows.Forms;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio
{
	[ViewPart(
		Id = "SDLTMAnonymizerLogViewPart",
		Name = "Log Report",
		Icon = "ReportsView",
		Description = "Log Report")]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 0, LocationByType = typeof(SDLTMAnonymizerView))]
	public class SDLTMAnonymizerLogViewPart : AbstractViewPartController
	{
		private static TmAnonymizerLogViewControl _control;
		private static MainViewModel _model;

		public SDLTMAnonymizerLogViewPart() { }

		public SDLTMAnonymizerLogViewPart(MainViewModel model)
		{
			_model = model;
		}

		protected override void Initialize()
		{
			_control = new TmAnonymizerLogViewControl(_model);			
		}

		protected override Control GetContentControl()
		{
			return _control;
		}

		public Control Control => _control;


	}
}
