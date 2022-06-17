using CustomViewExample.Model;
using CustomViewExample.Services;
using CustomViewExample.View;
using CustomViewExample.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace CustomViewExample.Ribbon.Actions
{
	[Action("CustomViewExample_OpenSettingsAction",
		Name = "CustomViewExample_OpenSettingsAction_Name",
		Description = "CustomViewExample_OpenSettingsAction_Description",
		ContextByType = typeof(CustomViewExampleController),
		Icon = "CustomViewExample_OpenSettingsAction_Icon"
	)]
	[ActionLayout(typeof(RibbonGroups.ConfigurationGroup), 10, DisplayType.Large)]
	internal class OpenSettingsAction : AbstractAction
	{
		protected override void Execute()
		{
			var pathInfo = new CustomViewPathInfo();
			var settingsService = new SettingsService(pathInfo);

			var viewModel = new SettingsViewModel(settingsService);
			var view = new SettingsView {DataContext = viewModel};

			view.ShowDialog();
		}
	}
}
