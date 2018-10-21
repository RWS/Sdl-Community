using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	public partial class TmAnonymizerUserControl : UserControl
	{
		private readonly MainViewModel _model;

		public TmAnonymizerUserControl(MainViewModel model)
		{
			InitializeComponent();

			_model = model;					

			LoadView();			
		}

		private void LoadView()
		{
			var mainWindow = new MainViewControl(_model);

			mainWindow.InitializeComponent();
			elementHost.Child = mainWindow;
		}
	}
}
