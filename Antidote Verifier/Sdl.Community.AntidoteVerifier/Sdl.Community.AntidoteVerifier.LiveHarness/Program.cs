using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;
using Sdl.Community.AntidoteVerifier;
using Sdl.Community.AntidoteVerifier.Connectix;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;

namespace Sdl.Community.AntidoteVerifier.LiveHarness
{
    /// <summary>
    /// End-to-end harness for the plugin's real launch path, runnable WITHOUT Trados Studio. It calls the
    /// actual <see cref="ConnectixSession.Launch(IEditorService, string)"/> the ribbon uses, from a real WPF
    /// <see cref="Application"/>/<see cref="Dispatcher"/> UI thread, so it exercises the persistent
    /// <see cref="ConnectixSession"/> singleton, its background connect thread, AND the UI-thread
    /// marshalling the plugin depends on in Studio.
    ///
    /// EXPERIMENT: capture what AgentConnectix actually sends over the single persistent connection when
    /// the user switches correction tabs INSIDE Antidote and applies corrections there — the traffic behind
    /// the "wrong Studio document scrolls/selects" limitation flagged for Druide. The run is split into
    /// labelled capture windows (launch A, launch B, switch to tab A, correct in tab A, correct in tab B,
    /// relaunch A) so every raw frame and every editor call can be attributed to the operator action that
    /// triggered it. At the end the harness prints, per window, the inbound message types received and
    /// WHICH document's editor received select/replace/canReplace calls — if a correction applied in tab A
    /// lands on document B's editor, the misroute is in the summary.
    ///
    /// FIDELITY — how this matches what Studio really does (it is protocol evidence only if it does):
    ///  - Launches go through the production stack (ConnectixSession -> StudioWordProcessorAgent ->
    ///    ConnectixAgent -> ClientWebSocketTransport); nothing protocol-side is faked. Zone ids, the init
    ///    configuration (cacheIdType "forcePath", markup "text", replaceWithoutSelection), and framing are
    ///    all the plugin's own.
    ///  - A FRESH editor service is created for EVERY launch, exactly like AntidoteLauncher.Launch does
    ///    (new EditorService(activeDocument) per ribbon click) — the previous harness reused one mutable
    ///    editor, which is not what Studio does.
    ///  - <see cref="StudioLikeEditorService"/> mirrors the observable behavior of the real EditorService,
    ///    including throwing KeyNotFoundException for a segment index outside the document (the real one
    ///    indexes _segmentMetadata[index]). That matters here: a misrouted callback whose zone id does not
    ///    exist in the retargeted document THROWS in Studio, ConnectixAgent logs the error and sends NO
    ///    response — a null-returning fake would instead answer "false" and hide that behavior.
    ///  - Document paths point at REAL files created on disk (Studio hands Antidote the sdlxliff's
    ///    LocalFilePath, which exists). Paths are stable across runs, like a Studio project's files.
    ///  - Both documents' segments differ in count and text, so even an un-attributable callback can be
    ///    matched to a document by its positions/context in the raw log.
    ///
    /// Every frame is also written verbatim to a timestamped capture file (path printed at start and end)
    /// for attaching to the Druide escalation.
    ///
    /// Prerequisites: Antidote / AgentConnectix installed locally (the same install Studio uses), with the
    /// French module (the sample segments contain deliberate French errors so the Corrector shows
    /// detections to click). Studio itself does NOT need to be running.
    /// </summary>
    internal static class Program
    {
        private static readonly Logger PhaseLogger = LogManager.GetLogger("LiveHarness.Phase");

        private static string _captureFilePath;
        private static Window _hostWindow;

        // Brings the host window to the foreground before a launch, the way Studio's main window is
        // foreground when the user clicks the ribbon. Antidote reads the launching app's window context.
        private static void ActivateHostWindow()
        {
            var dispatcher = Application.Current?.Dispatcher;
            dispatcher?.Invoke(() => _hostWindow?.Activate());
            Thread.Sleep(300);
        }

