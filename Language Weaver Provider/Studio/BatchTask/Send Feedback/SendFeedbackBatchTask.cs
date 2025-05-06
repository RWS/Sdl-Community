using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

        public Dictionary<string, List<SendFeedbackError>> Errors { get; set; } = new();

        public override void TaskComplete()
        {
            var report = "";
            foreach (var key in Errors)
            {
                report += key.Key + "\r\n\r\n";
                report = key.Value.Aggregate(report, (current, error) => current + $"{error.SourceSegment} - {error.Error}");
            }

            Application.OpenForms[0]?.Invoke((MethodInvoker)(() =>
            {
                MessageBox.Show(report);
            }));
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            var processor = new SendFeedbackProcessor();
            multiFileConverter.AddBilingualProcessor(processor);
            Errors[$"{projectFile.Language}\\{projectFile.Name}"] = processor.Errors;
        }
    }
}