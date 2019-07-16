using System.Windows.Forms.Integration;
using Sdl.Community.TuToTm.View;
using Sdl.Community.TuToTm.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.TuToTm
{
	[RibbonGroup("TuToTm", Name = "")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class TuToTmRibbon : AbstractRibbonGroup
	{
	}

	[Action("TuToTm",
		Name = "TU to TM",
		Icon = "tu2tm",
		Description = "TU to TM")]
	[ActionLayout(typeof(TuToTmRibbon), 20, DisplayType.Large)]
	public class TuToTmAction:AbstractAction
	{
		protected override void Execute()
		{
			var viewModel = new MainWindowViewModel();
			var window = new MainWindow
			{
				DataContext = viewModel
			};
			ElementHost.EnableModelessKeyboardInterop(window);
			window.Show();
		}
	}
}