        // Deliberate French errors so the Corrector flags detections the operator can apply. Different
        // segment counts and lengths per document, so positions alone can identify the target document.
        private static readonly string[] SegmentsA =
        {
            "Les chien noir mange une pomme rouge.",
            "Elle ont manger tout les gâteau hier soir."
        };

        private static readonly string[] SegmentsB =
        {
            "Je veut aller au cinéma avec mes ami demain.",
            "Cette maison sont très grande et lumineuse.",
            "Il faut que tu viens me voir la semaine prochaine."
        };

        // HARNESS_SEGMENTS_A / HARNESS_SEGMENTS_B ("|"-separated) replace the sample segments above, so a
        // specific text can be put on the wire -- e.g. bisecting whether Antidote's own analysis or the
        // plugin's extraction is responsible for a detection (SRQ-32047) -- without editing the harness.
        private static string[] SegmentsOverride(string environmentVariable)
        {
            var value = Environment.GetEnvironmentVariable(environmentVariable);
            return string.IsNullOrEmpty(value) ? null : value.Split('|');
        }

        // In --step mode each pause waits for this file to appear (then deletes it), so an external
        // orchestrator -- a script, or an agent driving the UI -- can advance the scenario window by window.
        private static readonly string StepSignalPath =
            Path.Combine(Path.GetTempPath(), "AntidoteVerifierHarness", "step.signal");

        [STAThread]
        private static int Main(string[] args)
        {
            var auto = Array.Exists(args, a =>
                a.Equals("--auto", StringComparison.OrdinalIgnoreCase) ||
                a.Equals("-a", StringComparison.OrdinalIgnoreCase));
            var step = Array.Exists(args, a => a.Equals("--step", StringComparison.OrdinalIgnoreCase));

            if (step && File.Exists(StepSignalPath))
                File.Delete(StepSignalPath);

            ConfigureLogging();

            Console.WriteLine("=== Antidote Connectix live harness — tab-switch traffic capture (no Studio required) ===");
            Console.WriteLine("Ensure Antidote / AgentConnectix is installed (with the French module).");
            Console.WriteLine("The run is split into labelled capture windows; follow each instruction, then continue.");
            Console.WriteLine("Raw frame capture file: " + _captureFilePath);
            Console.WriteLine(step
                ? "Mode: STEP (windows advance when the signal file appears: " + StepSignalPath + ")."
                : auto
                    ? "Mode: AUTO (windows advance on timers — perform each action within the window)."
                    : "Mode: INTERACTIVE (press ENTER to advance).");
            Console.WriteLine();

            // A real WPF Application so ConnectixSession.Launch resolves Application.Current.Dispatcher and
            // marshals editor access exactly as it does inside Studio. The scenario runs on a WORKER thread
            // so the dispatcher thread stays free to PUMP messages -- otherwise the session's background
            // connect thread would block forever on dispatcher.Invoke while we wait in Pause.
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            var exitCode = 0;
            app.Startup += (_, __) =>
            {
                // A real, visible host window with a CONSTANT title, so launches originate from a
                // windowed foreground app exactly like Studio (whose main window title also does not
                // change per document). Antidote integrates with the calling application's window;
                // a windowless client is NOT a faithful stand-in for Studio.
                _hostWindow = new Window
                {
                    Title = "Antidote LiveHarness Host",
                    Width = 460,
                    Height = 160,
                    Content = new System.Windows.Controls.TextBlock
                    {
                        Text = "Antidote Connectix live harness is running.\r\nThis window stands in for Studio's main window.",
                        Margin = new Thickness(16)
                    }
                };
                _hostWindow.Show();

                var worker = new Thread(() =>
                {
                    try { exitCode = RunScenario(auto, step); }
                    finally { app.Dispatcher.Invoke(app.Shutdown); }
                })
                { IsBackground = true };
                worker.Start();
            };
            app.Run();
            return exitCode;
        }

