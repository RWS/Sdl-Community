using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	    private TargetRenamerSettings _settings;
	    private List<ProjectFile> _projectFiles;
	    private ReportCreatorService _reportCreator;
	    private Dictionary<ProjectFile, Tuple<string, string>> _renamedFiles;
        protected override void OnInitializeTask()
        {
	        _reportCreator = new ReportCreatorService();
	        _projectFiles = new List<ProjectFile>();
	        _renamedFiles = new Dictionary<ProjectFile, Tuple<string, string>>();
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
	        var languageDirections = new List<LanguageDirection>();
	        foreach (var projectFile in _projectFiles)
	        {
		        if (!languageDirections.Contains(projectFile.GetLanguageDirection()))
		        {
			        languageDirections.Add(projectFile.GetLanguageDirection());
		        }
	        }

	        foreach (var languageDirection in languageDirections)
	        {
		        var reportName =
			        $"{PluginResources.TargetRenamer_Name}_{languageDirection.SourceLanguage.IsoAbbreviation}_{languageDirection.TargetLanguage.IsoAbbreviation}";

		        var projectFiles = _projectFiles
			        .Where(x => x.GetLanguageDirection().TargetLanguage == languageDirection.TargetLanguage).ToList();
		        _reportCreator.CreateReport(Project, projectFiles, _renamedFiles, _settings, languageDirection);

		        CreateReport(reportName, PluginResources.ReportDescription, _reportCreator.reportFile);
	        }
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
			_projectFiles.Add(projectFile);
	        var fileIds = new List<Guid> {projectFile.Id};
	        var task = Project.RunAutomaticTask(fileIds.ToArray(), AutomaticTaskTemplateIds.GenerateTargetTranslations);
			
	        if (task == null || task.Status != TaskStatus.Completed)
	        {
		        throw new Exception(task?.Messages?.FirstOrDefault()?.Message ?? "Task has not been run correctly.");
	        }
			
			var targetLanguage = projectFile.GetLanguageDirection().TargetLanguage.IsoAbbreviation.ToString();
	        
	        var files = Project.GetTargetLanguageFiles(projectFile.GetLanguageDirection().TargetLanguage);
	        
	        foreach (var file in files)
	        {
		        var filePath = Path.GetDirectoryName(file.LocalFilePath);
				var fileExtension = Path.GetExtension(file.LocalFilePath);
				var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
				var oldFileName = Path.GetFileName(file.LocalFilePath);
				string newFileName;

				if (_settings.AppendAsPrefix)
				{
					fileName = CreateFileNameWithPrefix(fileName, fileExtension, targetLanguage);
				}

				if (_settings.AppendAsSuffix)
				{
					fileName = CreateFileNameWithSuffix(fileName, fileExtension, targetLanguage);
				}

				if (_settings.UseRegularExpression)
				{
					fileName = CreateFileNameWithRegularExpression(fileName, fileExtension, targetLanguage);
				}

				if (_settings.UseCustomLocation)
				{
					if (!Directory.Exists(_settings.CustomLocation))
					{
						Directory.CreateDirectory(_settings.CustomLocation ?? throw new InvalidOperationException("Invalid file path!"));
					}
					newFileName = Path.Combine(_settings.CustomLocation, fileName);
				}
				else
				{
					newFileName = Path.Combine(filePath, fileName);
				}

				_renamedFiles.Add(projectFile, new Tuple<string, string>(oldFileName, fileName));

				if (_settings.OverwriteTargetFiles)
				{
					// Ensure that the file does not exist
					if (File.Exists(newFileName))
					{
						File.Delete(newFileName);
					}
					File.Move(file.LocalFilePath, newFileName);
				}
				else
				{
					File.Copy(file.LocalFilePath, newFileName);
				}
			}
        }

        private string CreateFileNameWithRegularExpression(string newFileName, string fileExtension, string targetLanguage)
        {
			// Maybe add an option to IgnoreCase?
	        var newName = Regex.Replace(newFileName, _settings.RegularExpressionSearchFor,
		        _settings.RegularExpressionReplaceWith) + fileExtension;
	        return newName;
        }

        private string CreateFileNameWithSuffix(string newFileName, string fileExtension, string targetLanguage)
        {
			if (_settings.AppendTargetLanguage)
			{
				if (_settings.UseShortLocales)
				{
					newFileName = newFileName + _settings.Delimiter + targetLanguage.Substring(0, 2) + fileExtension;
				}
				else
				{
					newFileName = newFileName + _settings.Delimiter + targetLanguage + fileExtension;
				}
			}
			else if (_settings.AppendCustomString)
			{
				newFileName = newFileName + _settings.Delimiter + _settings.CustomString + fileExtension;
			}

			return newFileName;
		}

        private string CreateFileNameWithPrefix(string newFileName, string fileExtension, string targetLanguage)
        {
	        if (_settings.AppendTargetLanguage)
	        {
		        if (_settings.UseShortLocales)
		        {
			        newFileName = targetLanguage.Substring(0, 2) + _settings.Delimiter + newFileName +
			                      fileExtension;
		        }
		        else
		        {
			        newFileName = targetLanguage + _settings.Delimiter + newFileName + fileExtension;
		        }
	        }
	        else if (_settings.AppendCustomString)
	        {
		        newFileName = _settings.CustomString + _settings.Delimiter + newFileName + fileExtension;
	        }

	        return newFileName;
        }
    }
}
