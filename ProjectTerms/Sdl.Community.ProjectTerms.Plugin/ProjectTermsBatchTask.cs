using Sdl.ProjectAutomation.AutomaticTasks;
using System;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [AutomaticTask("Coding.ProjectTerms.BatchTask",
        "Project Terms",
        "Export project terms according the following criteria...",
        //[TODO] You can change the file type according to your needs
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    //[TODO] You can change the file type according to your needs
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(ProjectTermsBatchTaskSettings), typeof(ProjectTermsBatchTaskSettingsPage))]
    public class ProjectTermsBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        protected override void OnInitializeTask()
        {
            base.OnInitializeTask();
        }
        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            //In here you should add your custom bilingual processor to the file converter
            throw new NotImplementedException();
        }
    }
}
