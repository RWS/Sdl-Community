using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.Model;
using Reports.Viewer.Plus.Service;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;

namespace Reports.Viewer.Plus.ViewModel
{
	public class AppendTemplateViewModel : INotifyPropertyChanged
	{		
		private string _windowTitle;
		private ICommand _clearPathCommand;
		private ICommand _selectedLanguagesChangedCommand;
		private ICommand _selectedTemplateScopesChangedCommand;
		private ICommand _browseFolderCommand;
		private string _path;
		private string _group;
		private bool _isEditMode;
		private readonly List<ReportTemplate> _reportTemplates;
		private readonly ReportTemplate _reportTemplate;
		private readonly IProject _project;
		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;
		private List<ReportTemplateScope> _templateScopes;
		private List<ReportTemplateScope> _selectedTemplateScopes;
		private List<string> _groupNames;
		private List<LanguageGroup> _languageGroups;

		public AppendTemplateViewModel(ReportTemplate reportTemplate,
			List<ReportTemplate> reportTemplates, IProject project, ImageService imageService,
			List<string> groupNames, bool isEditMode)
		{			
			_reportTemplate = reportTemplate ?? new ReportTemplate();
			_reportTemplates = reportTemplates ?? new List<ReportTemplate>();
			_project = project;

			GroupNames = groupNames;

			Path = reportTemplate?.Path.Clone() as string;
			Group = reportTemplate?.Group.Clone() as string;
			IsEditMode = isEditMode;

			var projectInfo = _project.GetProjectInfo();
			var projectLanguages = projectInfo.TargetLanguages.ToList();

			LanguageItems = LanguageRegistryApi.Instance.GetAllLanguages()
				.Select(language => new LanguageItem
				{
					Name = language.DisplayName,
					CultureInfo = language.CultureInfo,
					Image = imageService.GetImage(language.CultureInfo.Name),
					Group = GetLanguageGroup(language, projectLanguages)
				})
				.OrderBy(a => a.Name).ToList();
			
			SelectedLanguageItems = new List<LanguageItem> {
				LanguageItems.FirstOrDefault(a=> string.Compare(a.CultureInfo.Name, reportTemplate?.Language, StringComparison.CurrentCultureIgnoreCase)==0) };

			var templateScopes = new List<ReportTemplateScope>();
			foreach (var scope in (ReportTemplate.TemplateScope[])Enum.GetValues(typeof(ReportTemplate.TemplateScope)))
			{
				templateScopes.Add(GetReportTemplateScope(scope));
			}

			TemplateScopes = new List<ReportTemplateScope>(templateScopes);

			SelectedTemplateScopes = new List<ReportTemplateScope> {
				TemplateScopes.FirstOrDefault(a=> a.Scope == reportTemplate?.Scope) ?? TemplateScopes.FirstOrDefault()};

			WindowTitle = IsEditMode ? PluginResources.WindowTitle_EditReportTemplate : PluginResources.WindowTitle_AddReportTemplate;
		}
		
		public ICommand SelectedLanguagesChangedCommand => _selectedLanguagesChangedCommand ?? (_selectedLanguagesChangedCommand = new CommandHandler(SelectedLanguagesChanged));

		public ICommand SelectedTemplateScopesChangedCommand => _selectedTemplateScopesChangedCommand ?? (_selectedTemplateScopesChangedCommand = new CommandHandler(SelectedTemplateScopesChanged));

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

		public List<string> GroupNames
		{
			get => _groupNames;
			set
			{
				_groupNames = value;
				OnPropertyChanged(nameof(GroupNames));
			}
		}

		public List<LanguageItem> LanguageItems
		{
			get => _languageItems;
			set
			{
				_languageItems = value;
				OnPropertyChanged(nameof(LanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<LanguageItem> SelectedLanguageItems
		{
			get => _selectedLanguageItems;
			set
			{
				_selectedLanguageItems = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<ReportTemplateScope> TemplateScopes
		{
			get => _templateScopes;
			set
			{
				_templateScopes = value;
				OnPropertyChanged(nameof(TemplateScopes));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<ReportTemplateScope> SelectedTemplateScopes
		{
			get => _selectedTemplateScopes;
			set
			{
				_selectedTemplateScopes = value;
				OnPropertyChanged(nameof(SelectedTemplateScopes));
				OnPropertyChanged(nameof(IsValid));
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
				OnPropertyChanged(nameof(Group));
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
				}				

				var language = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name;
				var scope = SelectedTemplateScopes?.FirstOrDefault()?.Scope ?? ReportTemplate.TemplateScope.All;
				var uniqueReportId = GetUniqueReportId(_group, language, scope.ToString());

				if (IsEditMode)
				{
					if (string.Compare(Group, _reportTemplate?.Group, StringComparison.CurrentCultureIgnoreCase) != 0
						&& _reportTemplates.Exists(a => GetUniqueReportId(a) == uniqueReportId))
					{
						return false;
					}
				}
				else
				{
					if (_reportTemplates.Exists(a => GetUniqueReportId(a) == uniqueReportId))
					{
						return false;
					}
				}

				return true;
			}
		}

		public List<LanguageGroup> LanguageGroups
		{
			get
			{
				return _languageGroups ?? (_languageGroups = new List<LanguageGroup>
				{
					new LanguageGroup
					{
						Order = 0,
						Name = PluginResources.LanguageGroup_AvailableLanguages
					},
					new LanguageGroup
					{
						Order = 1,
						Name = PluginResources.LanguageGroup_AllLanguages
					}
				});
			}
		}

		private LanguageGroup GetLanguageGroup(Language language, List<Language> projectLanguages)
		{
			if (projectLanguages.Exists(a =>
				string.Compare(a.CultureInfo.Name, language.CultureInfo.Name, StringComparison.CurrentCultureIgnoreCase) == 0))
			{
				return LanguageGroups[0];
			}

			return LanguageGroups[1];
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

		private void ClearPath(object parameter)
		{
			if (parameter.ToString() == "path")
			{
				Path = string.Empty;
			}
		}

		private void BrowseFolder(object parameter)
		{
			var projectInfo = _project.GetProjectInfo();

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Title = PluginResources.WindowTitle_SelectTemplateFile;
			openFileDialog.InitialDirectory = !string.IsNullOrEmpty(Path) ? GetValidFolderPath(Path) : projectInfo.LocalProjectFolder;
			openFileDialog.Filter = "XSLT files(*.xslt)| *.xslt;*.xsl";

			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				Path = openFileDialog.FileName;
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

		private void SelectedLanguagesChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		private void SelectedTemplateScopesChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedTemplateScopes));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		private ReportTemplateScope GetReportTemplateScope(ReportTemplate.TemplateScope templateScope)
		{
			var name = string.Empty;
			switch (templateScope)
			{
				case ReportTemplate.TemplateScope.All:
					name = PluginResources.TemplateScope_All;
					break;
				case ReportTemplate.TemplateScope.StudioOnly:
					name = PluginResources.TemplateScope_StudioOnly;
					break;
				case ReportTemplate.TemplateScope.NonStudioOnly:
					name = PluginResources.TemplateScope_NonStudioOnly;
					break;
			}

			return new ReportTemplateScope
			{
				Name = name,
				Scope = templateScope
			};
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
