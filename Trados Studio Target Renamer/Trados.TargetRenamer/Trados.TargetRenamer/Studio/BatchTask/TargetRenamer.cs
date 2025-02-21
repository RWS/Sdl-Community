using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.Helpers;
using Trados.TargetRenamer.Services;

namespace Trados.TargetRenamer.Studio.BatchTask
{
	[AutomaticTask(Constants.PluginName,
		Constants.PluginName,
		"TargetRenamer_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(TargetRenamerSettings), typeof(TargetRenamerSettingsPage))]
	public class TargetRenamer : AbstractFileContentProcessingAutomaticTask
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private List<ProjectFile> _projectFiles;
		private Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>> _renamedFiles;
		private ReportCreatorService _reportCreator;
		private TargetRenamerSettings _settings;

		public override void TaskComplete()
		{
			base.TaskComplete();
			GenerateReports();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			_projectFiles.Add(projectFile);
			_settings = Project.GetSettings(projectFile.Language).GetSettingsGroup<TargetRenamerSettings>();

			var fileIds = new List<Guid>() { projectFile.Id };
			var task = Project.RunAutomaticTask(fileIds.ToArray(), AutomaticTaskTemplateIds.GenerateTargetTranslations);
			if (task == null || task.Status != TaskStatus.Completed)
				_logger.Error($"{task?.Messages?.FirstOrDefault()?.Message}\n The task has not been run correctly.");

			var targetLanguage = new CultureInfo(projectFile.GetLanguageDirection().TargetLanguage.IsoAbbreviation);
			var files = Project.GetTargetLanguageFiles(projectFile.GetLanguageDirection().TargetLanguage);
			var file = files.SingleOrDefault(x => x.SourceFile.Id.Equals(projectFile.SourceFile.Id));
			var filePath = Path.GetDirectoryName(file.LocalFilePath);
			var fileExtension = Path.GetExtension(file.LocalFilePath);
			var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
			var oldFileName = Path.GetFileName(file.LocalFilePath);
			if (_settings.AppendAsPrefix) fileName = CreateFileNameWithPrefix(fileName, fileExtension, targetLanguage);

			if (_settings.AppendAsSuffix) fileName = CreateFileNameWithSuffix(fileName, fileExtension, targetLanguage);

			if (_settings.UseRegularExpression)
				fileName = CreateFileNameWithRegularExpression(fileName, fileExtension);

			try
			{
				string newFileName;
				if (_settings.UseCustomLocation)
				{
					if (!Directory.Exists(_settings.CustomLocation))
						Directory.CreateDirectory(_settings.CustomLocation ??
												  throw new InvalidOperationException("Invalid file path!"));
					newFileName = Path.Combine(_settings.CustomLocation, targetLanguage.Name, projectFile.Folder, fileName);
				}
				else
				{
					newFileName = Path.Combine(filePath, fileName);
				}

				_renamedFiles.Add(
				(projectFile, projectFile.GetLanguageDirection()),
				new Tuple<string, string>(oldFileName, fileName));

				// Ensure that the file does not exist
				if (File.Exists(newFileName))
				{
					File.Delete(newFileName);
				}

				Directory.CreateDirectory(Path.Combine(_settings.CustomLocation, targetLanguage.Name,
						projectFile.Folder));

				File.Move(file.LocalFilePath, newFileName);
			}
			catch (Exception e)
			{
				_logger.Error($"{e.Message}\n {e.StackTrace}");
			}

			RevertToSdlXliff(projectFile);
		}

		protected override void OnInitializeTask()
		{
			Log.Setup();
			_reportCreator = new ReportCreatorService();
			_projectFiles = new List<ProjectFile>();
			_renamedFiles = new Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>>();
			base.OnInitializeTask();
		}

		private string CreateFileNameWithPrefix(string newFileName, string fileExtension, CultureInfo targetLanguage)
		{
			if (_settings.AppendTargetLanguage)
			{
				if (_settings.UseShortLocales)
					newFileName = targetLanguage.TwoLetterISOLanguageName + _settings.Delimiter + newFileName +
								  fileExtension;
				else
					newFileName = targetLanguage.Name + _settings.Delimiter + newFileName + fileExtension;
			}
			else if (_settings.AppendCustomString)
			{
				newFileName = _settings.CustomString + _settings.Delimiter + newFileName + fileExtension;
			}

			return newFileName;
		}

		private string CreateFileNameWithRegularExpression(string newFileName, string fileExtension)
		{
			// Maybe add an option to IgnoreCase?
			var newName = Regex.Replace(newFileName, _settings.RegularExpressionSearchFor,
				_settings.RegularExpressionReplaceWith) + fileExtension;
			return newName;
		}

		private string CreateFileNameWithSuffix(string newFileName, string fileExtension, CultureInfo targetLanguage)
		{
			if (_settings.AppendTargetLanguage)
			{
				if (_settings.UseShortLocales)
					newFileName = newFileName + _settings.Delimiter + targetLanguage.TwoLetterISOLanguageName + fileExtension;
				else
					newFileName = newFileName + _settings.Delimiter + targetLanguage.Name + fileExtension;
			}
			else if (_settings.AppendCustomString)
			{
				newFileName = newFileName + _settings.Delimiter + _settings.CustomString + fileExtension;
			}

			return newFileName;
		}

		private void GenerateReports()
		{
			var languageDirections = _projectFiles
				.GroupBy(p => new { p.GetLanguageDirection().TargetLanguage, p.GetLanguageDirection().SourceLanguage })
				.Select(g => g.First().GetLanguageDirection());

			foreach (var languageDirection in languageDirections)
			{
				_settings = Project.GetSettings(languageDirection.TargetLanguage).GetSettingsGroup<TargetRenamerSettings>();
				var reportName =
					$"{Constants.PluginName}_{languageDirection.SourceLanguage.IsoAbbreviation}_{languageDirection.TargetLanguage.IsoAbbreviation}";

				var projectFiles = _projectFiles
					.Where(x => Equals(x.GetLanguageDirection().TargetLanguage, languageDirection.TargetLanguage)).ToList();
				_reportCreator.CreateReport(Project, projectFiles, _renamedFiles, _settings, languageDirection);

				CreateReport(reportName, PluginResources.ReportDescription, _reportCreator.ReportFile, languageDirection);
			}
		}

		private void RevertToSdlXliff(ProjectFile projectFile)
		{
			Project.AddNewFileVersion(projectFile.Id, projectFile.LocalFilePath);
			var projectFileDirectory = Path.GetDirectoryName(projectFile.LocalFilePath);
			if (projectFileDirectory == null) return;

			var latestFile = Directory.GetFiles(projectFileDirectory).ToList().OrderByDescending(File.GetCreationTime).FirstOrDefault();
			if (string.IsNullOrWhiteSpace(Path.GetExtension(latestFile)) && Path.GetFileNameWithoutExtension(projectFile.OriginalName) == Path.GetFileNameWithoutExtension(latestFile) && latestFile != null)
			{
				File.Delete(latestFile);
			}
		}
	}
}