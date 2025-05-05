using LanguageWeaverProvider.Studio.BatchTask.ViewModel;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace LanguageWeaverProvider.Studio.BatchTask
{
    [AutomaticTask(Id = "Send Feedback to Language Weaver",
        Name = "Send Feedback to Language Weaver",
        Description = "Send Feedback to Language Weaver",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
        AllowMultiple = true)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    public class SendFeedbackBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            var processor = new SendFeedbackProcessor();
            multiFileConverter.AddBilingualProcessor(processor);
        }
    }
}