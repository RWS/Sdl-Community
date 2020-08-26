using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Service;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_OpenSettings_Action",
		Name = "ReportsViewer_OpenSettings_Name",
		Description = "ReportsViewer_OpenSettings_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Settings"
	)]
	[ActionLayout(typeof(ReportsViewerSettingsGroups), 10, DisplayType.Large)]
	public class OpenSettingsAction : BaseReportAction
	{
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			var reports = _reportsViewerController.GetReports();
			var groupNames = reports.OrderByDescending(b => b.Group).Select(a => a.Group).Distinct().ToList();

			var settings = GetSettings();
			var view = new SettingsWindow();
			var viewModel = new SettingsViewModel(view, settings, _imageService, _pathInfo, 
				_reportsViewerController.ReportsController, groupNames, _reportsViewerController.ClientId);
			view.DataContext = viewModel;
			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.UpdateSettings();
			}
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
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
			_canEnable = true;
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();

			SetEnabled();
		}		

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
