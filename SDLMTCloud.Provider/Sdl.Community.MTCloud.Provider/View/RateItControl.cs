using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.MTCloud.Provider.View
{
	public partial class RateItControl : UserControl, IUIControl
	{
		private RateItView _rateItWindow;
		public IRatingService RatingService { get; private set; }

		public RateItControl()
		{
			InitializeComponent();
			LoadDataContext();
		}

		public void FocusFeedbackTextBox()
		{
			_rateItWindow.FocusFeedbackTextBox();
		}

		private void LoadDataContext()
		{
			var versionService = new VersionService();
			var messageBoxService = new MessageBoxService();
			var shortcutService = new ShortcutService(versionService, messageBoxService);
			var actionProvider = new ActionProvider();


			var editorController = MtCloudApplicationInitializer.EditorController;

			var rateItViewModel = new RateItViewModel(shortcutService, actionProvider, MtCloudApplicationInitializer.SegmentSupervisor, messageBoxService,
				editorController);
			_rateItWindow = new RateItView
			{
				DataContext = rateItViewModel
			};

			RatingService = rateItViewModel;

			rateItElementHost.Child = _rateItWindow;
		}
	}
}
