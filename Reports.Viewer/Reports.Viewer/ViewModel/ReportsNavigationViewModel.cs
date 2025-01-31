using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Actions;
using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.CustomEventArgs;
using Reports.Viewer.Plus.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Task = System.Threading.Tasks.Task;

namespace Reports.Viewer.Plus.ViewModel
{
	public class ReportsNavigationViewModel : ReportsViewModelBase, INotifyPropertyChanged, IDisposable
	{
		private readonly PathInfo _pathInfo;
		private List<Report> _reports;
		private string _filterString;
		private List<Report> _filteredReports;
		private Report _selectedReport;
		private bool _isReportSelected;
		private ObservableCollection<ReportGroup> _reportGroups;
		private IProject _selectedProject;
		private GroupType _groupType;
		private List<GroupType> _groupTypes;
		private string _projectLocalFolder;
		private bool _isLoading;
		private List<ReportState> _previousReportStates;
		private ICommand _expandAllCommand;
		private ICommand _collapseAllCommand;
		private ICommand _clearFilterCommand;
		private ICommand _selectedItemChangedCommand;
		private ICommand _dragDropCommand;
		private ICommand _printPreviewCommand;
		private ICommand _pageSetupCommand;
		private ICommand _mouseDoubleClick;

		public ReportsNavigationViewModel(List<Report> reports, Settings settings, PathInfo pathInfo, IProject selectedProject)
		{
			_reports = reports;
			_filteredReports = _reports;

			Settings = settings;
			_pathInfo = pathInfo;
			_filterString = string.Empty;

			SelectedProject = selectedProject;

			_groupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();

			Task.Run(() => ApplyFilter(true));
		}

		public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public ICommand ExpandAllCommand => _expandAllCommand ?? (_expandAllCommand = new CommandHandler(ExpandAll));

		public ICommand CollapseAllCommand => _collapseAllCommand ?? (_collapseAllCommand = new CommandHandler(CollapseAll));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ?? (_selectedItemChangedCommand = new CommandHandler(SelectedItemChanged));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand PrintPreviewCommand => _printPreviewCommand ?? (_printPreviewCommand = new CommandHandler(PrintPreview));

		public ICommand PageSetupCommand => _pageSetupCommand ?? (_pageSetupCommand = new CommandHandler(PageSetup));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public void AddReports(List<Report> reports)
		{
			foreach (var report in reports)
			{
				if (!_reports.Exists(a => a.Id == report.Id))
				{
					_reports.Add(report);
				}
			}

			Task.Run(() => ApplyFilter(false));
		}

		public void UpdateReports(List<Report> reports)
		{
			foreach (var updatedReport in reports)
			{
				var report = _reports.FirstOrDefault(a => a.Id == updatedReport.Id);
				if (report == null)
				{
					continue;
				}

				report.Path = updatedReport.Path;
				report.XsltPath = updatedReport.XsltPath;
				report.Name = updatedReport.Name;
				report.Description = updatedReport.Description;
				report.Language = updatedReport.Language;
				report.Group = updatedReport.Group;
			}

			Task.Run(() => ApplyFilter(false));
		}

		public void DeleteReports(List<Report> reports)
		{
			foreach (var report in reports)
			{
				_reports.RemoveAll(a => a.Id == report.Id);
			}

			Task.Run(() => ApplyFilter(false));
		}

		public void RefreshView(Settings settings, List<Report> reports)
		{
			ClearVisibleReports();

			Settings = settings;
			_groupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();
			OnPropertyChanged(nameof(GroupType));

			_reports = reports;

			Task.Run(() => ApplyFilter(true));
		}

		public void ClearReports()
		{
			_previousReportStates = _reportGroups?.Count > 0 ? GetReportStates(_reportGroups) : _previousReportStates;
			_reports = new List<Report>();
			_reportGroups?.Clear();
			OnPropertyChanged(nameof(ReportGroups));
		}

		public Settings Settings { get; set; }

		public ReportViewModel ReportViewModel { get; internal set; }

		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				if (_isLoading == value)
				{
					return;
				}

				_isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		public override string ProjectLocalFolder
		{
			get => _projectLocalFolder;
			set
			{
				if (_projectLocalFolder == value)
				{
					return;
				}

				_projectLocalFolder = value;
				OnPropertyChanged(nameof(ProjectLocalFolder));

				if (ReportViewModel != null)
				{
					ReportViewModel.ProjectLocalFolder = ProjectLocalFolder;
				}
			}
		}

		public IProject SelectedProject
		{
			get { return _selectedProject; }
			set
			{
				_selectedProject = value;
				OnPropertyChanged(nameof(SelectedProject));

				if (ReportViewModel != null)
				{
					ReportViewModel.SelectedProject = SelectedProject;
				}
			}
		}

