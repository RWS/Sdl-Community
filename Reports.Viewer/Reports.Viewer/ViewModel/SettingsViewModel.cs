using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.Model;
using Reports.Viewer.Plus.Service;
using Reports.Viewer.Plus.View;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace Reports.Viewer.Plus.ViewModel
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		private readonly Window _window;
		private readonly Settings _settings;
		private readonly PathInfo _pathInfo;
		private readonly ImageService _imageService;
		private readonly ReportsViewerController _controller;
		private readonly List<string> _groupNames;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _resetCommand;
		private ICommand _addTemplateCommand;
		private ICommand _editTemplateCommand;
		private ICommand _removeTemplateCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;
		private bool _displayDateSuffixWithReportName;
		private GroupType _groupType;
		private List<GroupType> _groupTypes;
		private ObservableCollection<ReportTemplate> _reportTemplates;
		private IList _selectedReportTemplates;
		private ReportTemplate _selectedReportTemplate;

		public SettingsViewModel(Window window, Settings settings, ImageService imageService,
			PathInfo pathInfo, ReportsViewerController controller, List<string> groupNames)
		{
			_window = window;
			_settings = settings;
			_pathInfo = pathInfo;
			_imageService = imageService;
			_controller = controller;
			_groupNames = groupNames;

			WindowTitle = "Settings";

			DisplayDateSuffixWithReportName = settings.DisplayDateSuffixWithReportName;
			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();

			var reportTemplates = new List<ReportTemplate>();
			foreach (var reportTemplate in _controller.GetCustomReportTemplates())
			{
				if (!string.IsNullOrEmpty(reportTemplate.Path) && File.Exists(reportTemplate.Path))
				{
					reportTemplate.IsAvailable = true;
				}

				reportTemplates.Add(reportTemplate);
			}

			_reportTemplates = new ObservableCollection<ReportTemplate>(reportTemplates);
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

		public ICommand AddTemplateCommand => _addTemplateCommand ?? (_addTemplateCommand = new CommandHandler(AddTemplate));

		public ICommand EditTemplateCommand => _editTemplateCommand ?? (_editTemplateCommand = new CommandHandler(EditTemplate));

		public ICommand RemoveTemplateCommand => _removeTemplateCommand ?? (_removeTemplateCommand = new CommandHandler(RemoveTemplate));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public bool DisplayDateSuffixWithReportName
		{
			get => _displayDateSuffixWithReportName;
			set
			{
				if (_displayDateSuffixWithReportName == value)
				{
					return;
				}

				_displayDateSuffixWithReportName = value;
				OnPropertyChanged(nameof(DisplayDateSuffixWithReportName));
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
				OnPropertyChanged(nameof(GroupType));
			}
		}

		public string TemplateStatusLabel
		{
			get
			{
				var message = string.Format(PluginResources.StatusLabel_FileSelected,
					ReportTemplates?.Count ?? 0,
					SelectedReportTemplates?.Count ?? 0);
				return message;
			}
		}

		public ObservableCollection<ReportTemplate> ReportTemplates
		{
			get => _reportTemplates;
			set
			{
				_reportTemplates = value;
				OnPropertyChanged(nameof(ReportTemplates));
				OnPropertyChanged(nameof(TemplateStatusLabel));
			}
		}

		public ReportTemplate SelectedReportTemplate
		{
			get => _selectedReportTemplate;
			set
			{
				_selectedReportTemplate = value;
				OnPropertyChanged(nameof(SelectedReportTemplate));

				OnPropertyChanged(nameof(IsReportSelected));
				OnPropertyChanged(nameof(IsReportsSelected));
				OnPropertyChanged(nameof(TemplateStatusLabel));
			}
		}

		public IList SelectedReportTemplates
		{
			get => _selectedReportTemplates;
			set
			{
				_selectedReportTemplates = value;
				OnPropertyChanged(nameof(SelectedReportTemplates));


				_selectedReportTemplate = _selectedReportTemplates?.Cast<ReportTemplate>().ToList().FirstOrDefault();

				OnPropertyChanged(nameof(IsReportSelected));
				OnPropertyChanged(nameof(IsReportsSelected));
				OnPropertyChanged(nameof(TemplateStatusLabel));
			}
		}

		public bool IsReportsSelected => SelectedReportTemplates?.Cast<ReportTemplate>().ToList().Count > 0;

		public bool IsReportSelected => SelectedReportTemplates?.Cast<ReportTemplate>().ToList().Count == 1;

		public bool UpdatedTemplates { get; private set; }

		private void SaveChanges(object parameter)
		{
			_settings.DisplayDateSuffixWithReportName = DisplayDateSuffixWithReportName;
			_settings.GroupByType = GroupType.Type;

			File.WriteAllText(_pathInfo.SettingsFilePath, JsonConvert.SerializeObject(_settings));

			_controller.UpdateCustomReportTemplates(_reportTemplates.ToList());

			_window.DialogResult = true;
			_window?.Close();
		}

		private void Reset(object paramter)
		{
			var settings = new Settings();

			DisplayDateSuffixWithReportName = settings.DisplayDateSuffixWithReportName;
			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();
		}

		private void OpenAppendTemplate(ReportTemplate reportTemplate, bool isEditMode)
		{
			var viewModel = new AppendTemplateViewModel(reportTemplate,
				_reportTemplates.ToList(), _controller.GetSelectedProject(), _imageService, _groupNames, isEditMode);

			var window = new AppendTemplateWindow(viewModel, _window);

			var result = window.ShowDialog();
			if (result != null && (bool)result)
			{
				UpdatedTemplates = true;
				if (isEditMode)
				{
					var template = _reportTemplates.FirstOrDefault(a => a.Id == reportTemplate.Id);
					if (template != null)
					{
						template.IsAvailable = string.IsNullOrEmpty(viewModel.Path) && File.Exists(viewModel.Path);
						template.Group = viewModel.Group.Trim();
						template.Path = viewModel.Path;
						template.Scope = viewModel.SelectedTemplateScopes?.FirstOrDefault()?.Scope ?? ReportTemplate.TemplateScope.All;
						template.Language = viewModel.SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name ?? string.Empty;
					}
				}
				else
				{
					_reportTemplates.Add(new ReportTemplate
					{
						Scope = viewModel.SelectedTemplateScopes?.FirstOrDefault()?.Scope ?? ReportTemplate.TemplateScope.All,
						Language = viewModel.SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name ?? string.Empty,
						Group = viewModel.Group,
						Path = viewModel.Path,
						IsAvailable = string.IsNullOrEmpty(viewModel.Path) && File.Exists(viewModel.Path)
					});
				}

				OnPropertyChanged(nameof(ReportTemplates));
				OnPropertyChanged(nameof(TemplateStatusLabel));
			}
		}

		private void AddTemplate(object paramter)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Title = PluginResources.WindowTitle_SelectTemplateFile;
			openFileDialog.Filter = "XSLT files(*.xslt)| *.xslt;*.xsl";
			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(openFileDialog.FileName) && !File.Exists(openFileDialog.FileName))
				{
					return;
				}

				_window.Dispatcher.Invoke(DispatcherPriority.Input, new System.Action(delegate
				{
					OpenAppendTemplate(new ReportTemplate
					{
						Group = string.Empty,
						Language = string.Empty,
						Scope = ReportTemplate.TemplateScope.All,
						Path = openFileDialog.FileName
					}, false);
				}));
			}

		}

		private void EditTemplate(object paramter)
		{
			if (SelectedReportTemplate != null)
			{
				OpenAppendTemplate(SelectedReportTemplate, true);
			}
		}

		private void RemoveTemplate(object paramter)
		{
			var selectedIds = _selectedReportTemplates.Cast<ReportTemplate>().ToList().Select(a => a.Id);
			foreach (var id in selectedIds)
			{
				var template = _reportTemplates.FirstOrDefault(a => a.Id == id);
				if (template != null)
				{
					_reportTemplates.Remove(template);
				}
			}

			OnPropertyChanged(nameof(ReportTemplates));
			OnPropertyChanged(nameof(TemplateStatusLabel));
		}

		private void DragDrop(object parameter)
		{
			if (parameter == null || !(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
			{
				var xsltFilePaths = new List<string>();
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory))
					{
						if (fullPath.ToLower().EndsWith(".xslt")
							 || fullPath.ToLower().EndsWith(".xsl"))
						{
							xsltFilePaths.Add(fullPath);
						}
					}
				}

				if (xsltFilePaths.Count > 0)
				{
					_window.Dispatcher.Invoke(DispatcherPriority.Input, new System.Action(delegate
					{
						OpenAppendTemplate(new ReportTemplate
						{
							Group = string.Empty,
							Language = string.Empty,
							Scope = ReportTemplate.TemplateScope.All,
							Path = xsltFilePaths[0]
						}, false);
					}));
				}
			}
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedReportTemplate != null)
			{
				if (!_window.Dispatcher.CheckAccess())
				{
					_window.Dispatcher.Invoke(() => MouseDoubleClick(parameter));
				}
				
				//TODO investigate why the comboboxes are not prepopulated
				OpenAppendTemplate(SelectedReportTemplate, true);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