        private static int RunScenario(bool auto, bool step)
        {
            var recorder = new WireRecorder();

            // HARNESS_DOC_A / HARNESS_DOC_B override the documentPath identities WITHOUT touching the
            // files, so the harness can impersonate another client's documents (e.g. Studio's real
            // project files) and bisect path/cache-dependent behavior in Antidote.
            var documentsFolder = Path.Combine(Path.GetTempPath(), "AntidoteVerifierHarness");
            var docA = HarnessDocument.Resolve(documentsFolder, "document-A.sdlxliff",
                SegmentsOverride("HARNESS_SEGMENTS_A") ?? SegmentsA, "HARNESS_DOC_A");
            var docB = HarnessDocument.Resolve(documentsFolder, "document-B.sdlxliff",
                SegmentsOverride("HARNESS_SEGMENTS_B") ?? SegmentsB, "HARNESS_DOC_B");
            Console.WriteLine("Documents on disk (stable paths, like a Studio project):");
            Console.WriteLine("  A: " + docA.Path + "  (" + docA.Segments.Count + " segments)");
            Console.WriteLine("  B: " + docB.Path + "  (" + docB.Segments.Count + " segments)");

            try
            {
                Phase(recorder, "LAUNCH-A",
                    "Launching the Corrector for DOCUMENT A through the real ConnectixSession...");
                // A fresh editor per launch, exactly like AntidoteLauncher.Launch in Studio.
                ActivateHostWindow();
                ConnectixSession.Launch(new StudioLikeEditorService(docA), AntidoteTool.Corrector);
                Pause(auto, step, "Wait for the Corrector tab for DOCUMENT A to appear, then continue.", 10);

                Phase(recorder, "LAUNCH-B",
                    "Launching the Corrector for DOCUMENT B on the SAME persistent connection...");
                ActivateHostWindow();
                ConnectixSession.Launch(new StudioLikeEditorService(docB), AntidoteTool.Corrector);
                Pause(auto, step, "CHECK: a SECOND tab (document-B) opens while document-A STAYS open.", 10);

                Phase(recorder, "SWITCH-TO-A",
                    "IN ANTIDOTE: click the DOCUMENT-A tab now. Do NOT click any detection yet.",
                    "This window captures what (if anything) Antidote sends on a pure tab focus switch.");
                Pause(auto, step, "After clicking the document-A tab (and nothing else), continue.", 20);

                Phase(recorder, "INTERACT-A",
                    "IN ANTIDOTE: with the DOCUMENT-A tab focused, click a detection and APPLY its correction.",
                    "This window shows which document's editor actually receives select/canReplace/replace.");
                Pause(auto, step, "After applying one correction in the document-A tab, continue.", 25);

                Phase(recorder, "SWITCH-TO-B-AND-INTERACT",
                    "IN ANTIDOTE: click the DOCUMENT-B tab, then click a detection and APPLY its correction.",
                    "Document B was the last launch, so this should route correctly — the control case.");
                Pause(auto, step, "After applying one correction in the document-B tab, continue.", 25);

                Phase(recorder, "RELAUNCH-A",
                    "Relaunching the Corrector for DOCUMENT A (fresh editor, same connection)...");
                ActivateHostWindow();
                ConnectixSession.Launch(new StudioLikeEditorService(docA), AntidoteTool.Corrector);
                Pause(auto, step, "CHECK: Antidote RE-FOCUSES the document-A tab (no new tab). Then APPLY one " +
                            "more correction there — after the relaunch it should route to document A.", 25);
            }
            finally
            {
                recorder.SetWindow("END");
            }

            recorder.PrintSummary(Console.Out);
            recorder.Dispose();
            Console.WriteLine();
            Console.WriteLine("Raw frame capture written to: " + _captureFilePath);
            LogManager.Shutdown();

            if (recorder.HasLaunchErrors)
            {
                Console.WriteLine();
                Console.WriteLine("RESULT: FAIL — the plugin logged errors during a LAUNCH window; the launch " +
                                  "path itself regressed (independent of the tab-switch findings above).");
                return 1;
            }

            Console.WriteLine();
            Console.WriteLine("RESULT: capture complete. Errors inside SWITCH/INTERACT windows (if any) are " +
                              "FINDINGS — evidence of misrouted callbacks — not harness failures.");
            return 0;
        }