		public List<Report> Reports
		{
			get => _reports;
			private set
			{
				_reports = value;
				OnPropertyChanged(nameof(Reports));
			}
		}

		public string FilterString
		{
			get => _filterString;
			set
			{
				if (_filterString == value)
				{
					return;
				}

				_filterString = value;
				OnPropertyChanged(nameof(FilterString));

				Task.Run(() => ApplyFilter(false));
			}
		}

		private async void ApplyFilter(bool expandAll)
		{
			_filteredReports = string.IsNullOrEmpty(_filterString)
				? _reports
				: _reports.Where(a => a.Name.ToLower().Contains(_filterString.ToLower())).ToList();


			var reportGroups = expandAll ? ExpandAll(await BuildReportGroup()) : await BuildReportGroup();
			_reportGroups = new ObservableCollection<ReportGroup>(reportGroups);


			OnPropertyChanged(nameof(ReportGroups));
			OnPropertyChanged(nameof(StatusLabel));
		}

		public ObservableCollection<ReportGroup> ReportGroups
		{
			get => _reportGroups;
			private set
			{
				_reportGroups = value;
				OnPropertyChanged(nameof(ReportGroups));
			}
		}

		public override Report SelectedReport
		{
			get => _selectedReport;
			set
			{
				_selectedReport = value;
				OnPropertyChanged(nameof(SelectedReport));

				ReportViewModel?.UpdateReport(_selectedReport);

				ReportSelectionChanged?.Invoke(this, new ReportSelectionChangedEventArgs
				{
					SelectedReport = _selectedReport,
					SelectedReports = _selectedReport != null ? new List<Report> { _selectedReport } : null
				});

				IsReportSelected = _selectedReport != null;
			}
		}

		public GroupType GroupType
		{
			get => _groupType;
			set
			{
				if (_groupType == value)
				{
					return;
				}

				_groupType = value;

				OnPropertyChanged(nameof(GroupType));

				UpdateSettings();
				Task.Run(() => ApplyFilter(true));
			}
		}

		public List<GroupType> GroupTypes
		{
			get
			{
				return _groupTypes ?? (_groupTypes = new List<GroupType>
				{
					new GroupType
					{
						Name = PluginResources.GroupType_GroupName,
						Type = "Group"
					},
					new GroupType
					{
						Name = PluginResources.GroupType_Language,
						Type = "Language"
					},
				});
			}
			set
			{
				_groupTypes = value;
				OnPropertyChanged(nameof(GroupTypes));
			}
		}

		public bool IsReportSelected
		{
			get => _isReportSelected;
			set
			{
				if (_isReportSelected == value)
				{
					return;
				}

				_isReportSelected = value;
				OnPropertyChanged(nameof(IsReportSelected));
			}
		}

		public string StatusLabel
		{
			get
			{
				//var message = string.Format(PluginResources.StatusLabel_Selected_0, _selectedProjects?.Count);
				//return message;
				return string.Empty;
			}
		}

		public object SelectedItem { get; set; }

		public string GetSelectedLanguage()
		{
			if (SelectedItem is Report report)
			{
				return report.Language;
			}

			if (SelectedItem is GroupItem groupItem)
			{
				return groupItem.Reports?.FirstOrDefault()?.Language;
			}

			if (SelectedItem is ReportGroup reportGroup)
			{
				return reportGroup.GroupItems.FirstOrDefault()?.Reports?.FirstOrDefault()?.Language;
			}

			return string.Empty;
		}

		public string GetSelectedGroup()
		{
			if (SelectedItem is Report report)
			{
				return report.Group;
			}

			if (SelectedItem is GroupItem groupItem)
			{
				return groupItem.Reports?.FirstOrDefault()?.Group;
			}

			if (SelectedItem is ReportGroup reportGroup)
			{
				return reportGroup.GroupItems.FirstOrDefault()?.Reports?.FirstOrDefault()?.Group;
			}

			return string.Empty;
		}

