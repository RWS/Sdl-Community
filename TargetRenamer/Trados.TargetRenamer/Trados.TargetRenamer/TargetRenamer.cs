using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.IO;
using System.Text.RegularExpressions;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer
{
    [AutomaticTask("Target Renamer",
				   "TargetRenamer_Name",
				   "TargetRenamer_Description",
                   GeneratedFileType = AutomaticTaskFileType.NativeTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.NativeTarget)]
    [RequiresSettings(typeof(TargetRenamerSettingsViewModel), typeof(TargetRenamerSettingsPage))]
    public class TargetRenamer : AbstractFileContentProcessingAutomaticTask
    {
	    private TargetRenamerSettings _settings;

        protected override void OnInitializeTask()
        {
	        _settings = GetSetting<TargetRenamerSettings>();
            base.OnInitializeTask();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
	        var targetLanguage = projectFile.GetLanguageDirection().TargetLanguage.IsoAbbreviation.ToString();
	        var file = projectFile.LocalFilePath;
	        var fileName = Path.GetFileNameWithoutExtension(file);
	        var filePath = Path.GetDirectoryName(file);
	        var fileExtension = Path.GetExtension(file);
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
		        newFileName = Path.Combine(_settings.CustomLocation, fileName);
	        }
	        else
	        {
				newFileName = Path.Combine(filePath, fileName);
	        }

	        if (_settings.OverwriteTargetFiles)
	        {
				File.Move(file, newFileName);
	        }
	        else
	        {
		        File.Copy(file, newFileName);
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
					newFileName = newFileName + _settings.Delimitator + targetLanguage.Substring(0, 2) + fileExtension;
				}
				else
				{
					newFileName = newFileName + _settings.Delimitator + targetLanguage.Substring(0,2) + fileExtension;
				}
			}
			else if (_settings.AppendCustomString)
			{
				newFileName = newFileName + _settings.Delimitator + _settings.CustomString + fileExtension;
			}

			return newFileName;
		}

        private string CreateFileNameWithPrefix(string newFileName, string fileExtension, string targetLanguage)
        {
	        if (_settings.AppendTargetLanguage)
	        {
		        if (_settings.UseShortLocales)
		        {
			        newFileName = targetLanguage.Substring(0, 2) + _settings.Delimitator + newFileName +
			                      fileExtension;
		        }
		        else
		        {
			        newFileName = targetLanguage + _settings.Delimitator + newFileName + fileExtension;
		        }
	        }
	        else if (_settings.AppendCustomString)
	        {
		        newFileName = _settings.CustomString + _settings.Delimitator + newFileName + fileExtension;
	        }

	        return newFileName;
        }
    }
}
