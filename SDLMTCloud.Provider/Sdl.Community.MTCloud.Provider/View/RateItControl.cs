using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

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
			var shortcutService = new ShortcutService();
			var actionProvider = new ActionProvider();
			var messageBoxService = new MessageBoxService();

			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var segmentSupervisor = new SegmentSupervisor(editorController);
			var eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

			var rateItViewModel = new RateItViewModel(shortcutService, actionProvider, segmentSupervisor, messageBoxService,
				editorController, eventAggregator);
			_rateItWindow = new RateItView
			{
				DataContext = rateItViewModel
			};

			RatingService = rateItViewModel;

			rateItElementHost.Child = _rateItWindow;
		}
	}
}
