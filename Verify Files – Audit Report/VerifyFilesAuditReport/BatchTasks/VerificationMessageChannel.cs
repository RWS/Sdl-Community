using System;
using System.Collections.Concurrent;
using System.Threading;
using Sdl.ProjectAutomation.Core;

namespace VerifyFilesAuditReport.BatchTasks;

/// <summary>
/// Relays messages produced by the background Verify Files run to the
/// <see cref="ContentVerifier"/> consuming them on the batch task's processing thread.
/// </summary>
public sealed class VerificationMessageChannel : IDisposable
{
    private readonly BlockingCollection<ExecutionMessage> _messages = new();

    /// <summary>Blocks until a message is available; returns null once the channel is completed and drained.</summary>
    public ExecutionMessage TakeNext() =>
        _messages.TryTake(out var message, Timeout.Infinite) ? message : null;

    public void Post(ExecutionMessage message)
    {
        try
        {
            _messages.Add(message);
        }
        catch (InvalidOperationException)
        {
            // The channel was completed before this message arrived; drop it.
        }
    }

    public void Complete() => _messages.CompleteAdding();

    public void Dispose() => _messages.Dispose();
}
