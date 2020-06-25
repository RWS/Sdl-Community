using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsControl, ExportSettings>
	{		
		private readonly PathInfo _pathInfo;
		private readonly CustomerProvider _customerProvider;
		private readonly ImageService _imageService;
		private readonly ProjectsController _projectsController;
		private readonly XLIFFManagerViewController _xliffManagerController;
		private ExportSettings _settings;
		private ExportSettingsControl _control;
		private bool _validationCheckLoadedOnce;

		public ExportSettingsPage()
		{
			_projectsController = GetProjectsController();
			_xliffManagerController = GetXliffManagerController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
		}
		
		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<ExportSettings>();

			_control = base.GetControl() as ExportSettingsControl;
			if (_control != null)
			{				
				if (_control.ExportOptionsViewModel == null)
				{					
					CreateContext();
					_control.Settings = _settings;
					_control.SetDataContext();
				}
			}

			return _control;
		}
		
		private void CreateContext()
		{
			var selectedProject = _projectsController.SelectedProjects.FirstOrDefault()
			                      ?? _projectsController.CurrentProject;

			var projectInfo = selectedProject.GetProjectInfo();
			var selectedFileIds = new List<string>(); //TODO: Identify if we can get the selection from the user

			_settings.LocalProjectFolder = projectInfo.LocalProjectFolder;
			_settings.TransactionFolder = GetDefaultTransactionPath(_settings.LocalProjectFolder, Enumerators.Action.Export);

			var exportOptions = new ExportOptions
			{
				XliffSupport = Enumerators.XLIFFSupport.xliff12sdl,
				IncludeTranslations = true,
				CopySourceToTarget = true
			};

			_settings.ExportOptions = exportOptions;

			var projectModel = GetProjectModel(selectedProject, selectedFileIds);
			_settings.ProjectFiles = projectModel.ProjectFiles;
		}
	
		public override void Save()
		{
			base.Save();

			_settings = _control.Settings;
		}

		public override bool ValidateInput()
		{
			if (!_validationCheckLoadedOnce)
			{
				_validationCheckLoadedOnce = true;
				return base.ValidateInput();
			}

			var isValid = Settings.ProjectFiles.Count(a => a.Selected) > 0;
			if (!isValid)
			{
				MessageBox.Show(PluginResources.Message_NoProjectFilesSelected, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);				
			}

			return isValid;
		}

		private Project GetProjectModel(FileBasedProject selectedProject, IReadOnlyCollection<string> selectedFileIds)
		{
			if (selectedProject == null)
			{
				return null;
			}

			var projectInfo = selectedProject.GetProjectInfo();

			var project = new Project
			{
				Id = projectInfo.Id.ToString(),
				Name = projectInfo.Name,
				AbsoluteUri = projectInfo.Uri.AbsoluteUri,
				Customer = _customerProvider.GetProjectCustomer(selectedProject),
				Created = projectInfo.CreatedAt,
				DueDate = projectInfo.DueDate ?? DateTime.MaxValue,
				Path = projectInfo.LocalProjectFolder,
				SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
				TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
				ProjectType = GetProjectType(selectedProject)
			};

			var existingProject = _xliffManagerController.GetProjects().FirstOrDefault(a => a.Id == projectInfo.Id.ToString());

			if (existingProject != null)
			{
				foreach (var projectFile in existingProject.ProjectFiles)
				{
					if (projectFile.Clone() is ProjectFile clonedProjectFile)
					{
						//clonedProjectFile.Project = project;
						clonedProjectFile.Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.FileId.ToString());
						project.ProjectFiles.Add(clonedProjectFile);
					}
				}
			}
			else
			{
				project.ProjectFiles = GetProjectFiles(selectedProject, project, selectedFileIds);
			}

			return project;
		}

		private static List<ProjectFile> GetProjectFiles(IProject project, Project projectModel, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectInfo = project.GetProjectInfo();
			var projectFiles = new List<ProjectFile>();

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var languageFiles = project.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in languageFiles)
				{
					if (projectFile.Role != FileRole.Translatable)
					{
						continue;
					}

					var projectFileModel = GetProjectFile(projectModel, projectFile, targetLanguage, selectedFileIds);
					projectFiles.Add(projectFileModel);
				}
			}

			return projectFiles;
		}

		private static ProjectFile GetProjectFile(Project project, ProjectAutomation.Core.ProjectFile projectFile,
			Language targetLanguage, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectFileModel = new ProjectFile
			{
				ProjectId = project.Id,
				FileId = projectFile.Id.ToString(),
				Name = projectFile.Name,
				Path = projectFile.Folder,
				Location = projectFile.LocalFilePath,
				Action = Enumerators.Action.None,
				Status = Enumerators.Status.Ready,
				Date = DateTime.MinValue,
				TargetLanguage = targetLanguage.CultureInfo.Name,
				Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString()),
				FileType = projectFile.FileTypeId,
				//Project = project
			};

			return projectFileModel;
		}

		private List<LanguageInfo> GetLanguageInfos(IEnumerable<Language> languages)
		{
			var targetLanguages = new List<LanguageInfo>();
			foreach (var targetLanguage in languages)
			{
				targetLanguages.Add(GetLanguageInfo(targetLanguage.CultureInfo));
			}

			return targetLanguages;
		}

		private static string GetProjectType(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				return internalDynamicProject.ProjectType.ToString();
			}

			return null;
		}

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var languageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				Image = _imageService.GetImage(cultureInfo.Name)
			};

			return languageInfo;
		}

		public string GetDefaultTransactionPath(string localProjectFolder, Enumerators.Action action)
		{
			var rootPath = Path.Combine(localProjectFolder, "XLIFF.Manager");
			var path = Path.Combine(rootPath, action.ToString());

			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		private static ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		private static XLIFFManagerViewController GetXliffManagerController()
		{
			return SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
		}
	}
}
