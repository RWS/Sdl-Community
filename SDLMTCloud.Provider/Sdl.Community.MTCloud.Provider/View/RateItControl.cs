using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.MTCloud.Provider.View
{
	public partial class RateItControl : UserControl
	{
		public IRatingService RatingService { get; private set; }

		public RateItControl()
		{
			InitializeComponent();

			LoadDataContext();
		}

		private void LoadDataContext()
		{
			var shortcutService = new ShortcutService();
			var actionProvider = new ActionProvider();

			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var segmentSupervisor = new SegmentSupervisor(editorController);

			var rateItViewModel = new RateItViewModel(shortcutService, actionProvider, segmentSupervisor);
			var rateItWindow = new RateItView
			{
				DataContext = rateItViewModel
			};

			RatingService = rateItViewModel;

			rateItElementHost.Child = rateItWindow;
		}
	}
}
