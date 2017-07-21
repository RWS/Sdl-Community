using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;

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
        }
    }
}
