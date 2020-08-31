using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Service;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class AppendReportViewModel : INotifyPropertyChanged
	{
		private readonly Window _window;
		private readonly ImageService _imageService;
		private readonly IProject _project;
		private readonly List<ReportTemplate> _reportTemplates;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _selectedItemsChangedCommand;
		private ICommand _clearPathCommand;
		private ICommand _browseFolderCommand;
		private string _name;
		private string _description;
		private string _group;
		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;
		private List<string> _groupNames;
		private DateTime _date;
		private string _path;
		private string _xslt;
		private bool _isEditMode;
		private bool _canEditProperties;
		private bool _canEditXslt;
		private bool _previousIsCustomTemplate;
		private string _previousNonCustomTemplate;

		public AppendReportViewModel(Window window, Report report,
			ImageService imageService, IProject project,
			List<string> groupNames, List<ReportTemplate> reportTemplates, bool isEditMode = false)
		{
			_window = window;
			Report = report;
			_imageService = imageService;
			_project = project;
			_reportTemplates = reportTemplates;
			GroupNames = groupNames;

			IsEditMode = isEditMode;

			WindowTitle = IsEditMode ? "Edit Project Report Information" : "Add Project Report";

			var projectInfo = _project.GetProjectInfo();			
			LanguageItems = projectInfo.TargetLanguages
				.Select(language => new LanguageItem
				{
					Name = language.DisplayName,
					CultureInfo = language.CultureInfo,
					Image = imageService.GetImage(language.CultureInfo.Name)
				})
				.ToList();

			SelectedLanguageItems = new List<LanguageItem> {
				LanguageItems.FirstOrDefault(a=> string.Compare(a.CultureInfo.Name, Report.Language, 
					                                 StringComparison.CurrentCultureIgnoreCase)==0) };

			Date = Report.Date;
			Name = Report.Name;
			Group = Report.Group;
			Description = Report.Description;
			Path = Report.Path ?? string.Empty;
			Xslt = Report.XsltPath;
			if (report is ReportWithXslt reportWithXslt)
			{
				Xslt = reportWithXslt.Xslt;
			}

			CanEditProperties = !report.IsStudioReport;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public ICommand ClearPathCommand => _clearPathCommand ?? (_clearPathCommand = new CommandHandler(ClearPath));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public Report Report { get; }

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool CanEditProperties
		{
			get => _canEditProperties;
			set
			{
				if (_canEditProperties == value)
				{
					return;
				}

				_canEditProperties = value;
				OnPropertyChanged(nameof(CanEditProperties));
			}
		}

		public bool CanEditXslt
		{
			get => _canEditXslt;
			set
			{
				if (_canEditXslt == value)
				{
					return;
				}

				_canEditXslt = value;
				OnPropertyChanged(nameof(CanEditXslt));
			}
		}

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				if (_isEditMode == value)
				{
					return;
				}

				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				if (_description == value)
				{
					return;
				}

				_description = value;
				OnPropertyChanged(nameof(Description));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string Group
		{
			get => _group;
			set
			{
				if (_group == value)
				{
					return;
				}

				_group = value;
				ValidateReportTemplate();
				OnPropertyChanged(nameof(Group));
				OnPropertyChanged(nameof(IsValid));				
			}
		}

		public List<LanguageItem> LanguageItems
		{
			get => _languageItems;
			set
			{
				_languageItems = value;
				OnPropertyChanged(nameof(LanguageItems));
			}
		}

		public List<LanguageItem> SelectedLanguageItems
		{
			get => _selectedLanguageItems;
			set
			{
				_selectedLanguageItems = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
			}
		}

		public List<string> GroupNames
		{
			get => _groupNames;
			set
			{
				_groupNames = value;
				OnPropertyChanged(nameof(GroupNames));
			}
		}

		public DateTime Date
		{
			get => _date;
			set
			{
				if (_date == value)
				{
					return;
				}

				_date = value;
				OnPropertyChanged(nameof(Date));
			}
		}

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;
				OnPropertyChanged(nameof(Path));

				if (File.Exists(_path) && string.IsNullOrEmpty(Name))
				{
					Name = System.IO.Path.GetFileName(_path);
					while (!string.IsNullOrEmpty(System.IO.Path.GetExtension(Name)))
					{
						Name = Name?.Substring(0, Name.Length - System.IO.Path.GetExtension(Name).Length);
					}
				}

				ValidateReportTemplate();
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string Xslt
		{
			get => _xslt;
			set
			{
				if (_xslt == value)
				{
					return;
				}

				_xslt = value ?? "";
				OnPropertyChanged(nameof(Xslt));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				if (!IsEditMode)
				{
					if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
					{
						return false;
					}

					if (!string.IsNullOrEmpty(Xslt) && !File.Exists(Xslt))
					{
						return false;
					}
				}

				if (string.IsNullOrEmpty(Name))
				{
					return false;
				}

				return true;
			}
		}

		private void ValidateReportTemplate()
		{
			var reportTemplate = GetReportTemplate();
			if (reportTemplate != null)
			{
				if (!_previousIsCustomTemplate)
				{
					_previousNonCustomTemplate = Xslt;
				}

				Xslt = reportTemplate.Path;
				CanEditXslt = false;
				_previousIsCustomTemplate = true;
				return;
			}

			
			if (_previousIsCustomTemplate)
			{
				_previousIsCustomTemplate = false;
				Xslt = _previousNonCustomTemplate;
			}

			CanEditXslt = _path != null && File.Exists(_path) && _path.ToLower().EndsWith(".xml");

			OnPropertyChanged(nameof(IsValid));
		}

		private ReportTemplate GetReportTemplate()
		{
			var language = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name;
			var uniqueReportId = GetUniqueReportId(_group, language,
				Report.IsStudioReport
					? ReportTemplate.TemplateScope.StudioOnly.ToString()
					: ReportTemplate.TemplateScope.NonStudioOnly.ToString());
			var reportTemplate = _reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));
			if (reportTemplate == null)
			{
				uniqueReportId = GetUniqueReportId(_group, language, ReportTemplate.TemplateScope.All.ToString());
				reportTemplate = _reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));
			}

			return reportTemplate;
		}

		private string GetUniqueReportId(ReportTemplate reportTemplate)
		{
			return GetUniqueReportId(reportTemplate?.Group ?? string.Empty, reportTemplate?.Language ?? string.Empty, reportTemplate?.Scope.ToString());
		}

		private string GetUniqueReportId(string group, string language, string scope)
		{
			var id = (group + "-" + language + "-" + scope).ToLower();
			return id;
		}

		private void SaveChanges(object parameter)
		{
			if (IsValid)
			{
				Report.Name = Name;
				Report.Group = Group;
				Report.Description = Description;
				Report.Path = Path;
				Report.XsltPath = Xslt;
				Report.Language = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name ?? string.Empty;

				_window.DialogResult = true;
				_window?.Close();
			}
		}

		private void SelectedItemsChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedLanguageItems));
				ValidateReportTemplate();
			}
		}

		private void ClearPath(object parameter)
		{
			if (parameter.ToString() == "path")
			{
				Path = string.Empty;
			}
			else
			{
				Xslt = string.Empty;
			}
		}

		private void BrowseFolder(object parameter)
		{
			var fileType = parameter.ToString() == "path" ? "report" : "xslt template";

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Title = string.Format("Select the {0} file", fileType);

			if (fileType == "report")
			{
				openFileDialog.InitialDirectory = !string.IsNullOrEmpty(Path) ? GetValidFolderPath(Path) : _project.GetProjectInfo().LocalProjectFolder;
			}
			else
			{
				if (!string.IsNullOrEmpty(Xslt))
				{
					openFileDialog.InitialDirectory = GetValidFolderPath(Xslt);
				}
				else if (!string.IsNullOrEmpty(Path))
				{
					openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Path);
				}
				else
				{
					openFileDialog.InitialDirectory = _project.GetProjectInfo().LocalProjectFolder;
				}
			}

			openFileDialog.Filter = fileType == "report"
				? "All supported files (*.html;*.htm;*.xml)|*.html;*.htm;*.xml|HTML files(*.html;*.htm)|*.html;*.htm|XML files(*.xml)|*.xml"
				: "XSLT files(*.xslt)| *.xslt;*.xsl";

			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (fileType == "report")
				{
					Path = openFileDialog.FileName;
				}
				else
				{
					Xslt = openFileDialog.FileName;
				}
			}
		}

		private string GetValidFolderPath(string directory)
		{
			if (string.IsNullOrWhiteSpace(directory))
			{
				return string.Empty;
			}

			var folder = directory;
			if (Directory.Exists(folder))
			{
				return folder;
			}

			while (folder.Contains("\\"))
			{
				folder = folder.Substring(0, folder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(folder))
				{
					return folder;
				}
			}

			return folder;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
