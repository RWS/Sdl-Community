using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportsNavigationViewModel : INotifyPropertyChanged
	{
		private readonly ProjectsController _projectsController;
		private List<Report> _reports;
		private string _filterString;
		private List<Report> _filteredReports;
		private Report _selectedReport;
		private bool _isReportSelected;
		private List<ReportGroup> _reportGroups;
		private GroupType _groupType;
		public List<GroupType> _groupTypes;
		private ICommand _clearSelectionCommand;
		private ICommand _clearFilterCommand;
		private ICommand _removeProjectDataCommand;
		private ICommand _openProjectFolderCommand;
		private ICommand _selectedItemChanged;

		public ReportsNavigationViewModel(List<Report> reports, ProjectsController projectsController)
		{
			_reports = reports;
			_projectsController = projectsController;

			GroupType = GroupTypes[0];

			FilteredReports = _reports;
			FilterString = string.Empty;
		}

		public EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public ICommand RemoveProjectDataCommand => _removeProjectDataCommand ?? (_removeProjectDataCommand = new CommandHandler(RemoveProjectData));

		public ICommand OpenProjectFolderCommand => _openProjectFolderCommand ?? (_openProjectFolderCommand = new CommandHandler(OpenProjectFolder));

		public ICommand SelectedItemChangedCommand => _selectedItemChanged ?? (_selectedItemChanged = new CommandHandler(SelectedItemChanged));

		public ReportViewModel ReportViewModel { get; internal set; }

		public List<Report> Reports
		{
			get => _reports;
			set
			{
				_reports = value;

				OnPropertyChanged(nameof(Reports));

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


				BuildReportGroup();

				OnPropertyChanged(nameof(StatusLabel));
			}
		}

		public List<ReportGroup> ReportGroups
		{
			get => _reportGroups;
			set
			{
				_reportGroups = value;
				OnPropertyChanged(nameof(ReportGroups));
			}
		}

		public Report SelectedReport
		{
			get => _selectedReport;
			set
			{
				_selectedReport = value;
				OnPropertyChanged(nameof(SelectedReport));

				if (_selectedReport != null)
				{
					ReportViewModel.UpdateReport(_selectedReport.Path ?? string.Empty);
				}

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


				BuildReportGroup();
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
						Name = "Group",
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

		private void BuildReportGroup()
		{
			var reportGroups = new List<ReportGroup>();

			foreach (var report in Reports)
			{
				if (_groupType.Type == "Group")
				{
					var reportGroup = reportGroups.FirstOrDefault(a => a.Name == report.Group);
					if (reportGroup != null)
					{
						var groupItem = reportGroup.GroupItems.FirstOrDefault(a => a.Name == report.Language);
						if (groupItem != null)
						{
							groupItem.Reports.Add(report);

							if (report.IsSelected)
							{
								groupItem.IsExpanded = true;
								reportGroup.IsExpanded = true;
							}
						}
						else
						{
							groupItem = new GroupItem
							{
								Name = report.Language,
								Reports = new List<Report> { report }
							};

							reportGroup.GroupItems.Add(groupItem);

							if (report.IsSelected)
							{
								groupItem.IsExpanded = true;
								reportGroup.IsExpanded = true;
							}
						}
					}
					else
					{
						// new
						var groupItem = new GroupItem
						{
							Name = report.Language,
							Reports = new List<Report> { report }
						};

						reportGroup = new ReportGroup();
						reportGroup.Name = report.Group;
						reportGroup.GroupItems.Add(groupItem);
						reportGroups.Add(reportGroup);

						if (report.IsSelected)
						{
							groupItem.IsExpanded = true;
							reportGroup.IsExpanded = true;
						}
					}
				}
				else
				{
					var reportGroup = reportGroups.FirstOrDefault(a => a.Name == report.Language);
					if (reportGroup != null)
					{
						var groupItem = reportGroup.GroupItems.FirstOrDefault(a => a.Name == report.Group);
						if (groupItem != null)
						{
							groupItem.Reports.Add(report);
							if (report.IsSelected)
							{
								groupItem.IsExpanded = true;
								reportGroup.IsExpanded = true;
							}
						}
						else
						{
							groupItem = new GroupItem
							{
								Name = report.Group,
								Reports = new List<Report> { report }
							};

							reportGroup.GroupItems.Add(groupItem);

							if (report.IsSelected)
							{
								groupItem.IsExpanded = true;
								reportGroup.IsExpanded = true;
							}
						}
					}
					else
					{
						// new
						var groupItem = new GroupItem
						{
							Name = report.Group,
							Reports = new List<Report> { report }
						};

						reportGroup = new ReportGroup();
						reportGroup.Name = report.Language;
						reportGroup.GroupItems.Add(groupItem);
						reportGroups.Add(reportGroup);

						if (report.IsSelected)
						{
							groupItem.IsExpanded = true;
							reportGroup.IsExpanded = true;
						}
					}

				}


			}

			ReportGroups = reportGroups;
		}


		private void ClearSelection(object parameter)
		{
			SelectedReport = null;
		}

		private void ClearFilter(object parameter)
		{
			FilterString = string.Empty;
		}

		private void RemoveProjectData(object parameter)
		{
			//var message1 = PluginResources.Message_ActionWillRemoveAllProjectData;
			//var message2 = PluginResources.Message_DoYouWantToProceed;

			//var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
			//	PluginResources.TranscreateManager_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			//if (response == MessageBoxResult.No)
			//{
			//	return;
			//}

			//var selectedProject = _projectsController.GetProjects()
			//	.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == SelectedReport.Id);

			//if (selectedProject != null)
			//{
			//	var settingsBundle = selectedProject.GetSettings();
			//	var managerProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

			//	managerProject.ProjectFilesJson.Value = string.Empty;

			//	selectedProject.UpdateSettings(settingsBundle);
			//	selectedProject.Save();

			//	var xliffFolderPath = Path.Combine(SelectedReport.Path, "Transcreate");
			//	if (Directory.Exists(xliffFolderPath))
			//	{
			//		try
			//		{
			//			Directory.Delete(xliffFolderPath, true);
			//		}
			//		catch
			//		{
			//			// ignore; catch all
			//		}
			//	}

			//	// TODO: remove reports


			//	Projects = Projects.Where(a => a.Id != SelectedReport.Id).ToList();
			//}
		}

		private void OpenProjectFolder(object parameter)
		{
			if (Directory.Exists(SelectedReport.Path))
			{
				System.Diagnostics.Process.Start("explorer.exe", SelectedReport.Path);
			}
		}

		private void SelectedItemChanged(object parameter)
		{
			if (parameter is RoutedPropertyChangedEventArgs<object> property)
			{
				if (property.NewValue is Report report)
				{
					SelectedReport = report;
				}
				else if (property.NewValue is GroupItem groupItem)
				{
					ReportViewModel.UpdateData(groupItem.Reports);
				}
				else if (property.NewValue is ReportGroup reportGroup)
				{
					ReportViewModel.UpdateData(reportGroup.GroupItems?.SelectMany(a => a.Reports).ToList());
				}
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
