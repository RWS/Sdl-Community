using LanguageWeaverProvider.Studio.BatchTask.Send_Feedback.Controls;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback
{
    [AutomaticTask(Id = "Send Feedback to Language Weaver",
        Name = "Send Feedback to Language Weaver",
        Description = "Send Feedback to Language Weaver",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
        AllowMultiple = true)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    public class SendFeedbackBatchTask : AbstractFileContentProcessingAutomaticTask
    {

        public List<FileErrors> Errors { get; set; } = new();

        public override void TaskComplete()
        {
            var currentApp = Application.Current;
            currentApp?.Dispatcher.Invoke(() =>
            {
                var batchProcessingWindow = ApplicationInitializer.GetBatchTaskWindow();
                var errorWindow = new ErrorWindow(Errors)
                {
                    Owner = batchProcessingWindow
                };
                errorWindow.ShowDialog();
            });
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            var processor = new SendFeedbackProcessor();
            multiFileConverter.AddBilingualProcessor(processor);

            Errors.Add(new FileErrors
            {
                Filename = $"{projectFile.Language}\\{projectFile.Name}", SegmentErrors = processor.Errors
            });
        }
    }
}