		private async Task<List<ReportGroup>> BuildReportGroup()
		{
			var reportGroups = new List<ReportGroup>();
			if (_filteredReports == null)
			{
				return reportGroups;
			}

			_previousReportStates = _reportGroups?.Count > 0 ? GetReportStates(_reportGroups) : _previousReportStates;

			var orderedReports = _groupType.Type == "Group"
				? _filteredReports.OrderBy(a => a.Group ?? string.Empty).ThenBy(a => a.Language ?? string.Empty).ThenByDescending(a => a.Date).ToList()
				: _filteredReports.OrderBy(a => a.Language ?? string.Empty).ThenBy(a => a.Group ?? string.Empty).ThenByDescending(a => a.Date).ToList();

			foreach (var report in orderedReports)
			{
				var reportGroup = reportGroups.FirstOrDefault(a =>
					string.Compare(a.Name ?? string.Empty, (_groupType.Type == "Group" ? report.Group ?? string.Empty : report.Language ?? string.Empty), StringComparison.CurrentCultureIgnoreCase) == 0);
				if (reportGroup != null)
				{
					var groupItem = reportGroup.GroupItems.FirstOrDefault(a =>
						string.Compare(a.Name ?? string.Empty, (_groupType.Type == "Group" ? report.Language ?? string.Empty : report.Group ?? string.Empty), StringComparison.CurrentCultureIgnoreCase) == 0);
					if (groupItem != null)
					{
						var reportState = _previousReportStates?.FirstOrDefault(a => string.Compare(a.Id, report.Id.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0);
						if (reportState != null)
						{
							report.IsSelected = reportState.IsSelected;
						}

						groupItem.Reports.Add(report);
						UpdateReportParentState(report, groupItem, reportGroup);
					}
					else
					{
						groupItem = new GroupItem
						{
							Name = (_groupType.Type == "Group" ? report.Language ?? string.Empty : report.Group ?? string.Empty),
							Reports = new List<Report> { report }
						};
						var groupItemState = _previousReportStates?.FirstOrDefault(a =>
							string.Compare(a.Id, reportGroup.Name + "-" + groupItem.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
						if (groupItemState != null)
						{
							groupItem.IsExpanded = groupItemState.IsExpanded;
							groupItem.IsSelected = groupItemState.IsSelected;
						}

						reportGroup.GroupItems.Add(groupItem);
						foreach (var itemReport in groupItem.Reports)
						{
							var reportState = _previousReportStates?.FirstOrDefault(a => string.Compare(a.Id, itemReport.Id.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0);
							if (reportState != null)
							{
								itemReport.IsSelected = reportState.IsSelected;
							}

							UpdateReportParentState(itemReport, groupItem, reportGroup);
						}
					}
				}
				else
				{
					reportGroup = new ReportGroup
					{
						Name = (_groupType.Type == "Group" ? report.Group ?? string.Empty : report.Language ?? string.Empty)
					};
					var reportGroupState = _previousReportStates?.FirstOrDefault(a =>
						string.Compare(a.Id, reportGroup.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
					if (reportGroupState != null)
					{
						reportGroup.IsExpanded = reportGroupState.IsExpanded;
						reportGroup.IsSelected = reportGroupState.IsSelected;
					}

					var groupItem = new GroupItem
					{
						Name = (_groupType.Type == "Group" ? report.Language ?? string.Empty : report.Group ?? string.Empty),
						Reports = new List<Report> { report }
					};
					var groupItemState = _previousReportStates?.FirstOrDefault(a =>
						string.Compare(a.Id, reportGroup.Name + "-" + groupItem.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
					if (groupItemState != null)
					{
						groupItem.IsExpanded = groupItemState.IsExpanded;
						groupItem.IsSelected = groupItemState.IsSelected;
					}

					reportGroup.GroupItems.Add(groupItem);
					reportGroups.Add(reportGroup);

					foreach (var itemReport in groupItem.Reports)
					{
						var reportState = _previousReportStates?.FirstOrDefault(a => string.Compare(a.Id, itemReport.Id.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0);
						if (reportState != null)
						{
							itemReport.IsSelected = reportState.IsSelected;
						}

						UpdateReportParentState(itemReport, groupItem, reportGroup);
					}
				}
			}

			return await Task.FromResult(reportGroups);
		}

		private static void UpdateReportParentState(Report report, GroupItem groupItem, ReportGroup reportGroup)
		{
			if (report.IsSelected)
			{
				groupItem.IsExpanded = true;
				reportGroup.IsExpanded = true;
			}
		}

		private void ClearVisibleReports()
		{
			if (SelectedItem is Report)
			{
				SelectedReport = null;
			}

			if (SelectedItem is GroupItem)
			{
				SelectedReport = null;
				ReportViewModel.UpdateData(new List<Report>());
			}

			if (SelectedItem is ReportGroup)
			{
				SelectedReport = null;
				ReportViewModel.UpdateData(new List<Report>());
			}
		}

		private List<ReportState> GetReportStates(IEnumerable<ReportGroup> reportGroups)
		{
			var reportStates = new List<ReportState>();
			if (reportGroups == null)
			{
				return reportStates;
			}

			foreach (var reportGroup in reportGroups)
			{
				if (!reportStates.Exists(a => string.Compare(a.Id, reportGroup.Name, StringComparison.CurrentCultureIgnoreCase) == 0))
				{
					reportStates.Add(new ReportState
					{
						Id = reportGroup.Name,
						IsSelected = reportGroup.IsSelected,
						IsExpanded = reportGroup.IsExpanded
					});
				}

				foreach (var groupItem in reportGroup.GroupItems)
				{
					if (!reportStates.Exists(a => string.Compare(a.Id, reportGroup.Name + "-" + groupItem.Name, StringComparison.CurrentCultureIgnoreCase) == 0))
					{
						reportStates.Add(new ReportState
						{
							Id = reportGroup.Name + "-" + groupItem.Name,
							IsSelected = groupItem.IsSelected,
							IsExpanded = groupItem.IsExpanded
						});
					}

					foreach (var report in groupItem.Reports)
					{
						if (!reportStates.Exists(a => a.Id == report.Id.ToString()))
						{
							reportStates.Add(new ReportState
							{
								Id = report.Id.ToString(),
								IsSelected = report.IsSelected
							});
						}
					}
				}
			}

			return reportStates;
		}

		private void ExpandAll(object parameter)
		{
			_reportGroups = new ObservableCollection<ReportGroup>(ExpandAll(BuildReportGroup().Result));
			OnPropertyChanged(nameof(ReportGroups));
		}

		private List<ReportGroup> ExpandAll(List<ReportGroup> reportGroups)
		{
			foreach (var reportGroup in reportGroups)
			{
				reportGroup.IsExpanded = true;
				foreach (var groupItem in reportGroup.GroupItems)
				{
					groupItem.IsExpanded = true;
				}
			}

			return reportGroups;
		}

		private void CollapseAll(object parameter)
		{
			_reportGroups = new ObservableCollection<ReportGroup>(CollapseAll(BuildReportGroup().Result));
			OnPropertyChanged(nameof(ReportGroups));
		}

		private List<ReportGroup> CollapseAll(List<ReportGroup> reportGroups)
		{
			foreach (var reportGroup in reportGroups)
			{
				reportGroup.IsExpanded = false;
				reportGroup.IsSelected = false;
				foreach (var groupItem in reportGroup.GroupItems)
				{
					groupItem.IsExpanded = false;
					groupItem.IsSelected = false;
				}
			}

			var firstGroup = reportGroups.FirstOrDefault();
			if (firstGroup != null)
			{
				firstGroup.IsSelected = true;
			}

			return reportGroups;
		}

		private void UpdateSettings()
		{
			if (Settings.GroupByType != _groupType.Type)
			{
				Settings.GroupByType = _groupType.Type;
				File.WriteAllText(_pathInfo.SettingsFilePath, JsonConvert.SerializeObject(Settings));
			}
		}

		private void ClearFilter(object parameter)
		{
			FilterString = string.Empty;
		}

		private void SelectedItemChanged(object parameter)
		{
			if (parameter is RoutedPropertyChangedEventArgs<object> property)
			{
				if (property.NewValue is Report report)
				{
					IsReportSelected = true;
					SelectedItem = report;
					SelectedReport = report;
				}
				else if (property.NewValue is GroupItem groupItem)
				{
					SelectedItem = groupItem;
					SelectedReport = null;
					ReportViewModel.UpdateData(groupItem.Reports);
				}
				else if (property.NewValue is ReportGroup reportGroup)
				{
					SelectedItem = reportGroup;
					SelectedReport = null;
					ReportViewModel.UpdateData(reportGroup.GroupItems?.SelectMany(a => a.Reports).ToList());
				}
			}
		}

		private void PrintPreview(object parameter)
		{
			//var action = SdlTradosStudio.Application.GetAction<PrintPreviewReportAction>();
			//action.Run();
		}

		private void PageSetup(object parameter)
		{
			//var action = SdlTradosStudio.Application.GetAction<PageSetupAction>();
			//action.Run();
		}

		private void DragDrop(object parameter)
		{
			var report = new Report();

			if (parameter == null || !(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
			{
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory))
					{
						if (string.IsNullOrEmpty(report.XsltPath) &&
							(fullPath.ToLower().EndsWith(".xslt")
							 || fullPath.ToLower().EndsWith(".xsl")))
						{
							report.XsltPath = fullPath;
						}
						if (string.IsNullOrEmpty(report.Path) &&
							(fullPath.ToLower().EndsWith(".html")
							 || fullPath.ToLower().EndsWith(".htm")
							 || fullPath.ToLower().EndsWith(".xml")))
						{
							report.Path = fullPath;
						}
					}
				}
			}

			var action = SdlTradosStudio.Application.GetAction<AddReportAction>();
			action.Run(report);
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedReport != null)
			{
				var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
				action.Run();
			}
		}

		protected virtual void OnReportSelectionChanged(ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}
	}
}
