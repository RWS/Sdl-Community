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
using Sdl.Core.Globalization;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class AppendReportViewModel : INotifyPropertyChanged
	{
		private readonly Window _window;
		private readonly Settings _settings;
		private readonly PathInfo _pathInfo;
		private readonly ImageService _imageService;
		private readonly IProject _project;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _selectedItemsChangedCommand;
		private ICommand _clearPathCommand;
		private ICommand _browseFolderCommand;
		private string _name;
		private string _description;
		private string _groupName;
		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;
		private DateTime _date;
		private string _path;
		private string _xslt;


		public AppendReportViewModel(Window window, Report report, Settings settings,
			PathInfo pathInfo, ImageService imageService, IProject project)
		{
			_window = window;
			Report = report;
			_settings = settings;
			_pathInfo = pathInfo;
			_imageService = imageService;
			_project = project;

			WindowTitle = "Add Report to Project";

			var projectInfo = _project.GetProjectInfo();

			var allLanguages = new List<Language> { projectInfo.SourceLanguage };
			allLanguages.AddRange(projectInfo.TargetLanguages);

			LanguageItems = allLanguages
				.Select(language => new LanguageItem
				{
					Name = language.DisplayName,
					CultureInfo = language.CultureInfo,
					Image = imageService.GetImage(language.CultureInfo.Name)
				})
				.ToList();

			SelectedLanguageItems = new List<LanguageItem> {
				LanguageItems.FirstOrDefault(a=> a.CultureInfo.Name == Report.Language) };

			Date = Report.Date;
			Name = Report.Name;
			GroupName = Report.Group;
			Description = Report.Description;
			Path = Report.Path;
			Xslt = Report.Xslt;
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

		public string GroupName
		{
			get => _groupName;
			set
			{
				if (_groupName == value)
				{
					return;
				}

				_groupName = value;
				OnPropertyChanged(nameof(GroupName));
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


				if (File.Exists(Path) && string.IsNullOrEmpty(Name))
				{
					Name = System.IO.Path.GetFileName(Path);
					while (!string.IsNullOrEmpty(System.IO.Path.GetExtension(Name)))
					{
						Name = Name?.Substring(0, Name.Length - System.IO.Path.GetExtension(Name).Length);
					}
				}

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

				_xslt = value;
				OnPropertyChanged(nameof(Xslt));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
				{
					return false;
				}

				if (!string.IsNullOrEmpty(Xslt) && !File.Exists(Xslt))
				{
					return false;
				}				

				if (string.IsNullOrEmpty(Name))
				{
					return false;
				}

				return true;
			}
		}

		private void SaveChanges(object parameter)
		{
			if (IsValid)
			{
				Report.Name = Name;
				Report.Group = GroupName;
				Report.Description = Description;
				Report.Path = Path;
				Report.Xslt = Xslt;
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
			var fileType = parameter.ToString() == "path" ? "HTML" : "XSLT";

			var openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Title = string.Format("Select the {0} file", fileType);
			openFileDialog.InitialDirectory = _project.GetProjectInfo().LocalProjectFolder;
			openFileDialog.Filter = fileType == "HTML"
				? "HTML files(*.html;*.htm)| *.html;*.htm"
				: "XSLT files(*.xslt)| *.xslt;*.xsl";

			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (fileType == "HTML")
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
