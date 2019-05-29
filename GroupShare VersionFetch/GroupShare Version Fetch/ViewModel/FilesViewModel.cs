using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class FilesViewModel : ProjectWizardViewModelBase
	{
		private bool _isValid;
		public static readonly Log Log = Log.Instance;
		private readonly ProjectService _projectService;
		private readonly WizardModel _wizardModel;
		private SolidColorBrush _textMessageBrush;
		private string _textMessage;
		private string _textMessageVisibility;
		private readonly ObservableCollection<GsProject> _oldSelectedProjects;
		private string _displayName;

		public FilesViewModel(WizardModel wizardModel,object view) : base(view)
		{
			_projectService = new ProjectService();
			_oldSelectedProjects = new ObservableCollection<GsProject>();
			_displayName = "Projects files";
			_wizardModel = wizardModel;
			PropertyChanged += FilesViewModel_PropertyChanged;
			_wizardModel.GsFiles.CollectionChanged += GsFiles_CollectionChanged;
		}

		private void GsFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (GsFile gsFile in e.OldItems)
				{
					gsFile.PropertyChanged -= GsFile_PropertyChanged;
				}
			}

			if (e.NewItems == null) return;
			foreach (GsFile gsFile in e.NewItems)
			{
				gsFile.PropertyChanged += GsFile_PropertyChanged;
			}
		}

		private void GsFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				OnPropertyChanged(nameof(AllFilesChecked));
				if (_wizardModel?.GsFiles != null)
				{
					IsValid = _wizardModel.GsFiles.Any(f => f.IsSelected);
				}
			}
		}

		private void FilesViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					try
					{
						ShowMessage("Please wait, we are loading GroupShare files", "#00A8EB");

						var selectedProjects = _wizardModel.GsProjects.Where(p => p.IsSelected).ToList();
						if (selectedProjects.Count > 0)
						{
							var addedProjects = selectedProjects.Except(_oldSelectedProjects).ToList();
							if (addedProjects.Count > 0)
							{
								AddFilesToGrid(addedProjects);
							}

							// get the removed projects
							var removedProjects = _oldSelectedProjects.Except(selectedProjects).ToList();
							if (removedProjects.Count > 0)
							{
								RemoveFilesFromGrid(removedProjects);
							}
						}
					}
					catch (Exception ex)
					{
						Log.Logger.Error($"FilesViewModel_PropertyChanged method: {ex.Message}\n {ex.StackTrace}");
					}
				}
			}
		}

		private async void AddFilesToGrid(List<GsProject> projects)
		{
			foreach (var project in projects)
			{
				_oldSelectedProjects.Add(project);
				var files = await _projectService.GetProjectFiles(project.ProjectId);
				SetFileProperties(project, files);
			}
		}

		private void RemoveFilesFromGrid(List<GsProject> removedProjects)
		{
			foreach (var removedProject in removedProjects)
			{
				var projectToBeRemoved = _oldSelectedProjects.FirstOrDefault(p => p.ProjectId.Equals(removedProject.ProjectId));
				if (projectToBeRemoved != null)
				{
					_oldSelectedProjects.Remove(projectToBeRemoved);

					//remove coresponding files for removed project from grid
					var filesToBeRemoved = _wizardModel?.GsFiles.Where(p => p.ProjectId.Equals(projectToBeRemoved.ProjectId)).ToList();
					if (filesToBeRemoved != null)
					{
						foreach (var file in filesToBeRemoved)
						{
							_wizardModel?.GsFiles.Remove(file);
						}
						OnPropertyChanged(nameof(GsFiles));
					}
				}
			}
		}

		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool AllFilesChecked
		{
			get => AreAllFilesSelected();
			set
			{
				ToggleCheckAllFiles(value);
				OnPropertyChanged(nameof(AllFilesChecked));
			}
		}

		public string TextMessage
		{
			get => _textMessage;
			set
			{
				_textMessage = value;
				OnPropertyChanged(nameof(TextMessage));
			}
		}
		public string TextMessageVisibility
		{
			get => _textMessageVisibility;
			set
			{
				_textMessageVisibility = value;
				OnPropertyChanged(nameof(TextMessageVisibility));
			}
		}
		public SolidColorBrush TextMessageBrush
		{
			get => _textMessageBrush;
			set
			{
				_textMessageBrush = value;
				OnPropertyChanged(nameof(TextMessageBrush));
			}
		}

		private void ToggleCheckAllFiles(bool value)
		{
			foreach (var gsFile in GsFiles)
			{
				gsFile.IsSelected = value;
			}
		}

		private bool AreAllFilesSelected()
		{
			return GsFiles?.Count > 0 && GsFiles.All(f => f.IsSelected);
		}

		public ObservableCollection<GsFile> GsFiles
		{
			get => _wizardModel?.GsFiles;
			set
			{
				_wizardModel.GsFiles = value;
				OnPropertyChanged(nameof(GsFiles));
			}
		}

		public override string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}

				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}
		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = PluginResources.UnableToNavigateToSelectedPage + Environment.NewLine + Environment.NewLine +
				          string.Format(PluginResources.The_data_on__0__is_not_valid, _displayName);
				return false;
			}

			return true;
		}

		private void SetFileProperties(GsProject project, IEnumerable<GsFile> files)
		{
			foreach (var gsFile in files)
			{
				if (gsFile.LanguageCode != project.SourceLanguage)
				{
					gsFile.ProjectId = project.ProjectId;
					gsFile.ProjectName = project.Name;
					gsFile.LanguageFlagImage = new Language(gsFile.LanguageCode).GetFlagImage();
					gsFile.LanguageName = new Language(gsFile.LanguageCode).DisplayName;
					_wizardModel?.GsFiles?.Add(gsFile);
				}
			}
			TextMessageVisibility = "Collapsed";
		}

		private void ShowMessage(string message, string color)
		{
			TextMessage = message;
			TextMessageVisibility = "Visible";
			TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
		}
	}
}
