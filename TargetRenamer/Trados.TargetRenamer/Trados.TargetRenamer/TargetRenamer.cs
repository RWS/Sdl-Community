using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.Services;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer
{
	[AutomaticTask("Target Renamer",
		"TargetRenamer_Name",
		"TargetRenamer_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(TargetRenamerSettingsViewModel), typeof(TargetRenamerSettingsPage))]
	public class TargetRenamer : AbstractFileContentProcessingAutomaticTask
	{
		private List<ProjectFile> _projectFiles;
		private Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>> _renamedFiles;
		private ReportCreatorService _reportCreator;
		private TargetRenamerSettings _settings;

		protected override void OnInitializeTask()
		{
			_reportCreator = new ReportCreatorService();
			_projectFiles = new List<ProjectFile>();
			_renamedFiles = new Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>>();
			_settings = GetSetting<TargetRenamerSettings>();
			base.OnInitializeTask();
		}

		public override void TaskComplete()
		{
			base.TaskComplete();

			GenerateReports();
		}

		private void GenerateReports()
		{
			var languageDirections = _projectFiles
				.GroupBy(p => new {p.GetLanguageDirection().TargetLanguage, p.GetLanguageDirection().SourceLanguage})
				.Select(g => g.First().GetLanguageDirection());

			foreach (var languageDirection in languageDirections)
			{
				var reportName =
					$"{PluginResources.TargetRenamer_Name}_{languageDirection.SourceLanguage.IsoAbbreviation}_{languageDirection.TargetLanguage.IsoAbbreviation}";

				var projectFiles = _projectFiles
					.Where(x => x.GetLanguageDirection().TargetLanguage == languageDirection.TargetLanguage).ToList();
				_reportCreator.CreateReport(Project, projectFiles, _renamedFiles, _settings, languageDirection);

				CreateReport(reportName, PluginResources.ReportDescription, _reportCreator.reportFile, languageDirection);
			}
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			_projectFiles.Add(projectFile);

			var fileIds = new List<Guid>(){ projectFile.Id };
			var task = Project.RunAutomaticTask(fileIds.ToArray(), AutomaticTaskTemplateIds.GenerateTargetTranslations);
			if (task == null || task.Status != TaskStatus.Completed)
				throw new Exception(task?.Messages?.FirstOrDefault()?.Message ?? "Task has not been run correctly.");

			var targetLanguage = new CultureInfo(projectFile.GetLanguageDirection().TargetLanguage.IsoAbbreviation);
			var files = Project.GetTargetLanguageFiles(projectFile.GetLanguageDirection().TargetLanguage);
			var file = files.SingleOrDefault(x => x.SourceFile.Name.Equals(projectFile.Name));

			var filePath = Path.GetDirectoryName(file.LocalFilePath);
			var fileExtension = Path.GetExtension(file.LocalFilePath);
			var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
			var oldFileName = Path.GetFileName(file.LocalFilePath);
			string newFileName;
			if (_settings.AppendAsPrefix) fileName = CreateFileNameWithPrefix(fileName, fileExtension, targetLanguage);

			if (_settings.AppendAsSuffix) fileName = CreateFileNameWithSuffix(fileName, fileExtension, targetLanguage);

			if (_settings.UseRegularExpression)
				fileName = CreateFileNameWithRegularExpression(fileName, fileExtension);

			if (_settings.UseCustomLocation)
			{
				if (!Directory.Exists(_settings.CustomLocation))
					Directory.CreateDirectory(_settings.CustomLocation ??
					                          throw new InvalidOperationException("Invalid file path!"));
				newFileName = Path.Combine(_settings.CustomLocation, fileName);
			}
			else
			{
				newFileName = Path.Combine(filePath, fileName);
			}

			_renamedFiles.Add(
				(projectFile, projectFile.GetLanguageDirection()),
				new Tuple<string, string>(oldFileName, fileName));

			if (_settings.OverwriteTargetFiles)
			{
				// Ensure that the file does not exist
				if (File.Exists(newFileName)) File.Delete(newFileName);
				File.Move(file.LocalFilePath, newFileName);
			}
			else
			{
				File.Copy(file.LocalFilePath, newFileName);
			}
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
	}
}