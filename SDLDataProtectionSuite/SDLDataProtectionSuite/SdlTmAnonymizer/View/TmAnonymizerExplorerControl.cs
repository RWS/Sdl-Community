using System.ComponentModel;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	public partial class TmAnonymizerExplorerControl : System.Windows.Forms.UserControl
	{
		private readonly AcceptView _acceptView;
		private readonly MainViewModel _model;

		public TmAnonymizerExplorerControl(MainViewModel model)
		{
			InitializeComponent();

			if (System.Windows.Application.Current == null)
			{
				new System.Windows.Application();
			}

			if (System.Windows.Application.Current != null)
			{
				System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}

			_model = model;

			if (!model.TranslationMemoryViewModel.SettingsService.UserAgreed())
			{				
				_acceptView = new AcceptView(model.TranslationMemoryViewModel.SettingsService);
				_acceptView.InitializeComponent();
				_acceptView.Closing += AcceptViewClosing;
				_acceptView.Topmost = true;
				_acceptView.ShowDialog();
			}
			else
			{
				LoadView();
			}
		}		

		private void AcceptViewClosing(object sender, CancelEventArgs e)
		{
			_acceptView.Closing -= AcceptViewClosing;

			if (_model.TranslationMemoryViewModel.SettingsService.UserAgreed())
			{
				LoadView();
			}
		}

		private void LoadView()
		{
			var mainWindow = new MainExplorerView(_model);

			mainWindow.InitializeComponent();
			elementHost.Child = mainWindow;
		}	
	}
}
