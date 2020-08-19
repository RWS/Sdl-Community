using System.IO;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Service;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_AddReport_Action",
		Name = "ReportsViewer_AddReport_Name",
		Description = "ReportsViewer_AddReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Add"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 9, DisplayType.Large)]
	public class AddReportAction : AbstractViewControllerAction<ReportsViewerController>
	{
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private ReportsViewerController _reportsViewerController;

		protected override void Execute()
		{		
			var settings = GetSettings();
			var view = new AppendReportWindow();
			var viewModel = new AppendReportViewModel(view, new Report(), settings, _pathInfo, _imageService,
				_reportsViewerController.GetSelectedProject());
			view.DataContext = viewModel;
			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.AddReport(viewModel.Report);
			}
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
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
		}
	}
}
