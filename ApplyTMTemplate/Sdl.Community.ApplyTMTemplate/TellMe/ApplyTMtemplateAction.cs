﻿using System.Drawing;
using Sdl.Community.ApplyTMTemplate.Services;
using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMTemplateAction : AbstractTellMeAction
	{
		public ApplyTMTemplateAction()
		{
			Name = "Start Apply TM Template";
		}

		public override string Category => "Apply TM Template results";

		public override Icon Icon => PluginResources.ATTA;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			var timedTextBoxViewModel = new ViewModels.TimedTextBox();
			var mainWindowViewModel = new MainWindowViewModel(new TemplateLoader(), new TMLoader(), new MessageService(), timedTextBoxViewModel);

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(mainWindow);
			mainWindow.Show();
		}
	}
}