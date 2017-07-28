using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using System.IO;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [AutomaticTask("Coding.ProjectTerms.BatchTask",
        "Project Terms",
        "Export project terms according the following criteria...",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(ProjectTermsBatchTaskSettings), typeof(ProjectTermsBatchTaskSettingsPage))]
    public class ProjectTermsBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        private HashSet<string> projectFiles;
        private ProjectTermsBatchTaskSettingsControl control;
        private ProjectTermsBatchTaskSettings settings;

        protected override void OnInitializeTask()
        {
            base.OnInitializeTask();

            projectFiles = new HashSet<string>();
            control = new ProjectTermsBatchTaskSettingsControl();
            settings = GetSetting<ProjectTermsBatchTaskSettings>();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (projectFiles.Contains(projectFile.Name)) return;

            control.ExtractProjectFileTerms(projectFile, multiFileConverter);
            projectFiles.Add(projectFile.Name);
        }

        public override void TaskComplete()
        {
            control.ExtractProjectTerms(settings);

            AddXMlToProject(Project, Path.GetDirectoryName(ProjectTermsCache.GetXMLFilePath(control.ProjectPath)), false);
        }

        private void AddXMlToProject(IProject project, string xmlFolder, bool recursion)
        {
            project.AddFolderWithFiles(xmlFolder, recursion);
            ProjectFile[] projectFiles = project.GetSourceLanguageFiles();
            AutomaticTask scan = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            AutomaticTask convertTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.ConvertToTranslatableFormat);
            AutomaticTask copyTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.CopyToTargetLanguages);
        }
    }
}
