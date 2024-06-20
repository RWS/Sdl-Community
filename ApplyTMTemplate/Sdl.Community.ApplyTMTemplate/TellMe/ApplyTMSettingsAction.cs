using System.Drawing;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Services;
using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMSettingsAction : AbstractTellMeAction
	{
		public ApplyTMSettingsAction()
		{
			Name = "applyTM Template Settings";
		}

		public override string Category => "applyTM Template results";

		public override Icon Icon => PluginResources.TellMe_Settings;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			var timedTextBoxViewModel = new ViewModels.TimedTextBox();
			var mainWindowViewModel = new MainWindowViewModel(new LanguageResourcesAdapter(), new ResourceManager(new ExcelResourceManager(), new MessageService()), new TmLoader(), new MessageService(), timedTextBoxViewModel, new FilePathDialogService());

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(mainWindow);
			mainWindow.ShowDialog();
		}
	}
}