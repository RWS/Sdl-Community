using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Services;
using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ApplyTMTemplate.Studio
{
	[Action("ApplyTMTemplateAction", Icon = "ATTA", Name = "Apply TM Template", Description = "Applies settings from a TM template to a TM")]
	[ActionLayout(typeof(ApplyTMTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ApplyTMTemplateAction : AbstractAction
	{
		protected override void Execute()
		{
			var timedTextBox = new ViewModels.TimedTextBox();
			var mainWindowViewModel = new MainWindowViewModel(new LanguageResourcesAdapter(), new ResourceManager(new ExcelResourceManager(), new MessageService()), new TmLoader(), new MessageService(), timedTextBox, new FilePathDialogService(), new ApplyTMSettingsManager());

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(mainWindow);
			mainWindow.ShowDialog();
		}
	}
}