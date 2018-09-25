using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace ExportToExcel
{
    /// <summary>
    /// Automatic task attribute
    /// Parameters: task name, plugin name and plugin description
    /// </summary>
    [AutomaticTask("Export to Excel",
        "Export to Excel",
        "Exports a file in Excel format",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof (GeneratorSettings), typeof (ExportToExcelSettingsPage))]

    public class ExportToExcelTask : AbstractFileContentProcessingAutomaticTask
    {
        private GeneratorSettings _settings;

        /// <summary>
        /// Initialize settings for the project
        /// </summary>
        protected override void OnInitializeTask()
        {          
            _settings = GetSetting<GeneratorSettings>();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            var worker = new Worker(_settings);
            worker.GeneratePreviewFiles(projectFile.LocalFilePath, multiFileConverter);
        }
    }
}