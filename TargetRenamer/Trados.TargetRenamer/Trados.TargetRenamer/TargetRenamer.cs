using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer
{
    [AutomaticTask("Target Renamer",
                   "Target Renamer",
                   "Description",
                   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(TargetRenamerSettingsViewModel), typeof(TargetRenamerSettingsPage))]
    public class TargetRenamer : AbstractFileContentProcessingAutomaticTask
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
