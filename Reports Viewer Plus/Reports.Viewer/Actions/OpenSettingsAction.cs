using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Model;
using Reports.Viewer.Plus.Service;
using Reports.Viewer.Plus.View;
using Reports.Viewer.Plus.ViewModel;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
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
			
			var groupNames = reports?.OrderByDescending(b => b.Group).Select(a => a.Group).Distinct().ToList();

			var settings = GetSettings();
			var view = new SettingsWindow();
			var viewModel = new SettingsViewModel(view, settings, _imageService, _pathInfo,
				_reportsViewerController, groupNames);
			view.DataContext = viewModel;
			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.UpdateSettings(viewModel.UpdatedTemplates);
			}
		}

		public void Run()
		{
			Execute();
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public override void Initialize()
		{
			_canEnable = true;
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();

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

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
