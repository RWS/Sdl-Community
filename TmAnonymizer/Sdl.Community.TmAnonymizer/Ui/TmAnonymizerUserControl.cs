using System;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.Ui
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
			var wpfMainWindow = new MainViewControl(_model);

			wpfMainWindow.InitializeComponent();
			elementHost.Child = wpfMainWindow;
		}
	}
}
