using System;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;

namespace VerifyFilesAuditReport.BatchTasks;

public class ContentVerifier : AbstractBilingualContentProcessor
{
    public ExecutionMessage[] Messages { get; set; }

    private bool BeenThereDoneThat { get; set; }

    public override void FileComplete()
    {
        if (BeenThereDoneThat) return;
        foreach (var executionMessage in Messages)
            ReportMessage(executionMessage.Source, executionMessage.ProjectFileName,
                GetErrorLevel(executionMessage.Level), executionMessage.Message, executionMessage.ProjectFileName);

        BeenThereDoneThat = true;
    }

    private ErrorLevel GetErrorLevel(MessageLevel executionMessageLevel)
    {
        switch (executionMessageLevel)
        {
            case MessageLevel.Information:
                return ErrorLevel.Note;

            case MessageLevel.Warning:
                return ErrorLevel.Warning;

            case MessageLevel.Error:
                return ErrorLevel.Error;

            default:
                throw new ArgumentOutOfRangeException(nameof(executionMessageLevel), executionMessageLevel, null);
        }
    }
}