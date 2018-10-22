using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	[ViewPart(
		Id = "SdLTmAnonymizerLogViewController",
		Name = "Log Report",
		Icon = "ReportsView",
		Description = "Log Report")]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 0, LocationByType = typeof(TmAnonymizerViewController))]
	public class TmAnonymizerLogViewController : AbstractViewPartController
	{
		private static TmAnonymizerLogViewControl _control;
		private static MainViewModel _model;

		public TmAnonymizerLogViewController() { }

		public TmAnonymizerLogViewController(MainViewModel model)
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
	}
}
