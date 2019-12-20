using System;
using System.IO;
using System.Linq;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.Settings;

namespace Sdl.Community.CleanUpTasks
{
	[AutomaticTask("Cleanup Target and Generate Files",
    "Cleanup Target and Generate Files",
    "Cleans up target segments and generates target files",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(CleanUpTargetSettings), typeof(CleanUpTargetSettingsPage))]
    public class CleanUpTargetTask : AbstractFileContentProcessingAutomaticTask
    {
        private const string BackupFolder = "Cleanup Backups";
        private IXmlReportGenerator reportGenerator = null;
        private string saveFolder = string.Empty;
        private CleanUpSourceSettings sourceSettings = null;
        private CleanUpTargetSettings targetSettings = null;

        public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            return true;
        }

        public override void TaskComplete()
        {
            if (targetSettings.SaveTarget)
            {
                if (!string.IsNullOrEmpty(targetSettings.SaveFolder))
                {
                    ExportFiles();
                }
            }

            if (targetSettings.OverwriteSdlXliff)
            {
                OverwriteSdlXliffs();
            }

            CreateReport("Cleanup Target Report", "Cleanup Target Batch Task Results", reportGenerator.ToString());
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            reportGenerator.AddFile(projectFile.LocalFilePath);
            multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new SaveToTargetPreProcessor(sourceSettings, targetSettings, reportGenerator)));
        }

        protected override void OnInitializeTask()
        {
            var settingsGroup = Project.GetSettings();
            sourceSettings = settingsGroup.GetSettingsGroup<CleanUpSourceSettings>();

            targetSettings = GetSetting<CleanUpTargetSettings>();

            if (targetSettings.MakeBackups)
            {
                BackupFiles();
            }

            var logFolder = Path.Combine(GetProjectFolder(), "Cleanup Logs");
            reportGenerator = new XmlReportGenerator(logFolder);
        }

        private void BackupFiles()
        {
            saveFolder = targetSettings.BackupsSaveFolder;

            if (string.IsNullOrEmpty(saveFolder))
            {
                var projectInfo = Project.GetProjectInfo();
                if (projectInfo != null)
                {
                    saveFolder = Path.Combine(projectInfo.LocalProjectFolder, BackupFolder);
                }
            }

            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            try
            {
                foreach (var file in TaskFiles)
                {
                    var savePath = Path.Combine(saveFolder, file.Name);

                    File.Copy(file.LocalFilePath, savePath, true);
                }
            }
            catch (Exception)
            {
                // TODO: Log and rethrow
            }
        }

        private void CopyGeneratedFiles()
        {
            var saveFolder = targetSettings.SaveFolder;

            if (Directory.Exists(saveFolder))
            {
                foreach (var file in TaskFiles)
                {
                    var savePath = Path.Combine(saveFolder, file.Name);

                    File.Copy(file.LocalFilePath, savePath, true);
                }
            }
        }

        private void ExportFiles()
        {
            var saveFolder = targetSettings.SaveFolder;

            if (Directory.Exists(saveFolder))
            {
                var settings = GetSetting<ExportFilesSettings>();

                // Store current settings so they can be restored later
                var exportLocation = settings.GetSetting<string>("ExportLocation");
                var fileVersion = settings.GetSetting<ExportFileVersion>("ExportFileVersion");

                settings.BeginEdit();
                settings.GetSetting<string>("ExportLocation").Value = saveFolder;
                settings.GetSetting<ExportFileVersion>("ExportFileVersion").Value = ExportFileVersion.Native;
                settings.EndEdit();

                Project.UpdateSettings(settings.SettingsBundle);

                try
                {
                    Project.RunAutomaticTask(TaskFiles.GetIds(), AutomaticTaskTemplateIds.ExportFiles);
                }
                finally
                {
                    // Restore
                    settings.BeginEdit();
                    settings.GetSetting<string>("ExportLocation").Value = exportLocation;
                    settings.GetSetting<ExportFileVersion>("ExportFileVersion").Value = fileVersion;
                    settings.EndEdit();

                    Project.UpdateSettings(settings.SettingsBundle);
                }
            }
        }

        private string GetProjectFolder()
        {
            var first = TaskFiles.FirstOrDefault();

            return Path.GetDirectoryName(first.LocalFilePath);
        }

        private void OverwriteSdlXliffs()
        {
            if (Directory.Exists(saveFolder))
            {
                try
                {
                    foreach (var file in TaskFiles)
                    {
                        var originalPath = Path.Combine(saveFolder, file.Name);

                        File.Copy(originalPath, file.LocalFilePath, true);
                    }
                }
                catch (Exception)
                {
                    // TODO: Log and rethrow
                }
            }
        }
    }
}