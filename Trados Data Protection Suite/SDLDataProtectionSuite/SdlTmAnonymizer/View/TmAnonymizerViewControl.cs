﻿using System.Windows.Forms;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	public partial class TmAnonymizerViewControl : UserControl, IUIControl
	{
		private readonly MainViewModel _model;

		public TmAnonymizerViewControl(MainViewModel model)
		{
			InitializeComponent();

			_model = model;					

			LoadView();			
		}

		private void LoadView()
		{
			var mainWindow = new MainView(_model);

			mainWindow.InitializeComponent();
			elementHost.Child = mainWindow;
		}
	}
}
