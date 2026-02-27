using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;
using System;

namespace VerifyFilesAuditReport.BatchTasks;

public class ContentVerifier : AbstractBilingualContentProcessor
{
    public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
    {
        if (paragraphUnit.IsStructure)
            return;
        foreach (var _ in paragraphUnit.SegmentPairs)
        {
            var message = Signal.GetMessage();
            if (message is null)
                return;
            ReportMessage(message.Source, message.ProjectFileName,
                GetErrorLevel(message.Level), message.Message, message.ProjectFileName);
        }
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