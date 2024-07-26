using System.IO;
using Newtonsoft.Json;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_OpenSettings_Action", typeof(XLIFFManagerViewController), 
		Name = "XLIFFManager_Settings_Name", 
		Icon = "Settings", 
		Description = "XLIFFManager_Settings_Description")]
	[ActionLayout(typeof(XLIFFManagerSettingsGroup), 7, DisplayType.Large)]
	public class OpenSettingsAction: AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private PathInfo _pathInfo;

		protected override void Execute()
		{
			var settings = GetSettings();
			var view = new SettingsWindow();			
			var viewModel = new SettingsViewModel(view, settings, _pathInfo);
			view.DataContext = viewModel;
			view.ShowDialog();
		}

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		public override void Initialize()
		{
			Enabled = true;
			_pathInfo = new PathInfo();
		}
	}
}