        private static void Phase(WireRecorder recorder, string window, params string[] lines)
        {
            recorder.SetWindow(window);
            Console.WriteLine();
            Console.WriteLine(">>> [" + window + "]");
            foreach (var line in lines)
                Console.WriteLine("    " + line);
            // Also stamp the window into the NLog stream so the capture file interleaves markers with frames.
            PhaseLogger.Info("================ WINDOW {0} ================", window);
        }

        private static void Pause(bool auto, bool step, string message, int autoDelaySeconds)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            if (step)
            {
                Console.WriteLine("(STEP mode: waiting for " + StepSignalPath + " ...)");
                while (!File.Exists(StepSignalPath))
                    Thread.Sleep(250);
                File.Delete(StepSignalPath);
            }
            else if (auto)
            {
                Console.WriteLine("(AUTO mode: continuing in " + autoDelaySeconds + "s...)");
                Thread.Sleep(TimeSpan.FromSeconds(autoDelaySeconds));
            }
            else
            {
                Console.WriteLine("Press ENTER to continue.");
                Console.ReadLine();
            }
        }

        private static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var layout = "${time} ${level:uppercase=true:padding=-5} ${logger:shortName=true} ${message}" +
                         "${onexception:inner=${newline}${exception:format=tostring}}";

            var console = new ColoredConsoleTarget("console") { Layout = layout };
            config.AddTarget(console);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, console);

            // Verbatim capture of everything (all frames pass through the transport's Debug log) for
            // attaching to the Druide escalation.
            _captureFilePath = Path.Combine(Path.GetTempPath(), "AntidoteVerifierHarness",
                "capture-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log");
            var file = new FileTarget("capture")
            {
                FileName = _captureFilePath,
                Layout = "${longdate} ${level:uppercase=true:padding=-5} ${logger} ${message}" +
                         "${onexception:inner=${newline}${exception:format=tostring}}"
            };
            config.AddTarget(file);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, file);

            LogManager.Configuration = config;
        }

        /// <summary>
        /// Attributes every raw frame, editor call, and plugin error to the capture window that was active
        /// when it happened, by tapping the NLog stream the production code already writes to (the
        /// transport logs every frame at Debug; ConnectixAgent logs callback failures at Error). Prints a
        /// per-window summary at the end so misrouted callbacks are visible without reading the raw log.
        /// </summary>
        private sealed class WireRecorder : IDisposable
        {
            // Matches the message type inside a frame's escaped "data" payload, e.g. ...,\"message\":\"select\",...
            // Anchored on the preceding '{' or ',' so it does not match the tail of \"idMessage\".
            private static readonly Regex InnerMessageType = new Regex(
                @"[{,]\\?""message\\?""\s*:\s*\\?""([A-Za-z]+)", RegexOptions.Compiled);

            private readonly object _lock = new object();
            private readonly List<string> _windowOrder = new List<string>();
            private readonly List<Entry> _entries = new List<Entry>();
            private readonly MethodCallTarget _target;
            private volatile string _window = "PRE-START";

            public WireRecorder()
            {
                _target = new MethodCallTarget("harness-wire-recorder", (evt, _) => Record(evt));
                var config = LogManager.Configuration;
                config.AddTarget(_target);
                // Raw frames (both directions) from the production transport.
                config.AddRule(LogLevel.Debug, LogLevel.Debug, _target,
                    "Sdl.Community.AntidoteVerifier.Connectix.ClientWebSocketTransport");
                // Editor-call attribution from the harness editor service.
                config.AddRule(LogLevel.Info, LogLevel.Info, _target, "LiveHarness.Editor");
                // Callback/launch failures from the plugin (misrouted zone -> KeyNotFoundException lands here).
                config.AddRule(LogLevel.Error, LogLevel.Fatal, _target, "Sdl.Community.AntidoteVerifier.Connectix.*");
                LogManager.Configuration = config;
            }

            public bool HasLaunchErrors
            {
                get
                {
                    lock (_lock)
                    {
                        return _entries.Exists(e => e.Kind == EntryKind.Error &&
                                                    (e.Window.StartsWith("LAUNCH") || e.Window.StartsWith("RELAUNCH")));
                    }
                }
            }

            public void SetWindow(string window)
            {
                lock (_lock)
                {
                    _window = window;
                    if (!_windowOrder.Contains(window))
                        _windowOrder.Add(window);
                }
            }

            private void Record(LogEventInfo evt)
            {
                var message = evt.FormattedMessage ?? string.Empty;
                Entry entry;
                if (evt.Level >= LogLevel.Error)
                {
                    var detail = message;
                    if (evt.Exception != null)
                        detail += " [" + evt.Exception.GetType().Name + ": " + evt.Exception.Message + "]";
                    entry = new Entry(_window, EntryKind.Error, detail);
                }
                else if (message.StartsWith("<- AgentConnectix"))
                {
                    entry = new Entry(_window, EntryKind.Inbound, ExtractMessageType(message) ?? "(continuation frame)");
                }
                else if (message.StartsWith("-> AgentConnectix"))
                {
                    entry = new Entry(_window, EntryKind.Outbound, ExtractMessageType(message) ?? "(response)");
                }
                else
                {
                    entry = new Entry(_window, EntryKind.Editor, message);
                }

                lock (_lock)
                {
                    _entries.Add(entry);
                }
            }

            private static string ExtractMessageType(string frame)
            {
                var match = InnerMessageType.Match(frame);
                return match.Success ? match.Groups[1].Value : null;
            }

            public void PrintSummary(TextWriter writer)
            {
                lock (_lock)
                {
                    writer.WriteLine();
                    writer.WriteLine("==================== CAPTURE SUMMARY ====================");
                    foreach (var window in _windowOrder)
                    {
                        writer.WriteLine();
                        writer.WriteLine("--- " + window + " ---");

                        var inbound = new Dictionary<string, int>();
                        var editorCalls = new List<string>();
                        var errors = new List<string>();
                        foreach (var entry in _entries)
                        {
                            if (entry.Window != window) continue;
                            switch (entry.Kind)
                            {
                                case EntryKind.Inbound:
                                    inbound.TryGetValue(entry.Detail, out var count);
                                    inbound[entry.Detail] = count + 1;
                                    break;
                                case EntryKind.Editor:
                                    editorCalls.Add(entry.Detail);
                                    break;
                                case EntryKind.Error:
                                    errors.Add(entry.Detail);
                                    break;
                            }
                        }

                        if (inbound.Count == 0)
                        {
                            writer.WriteLine("  inbound : (none)");
                        }
                        else
                        {
                            var parts = new List<string>();
                            foreach (var kvp in inbound)
                                parts.Add(kvp.Key + " x" + kvp.Value);
                            writer.WriteLine("  inbound : " + string.Join(", ", parts));
                        }

                        foreach (var call in editorCalls)
                            writer.WriteLine("  editor  : " + call);
                        foreach (var error in errors)
                            writer.WriteLine("  ERROR   : " + error);
                    }
                    writer.WriteLine();
                    writer.WriteLine("==========================================================");
                }
            }

            public void Dispose()
            {
                var config = LogManager.Configuration;
                config?.RemoveTarget("harness-wire-recorder");
                if (config != null)
                    LogManager.Configuration = config;
            }

            private enum EntryKind { Inbound, Outbound, Editor, Error }

            private sealed class Entry
            {
                public Entry(string window, EntryKind kind, string detail)
                {
                    Window = window;
                    Kind = kind;
                    Detail = detail;
                }

                public string Window { get; }
                public EntryKind Kind { get; }
                public string Detail { get; }
            }
        }

        /// <summary>
        /// One harness document: a REAL file on disk (Antidote receives Studio's sdlxliff LocalFilePath,
        /// which exists) plus its segment texts. The segment list is shared by every editor instance bound
        /// to this document, the way successive EditorService instances in Studio see the same underlying
        /// document state.
        /// </summary>
        private sealed class HarnessDocument
        {
            private HarnessDocument(string path, List<string> segments)
            {
                Path = path;
                Segments = segments;
            }

            public string Path { get; }
            public List<string> Segments { get; }
            public string Tag => System.IO.Path.GetFileName(Path);

            public static HarnessDocument Create(string folder, string fileName, IEnumerable<string> segments)
            {
                Directory.CreateDirectory(folder);
                var path = System.IO.Path.Combine(folder, fileName);
                var list = new List<string>(segments);
                File.WriteAllText(path, string.Join(Environment.NewLine, list));
                return new HarnessDocument(path, list);
            }

            // Uses the path from the given environment variable as the document identity WITHOUT
            // creating or overwriting anything on disk (the override may point at real project files);
            // falls back to the standard harness document when the variable is not set.
            public static HarnessDocument Resolve(string folder, string fileName, IEnumerable<string> segments,
                string environmentVariable)
            {
                var overridePath = Environment.GetEnvironmentVariable(environmentVariable);
                if (!string.IsNullOrEmpty(overridePath))
                    return new HarnessDocument(overridePath, new List<string>(segments));
                return Create(folder, fileName, segments);
            }
        }

        /// <summary>
        /// In-memory <see cref="IEditorService"/> mirroring the OBSERVABLE behavior of the plugin's real
        /// EditorService: bound to one document for its lifetime (Studio news one up per launch), and
        /// throwing <see cref="KeyNotFoundException"/> for a segment index the document does not contain
        /// (the real service indexes _segmentMetadata[index]). Every routing-relevant call is logged with
        /// the document tag so the capture summary shows WHICH document received it.
        /// </summary>
        private sealed class StudioLikeEditorService : IEditorService
        {
            private static readonly Logger EditorLogger = LogManager.GetLogger("LiveHarness.Editor");

            private readonly HarnessDocument _document;

            public StudioLikeEditorService(HarnessDocument document) => _document = document;

            public int GetDocumentNoOfSegments() => _document.Segments.Count;

            // The real EditorService resolves the active segment and falls back to 1; segment 1 is a
            // faithful stand-in for a freshly opened document.
            public int GetActiveSegmentId() => 1;

            public string GetSegmentText(int index)
            {
                EnsureKnownSegment(index);
                return _document.Segments[index - 1];
            }

            public IReadOnlyList<string> GetSegmentTexts() => _document.Segments.ToArray();

            public bool CanReplace(int index, int startPosition, int endPosition, string origString,
                string displayLanguage, ref string message, ref string explication)
            {
                EnsureKnownSegment(index);
                EditorLogger.Info("[{0}] CanReplace(segment={1}, {2}..{3}, expected=\"{4}\")",
                    _document.Tag, index, startPosition, endPosition, origString);
                return true;
            }

            public string GetDocumentName() => _document.Tag;

            public string GetDocumentPath() => _document.Path;

            public void ReplaceTextInSegment(int index, int startPosition, int endPosition, string segmentText)
            {
                EnsureKnownSegment(index);
                var current = _document.Segments[index - 1];
                _document.Segments[index - 1] =
                    current.Substring(0, startPosition) + segmentText + current.Substring(endPosition);
                EditorLogger.Info("[{0}] REPLACE segment={1} [{2}..{3}] -> \"{4}\" (segment now: \"{5}\")",
                    _document.Tag, index, startPosition, endPosition, segmentText, _document.Segments[index - 1]);
            }

            public void SelectText(int index, int startPosition, int endPosition)
            {
                EnsureKnownSegment(index);
                EditorLogger.Info("[{0}] SELECT segment={1} {2}..{3}", _document.Tag, index, startPosition, endPosition);
            }

            public void ActivateDocument()
                => EditorLogger.Info("[{0}] ActivateDocument()", _document.Tag);

            private void EnsureKnownSegment(int index)
            {
                if (index < 1 || index > _document.Segments.Count)
                    throw new KeyNotFoundException(
                        "Segment index " + index + " does not exist in " + _document.Tag +
                        " (mirrors EditorService._segmentMetadata[index] for a misrouted zone id).");
            }
        }
    }
}
