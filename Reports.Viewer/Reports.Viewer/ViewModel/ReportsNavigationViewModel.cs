using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Actions;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportsNavigationViewModel : INotifyPropertyChanged, IDisposable
	{
		private readonly PathInfo _pathInfo;
		private List<Report> _reports;
		private string _filterString;
		private List<Report> _filteredReports;
		private Report _selectedReport;
		private bool _isReportSelected;
		private ObservableCollection<ReportGroup> _reportGroups;
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
		private ICommand _editReportCommand;
		private ICommand _removeReportCommand;
		private ICommand _openFolderCommand;
		private ICommand _printReportCommand;
		private ICommand _printPreviewCommand;
		private ICommand _pageSetupCommand;
		private ICommand _saveAsCommand;
		private ICommand _mouseDoubleClick;

		public ReportsNavigationViewModel(List<Report> reports, Settings settings, PathInfo pathInfo)
		{
			_reports = reports;
			Settings = settings;
			_pathInfo = pathInfo;

			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();

			FilteredReports = _reports;
			FilterString = string.Empty;
		}

		public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public ICommand ExpandAllCommand => _expandAllCommand ?? (_expandAllCommand = new CommandHandler(ExpandAll));

		public ICommand CollapseAllCommand => _collapseAllCommand ?? (_collapseAllCommand = new CommandHandler(CollapseAll));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ?? (_selectedItemChangedCommand = new CommandHandler(SelectedItemChanged));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand EditReportCommand => _editReportCommand ?? (_editReportCommand = new CommandHandler(EditReport));

		public ICommand RemoveReportCommand => _removeReportCommand ?? (_removeReportCommand = new CommandHandler(RemoveReport));

		public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

		public ICommand PrintReportCommand => _printReportCommand ?? (_printReportCommand = new CommandHandler(PrintReport));

		public ICommand PrintPreviewCommand => _printPreviewCommand ?? (_printPreviewCommand = new CommandHandler(PrintPreview));

		public ICommand PageSetupCommand => _pageSetupCommand ?? (_pageSetupCommand = new CommandHandler(PageSetup));

		public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new CommandHandler(SaveAs));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public ReportsNavigationView ReportsNavigationView { get; set; }

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

		public string ProjectLocalFolder
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

		public List<Report> Reports
		{
			get => _reports;
			set
			{
				_reports = value;

				OnPropertyChanged(nameof(Reports));

				if (_reportGroups != null)
				{
					_previousReportStates = GetReportStates(_reportGroups);
				}

				_reportGroups = new ObservableCollection<ReportGroup>();

				FilterString = string.Empty;
				FilteredReports = _reports;
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

				FilteredReports = string.IsNullOrEmpty(_filterString)
					? _reports
					: _reports.Where(a => a.Name.ToLower().Contains(_filterString.ToLower())).ToList();
			}
		}

		public List<Report> FilteredReports
		{
			get => _filteredReports;
			set
			{
				_filteredReports = value;
				OnPropertyChanged(nameof(FilteredReports));

				if (_filteredReports?.Count > 0 && !_filteredReports.Contains(SelectedReport))
				{
					SelectedReport = _filteredReports[0];
				}
				else if (_filteredReports?.Count == 0)
				{
					SelectedReport = null;
				}

				if (ReportGroups == null || ReportGroups.Count == 0)
				{
					var reportGroups = BuildReportGroup();
					ReportGroups = new ObservableCollection<ReportGroup>(reportGroups);
				}
				else
				{
					foreach (var reportGroup in ReportGroups)
					{
						foreach (var itemGroupItem in reportGroup.GroupItems)
						{
							itemGroupItem.Reports = GroupType.Type == "Group"
								? _filteredReports?.Where(a => a.Group == reportGroup.Name && a.Language == itemGroupItem.Name).ToList()
								: _filteredReports?.Where(a => a.Language == reportGroup.Name && a.Group == itemGroupItem.Name).ToList();
						}
					}

					OnPropertyChanged(nameof(ReportGroups));
				}

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public ObservableCollection<ReportGroup> ReportGroups
		{
			get => _reportGroups;
			set
			{
				_reportGroups = value;
				OnPropertyChanged(nameof(ReportGroups));
			}
		}

		public void UpdateReports(Settings settings)
		{
			Settings = settings;
			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();
		}

		public Report SelectedReport
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

				var previousGroupType = _groupType;

				_groupType = value;
				OnPropertyChanged(nameof(GroupType));

				if (previousGroupType != null)
				{
					if (_reportGroups != null)
					{
						_previousReportStates = GetReportStates(_reportGroups);
					}
					
					ReportGroups = new ObservableCollection<ReportGroup>(BuildReportGroup());
				}

				UpdateSettings();
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
						Name = "Group Name",
						Type = "Group"
					},
					new GroupType
					{
						Name = "Language",
						Type = "Language"
					},
				});
			}
			set
			{
				_groupTypes = value;
				OnPropertyChanged(nameof(GroupType));
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

		private List<ReportGroup> BuildReportGroup()
		{
			var reportGroups = new List<ReportGroup>();
			if (FilteredReports == null)
			{
				return new List<ReportGroup>(reportGroups);
			}

			var orderedReports = GroupType.Type == "Group"
				? FilteredReports.OrderBy(a => a.Group).ThenBy(a => a.Language).ThenByDescending(a => a.Date).ToList()
				: FilteredReports.OrderBy(a => a.Language).ThenBy(a => a.Group).ThenByDescending(a => a.Date).ToList();

			foreach (var report in orderedReports)
			{
				var reportGroup = reportGroups.FirstOrDefault(a => a.Name == (GroupType.Type == "Group" ? report.Group : report.Language));
				if (reportGroup != null)
				{
					var groupItem = reportGroup.GroupItems.FirstOrDefault(a => a.Name == (GroupType.Type == "Group" ? report.Language : report.Group));
					if (groupItem != null)
					{
						groupItem.Reports.Add(report);
						UpdateReportParentState(report, groupItem, reportGroup);
					}
					else
					{
						groupItem = new GroupItem
						{
							Name = (GroupType.Type == "Group" ? report.Language : report.Group),
							Reports = new List<Report> { report }
						};
						var groupItemState = _previousReportStates?.FirstOrDefault(a => a.Id == reportGroup.Name + "-" + groupItem.Name);
						if (groupItemState != null)
						{
							groupItem.IsExpanded = groupItemState.IsExpanded;
							groupItem.IsSelected = groupItemState.IsSelected;
						}

						reportGroup.GroupItems.Add(groupItem);
						foreach (var itemReport in groupItem.Reports)
						{
							UpdateReportParentState(itemReport, groupItem, reportGroup);
						}
					}
				}
				else
				{
					reportGroup = new ReportGroup
					{
						Name = (GroupType.Type == "Group" ? report.Group : report.Language)
					};
					var reportGroupState = _previousReportStates?.FirstOrDefault(a => a.Id == reportGroup.Name);
					if (reportGroupState != null)
					{
						reportGroup.IsExpanded = reportGroupState.IsExpanded;
						reportGroup.IsSelected = reportGroupState.IsSelected;
					}

					var groupItem = new GroupItem
					{
						Name = (GroupType.Type == "Group" ? report.Language : report.Group),
						Reports = new List<Report> { report }
					};
					var groupItemState = _previousReportStates?.FirstOrDefault(a => a.Id == reportGroup.Name + "-" + groupItem.Name);
					if (groupItemState != null)
					{
						groupItem.IsExpanded = groupItemState.IsExpanded;
						groupItem.IsSelected = groupItemState.IsSelected;
					}

					reportGroup.GroupItems.Add(groupItem);
					reportGroups.Add(reportGroup);

					foreach (var itemReport in groupItem.Reports)
					{
						UpdateReportParentState(itemReport, groupItem, reportGroup);
					}
				}
			}

			return new List<ReportGroup>(reportGroups);
		}

		private static void UpdateReportParentState(Report report, GroupItem groupItem, ReportGroup reportGroup)
		{
			if (report.IsSelected)
			{
				groupItem.IsExpanded = true;
				reportGroup.IsExpanded = true;
			}
		}

		private List<ReportState> GetReportStates(IEnumerable<ReportGroup> reportGroups)
		{
			var itemStates = new List<ReportState>();

			foreach (var reportGroup in reportGroups)
			{
				if (!itemStates.Exists(a => a.Id == reportGroup.Name))
				{
					itemStates.Add(new ReportState
					{
						Id = reportGroup.Name,
						IsSelected = reportGroup.IsSelected,
						IsExpanded = reportGroup.IsExpanded
					});
				}

				foreach (var groupItem in reportGroup.GroupItems)
				{
					if (!itemStates.Exists(a => a.Id == reportGroup.Name + "-" + groupItem.Name))
					{
						itemStates.Add(new ReportState
						{
							Id = reportGroup.Name + "-" + groupItem.Name,
							IsSelected = groupItem.IsSelected,
							IsExpanded = groupItem.IsExpanded
						});
					}

					foreach (var report in groupItem.Reports)
					{
						if (!itemStates.Exists(a => a.Id == report.Id))
						{
							itemStates.Add(new ReportState
							{
								Id = report.Id,
								IsSelected = report.IsSelected
							});
						}
					}
				}
			}

			return itemStates;
		}

		private void ExpandAll(object parameter)
		{
			var reportGroups = ExpandAll(BuildReportGroup());

			ReportGroups = new ObservableCollection<ReportGroup>(reportGroups);
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
			var reportGroups = CollapseAll(BuildReportGroup());
			ReportGroups = new ObservableCollection<ReportGroup>(reportGroups);
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

			reportGroups[0].IsSelected = true;

			return reportGroups;
		}

		private void UpdateSettings()
		{
			if (Settings.GroupByType != GroupType.Type)
			{
				Settings.GroupByType = GroupType.Type;
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

		private void EditReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
			action.Run();
		}

		private void RemoveReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
			action.Run();
		}

		private void OpenFolder(object parameter)
		{
			if (SelectedReport?.Path == null || string.IsNullOrEmpty(ProjectLocalFolder)
				|| !Directory.Exists(ProjectLocalFolder))
			{
				return;
			}

			var path = Path.Combine(ProjectLocalFolder, SelectedReport.Path.Trim('\\'));

			if (File.Exists(path))
			{
				System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(path));
			}
		}

		private void PrintReport(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PrintReportAction>();
			action.Run();
		}

		private void PrintPreview(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PrintPreviewReportAction>();
			action.Run();
		}

		private void PageSetup(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<PageSetupAction>();
			action.Run();
		}

		private void SaveAs(object parameter)
		{
			var action = SdlTradosStudio.Application.GetAction<SaveAsReportAction>();
			action.Run();
		}

		private void DragDrop(object parameter)
		{
			var report = new ReportWithXslt();

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
						if (string.IsNullOrEmpty(report.Xslt) &&
							(fullPath.ToLower().EndsWith(".xslt")
							 || fullPath.ToLower().EndsWith(".xsl")))
						{
							report.Xslt = fullPath;
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

		public void Dispose()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnReportSelectionChanged(ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}
	}
}
