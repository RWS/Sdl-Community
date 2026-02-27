//namespace VerifyFilesAuditReport.BatchTasks.UI;

//using System;
//using System.Threading;
//using System.Threading.Tasks;

//public sealed class ControlledTask : IDisposable
//{
//    private readonly Func<CancellationToken, Task> _work;
//    private readonly CancellationTokenSource _cts = new();
//    private readonly ManualResetEventSlim _pause = new(false);
//    private Task? _runner;
//    private TaskCompletionSource<bool>? _pausedTcs;

//    public ControlledTask(Func<CancellationToken, Task> work)
//    {
//        _work = work;
//        Start();
//    }

//    public void Start()
//    {
//        if (_runner != null)
//            return;

//        _runner = Task.Run(async () =>
//        {
//            while (!_cts.Token.IsCancellationRequested)
//            {
//                // If paused, create a TCS
//                if (!_pause.IsSet && _pausedTcs == null)
//                    _pausedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

//                // Wait until resumed
//                _pause.Wait(_cts.Token);

//                // Signal that pause is over (or we are running)
//                _pausedTcs?.TrySetResult(true);
//                _pausedTcs = null;

//                await _work(_cts.Token);
//            }
//        }, _cts.Token);
//    }

//    public void Pause() => _pause.Reset();

//    public void Resume() => _pause.Set();

//    // External code can call this
//    public Task WaitUntilPausedAsync()
//    {
//        if (_pause.IsSet)
//        {
//            // Prepare to pause so loop will hit Wait
//            _pausedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
//            _pause.Reset();
//        }

//        return _pausedTcs?.Task ?? Task.CompletedTask;
//    }

//    public void Stop() => _cts.Cancel();

//    public void Dispose()
//    {
//        _cts.Cancel();
//        _pause.Set();
//        _pause.Dispose();
//        _cts.Dispose();
//    }
//}