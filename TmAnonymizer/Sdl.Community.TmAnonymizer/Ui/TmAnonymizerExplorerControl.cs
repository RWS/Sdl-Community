using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.Ui
{
	public partial class TmAnonymizerExplorerControl : UserControl
	{
		private readonly AcceptWindow _acceptWindow;
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

			if (!model.TmViewModel.SettingsService.UserAgreed())
			{				
				_acceptWindow = new AcceptWindow(model.TmViewModel.SettingsService);
				_acceptWindow.InitializeComponent();
				_acceptWindow.Closing += AcceptWindow_Closing;
				_acceptWindow.ShowDialog();				
			}
			else
			{
				LoadView();
			}
		}		

		private void AcceptWindow_Closing(object sender, CancelEventArgs e)
		{
			_acceptWindow.Closing -= AcceptWindow_Closing;

			if (_model.TmViewModel.SettingsService.UserAgreed())
			{
				LoadView();
			}
		}

		private void LoadView()
		{
			var wpfMainWindow = new MainExplorerControl(_model);

			wpfMainWindow.InitializeComponent();
			elementHost.Child = wpfMainWindow;
		}	
	}
}
