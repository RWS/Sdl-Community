using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;
using Sdl.Community.Reports.Viewer.Controls;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.TestData;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Reports.Viewer
{
	[View(
		Id = "SDLReportsViewer_View",
		Name = "SDLReportsViewer_Name",
		Description = "SDLReportsViewer_Description",
		Icon = "ReportsView",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class ReportsViewerController : AbstractViewController
	{
		private List<Report> _reports;
		private ReportViewModel _reportViewModel;
		private ReportsNavigationViewModel _reportsNavigationViewModel;
		private ReportViewControl _reportViewControl;
		private ReportsNavigationViewControl _reportsNavigationViewControl;
		private ProjectsController _projectsController;
		private ReportView _reportView;
		private ReportsNavigationView _reportsNavigationView;
		//private DataView _dataView;
		//private BrowserView _browserView;
		//private DataViewModel _dataViewModel;
		//private BrowserViewModel _browserViewModel;
		

		protected override void Initialize(IViewContext context)
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;

			var testDataUtil = new TestDataUtil();
			_reports = testDataUtil.GetTestReports();
			if (_reports.Count > 0)
			{
				_reports[0].IsSelected = true;
			}

			InitializeViews();
		}
		
		protected override Control GetExplorerBarControl()
		{
			return _reportsNavigationViewControl ?? (_reportsNavigationViewControl = new ReportsNavigationViewControl());
		}

		protected override Control GetContentControl()
		{
			return _reportViewControl ?? (_reportViewControl = new ReportViewControl());
		}

		public EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		private void InitializeViews()
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(delegate
			{			
				_reportViewModel = new ReportViewModel();
				_reportView = new ReportView();
				_reportView.DataContext = _reportViewModel;

				_reportsNavigationViewModel = new ReportsNavigationViewModel(_reports, _projectsController);
				_reportsNavigationViewModel.ReportSelectionChanged += OnReportSelectionChanged;
				_reportsNavigationViewModel.ReportViewModel = _reportViewModel;

				_reportsNavigationView = new ReportsNavigationView(_reportsNavigationViewModel);

				_reportViewControl.UpdateViewModel(_reportView);
				_reportsNavigationViewControl.UpdateViewModel(_reportsNavigationView);
				
			}));
		}


		private void OnReportSelectionChanged(object sender, ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			// TODO load project reports into the viewer

			//var updated = AddNewProjectToContainer(_projectsController?.CurrentProject);
			//if (!updated)
			//{
			//	updated = UnloadRemovedProjectsFromContainer();
			//}

			//if (updated && _projectsNavigationViewModel != null)
			//{
			//	_projectsNavigationViewModel.Projects = new List<Project>();
			//	_projectsNavigationViewModel.Projects = _xliffProjects;
			//}
		}

	}
}
