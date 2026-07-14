using System;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;

namespace VerifyFilesAuditReport.BatchTasks;

public class ContentVerifier(VerificationMessageChannel messageChannel) : AbstractBilingualContentProcessor
{
    public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
    {
        if (paragraphUnit.IsStructure)
            return;

        foreach (var _ in paragraphUnit.SegmentPairs)
        {
            var message = messageChannel.TakeNext();
            if (message is null)
                return;

            ReportMessage(message.Source, message.ProjectFileName,
                GetErrorLevel(message.Level), message.Message, message.ProjectFileName);
        }
    }

    private static ErrorLevel GetErrorLevel(MessageLevel executionMessageLevel) =>
        executionMessageLevel switch
        {
            MessageLevel.Information => ErrorLevel.Note,
            MessageLevel.Warning => ErrorLevel.Warning,
            MessageLevel.Error => ErrorLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(executionMessageLevel), executionMessageLevel, null)
        };
}
