using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsControl, XliffManagerExportSettings>
	{
		private readonly ProjectsController _projectsController;
		private XliffManagerExportSettings _settings;
		private PathInfo _pathInfo;
		private ExportSettingsControl _control;

		public ExportSettingsPage()
		{
			_pathInfo = new PathInfo();
			_projectsController = GetProjectsController();
		}

		public override object GetControl()
		{			
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<XliffManagerExportSettings>();						
			_control = base.GetControl() as ExportSettingsControl;
			if (_control != null && _control.ExportOptionsViewModel == null)
			{
				CreateContext();
				_control.Settings = _settings;
				_control.SetDataContext();
			}

			return _control;
		}	

		private void CreateContext()
		{
			_settings.DateTimeStamp = new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Utc);
			var selectedProject = _projectsController?.SelectedProjects.FirstOrDefault()
								  ?? _projectsController?.CurrentProject;

			if (selectedProject != null)
			{
				var projectInfo = selectedProject.GetProjectInfo();
				_settings.LocalProjectFolder = projectInfo.LocalProjectFolder;
				_settings.TransactionFolder = GetDefaultTransactionPath(_settings.LocalProjectFolder, Enumerators.Action.Export);
			}

			_settings.ExportOptions = GetSettings().ExportOptions;
		}

		public override void Save()
		{
			base.Save();
			_settings = _control.Settings;
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

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private static ProjectsController GetProjectsController()
		{
			try
			{
				return SdlTradosStudio.Application.GetController<ProjectsController>();
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}
	}
}
