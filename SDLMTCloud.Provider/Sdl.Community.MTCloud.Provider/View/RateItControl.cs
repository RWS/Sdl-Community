using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.View
{
	public partial class RateItControl : UserControl
	{
		public IRatingService RatingService { get; private set; }

		public RateItControl(ITranslationService translationService)
		{
			InitializeComponent();

			LoadDataContext(translationService);
		}

		private void LoadDataContext(ITranslationService translationService)
		{
			var shortcutService = new ShortcutService();
			var actionProvider = new ActionProvider();

			var rateItViewModel = new RateItViewModel(translationService, shortcutService, actionProvider);
			var rateItWindow = new RateItView
			{
				DataContext = rateItViewModel
			};

			RatingService = rateItViewModel;

			rateItElementHost.Child = rateItWindow;
		}
	}
}
