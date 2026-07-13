using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Owns the plugin's Connectix correction session for the Studio process. AgentConnectix accepts only
    /// ONE live WebSocket at a time: opening a second connection while the first is still alive makes the
    /// endpoint forcibly close a socket ("An existing connection was forcibly closed by the remote host",
    /// confirmed via runtime logs). So the plugin keeps a SINGLE persistent connection and retargets its
    /// <see cref="StudioWordProcessorAgent"/> at the active document on each launch. Antidote opens one
    /// correction tab per distinct documentPath over that connection; zone-id-bearing callbacks (select /
    /// allowEdit / replace) are routed to the right document by the agent's document registry, while
    /// identifier-less callbacks (getTextZones / docIsAvailable) can only be answered for the last
    /// launched document -- a protocol gap flagged for Druide in the migration doc.
    /// Discovery and the blocking WebSocket connect run off the UI thread, while editor access is
    /// marshalled back to the UI thread because Studio editor objects are UI-thread affine.
    /// </summary>
    public sealed class ConnectixSession
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ConnectixSession Instance =
            new ConnectixSession(new ConnectixPortLocator().LocatePort);

        private readonly Func<int> _locatePort;
        private readonly object _gate = new object();

        // Every launch is queued and processed by ONE worker thread so the blocking WebSocket connect stays
        // off Studio's UI thread and launches never overlap.
        private readonly BlockingCollection<LaunchRequest> _queue = new BlockingCollection<LaunchRequest>();
        private readonly object _workerGate = new object();
        private Thread _worker;

        // How many times to retry the connection if the agent aborts the socket mid-handshake.
        private const int ConnectAttempts = 3;
        private static readonly TimeSpan RetryBackoff = TimeSpan.FromMilliseconds(500);

        // The single live connection, reused for every document. Rebuilt only if Antidote closes it.
        private Connection _connection;

        private ConnectixSession(Func<int> locatePort)
        {
            _locatePort = locatePort;
        }

        /// <summary>
        /// Launches <paramref name="tool"/> (an <see cref="AntidoteTool"/> id) for the active Studio
        /// document over the single persistent Connectix connection, retargeting it at the active document
        /// first. Editor access is marshalled onto Studio's UI thread through the application
        /// <see cref="System.Windows.Threading.Dispatcher"/>, which is stable across launches.
        /// </summary>
        public static void Launch(IEditorService editor, string tool)
        {
            // Marshal editor access to Studio's UI thread via the application dispatcher, which is stable
            // for the process lifetime. A per-launch WindowsFormsSynchronizationContext captured here went
            // stale on later launches and threw NullReferenceException inside Control.Invoke, so
            // getTextZones returned nothing and the second document's Corrector showed nothing.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            Action<Action> runOnUi = dispatcher != null
                ? action => dispatcher.Invoke(action)
                : action => action();

            Instance.LaunchInternal(editor, runOnUi, tool);
        }

        private void LaunchInternal(IEditorService editor, Action<Action> runOnUi, string tool)
        {
            Logger.Info("Antidote launch queued for tool '{0}'.", tool);

            EnsureWorker();
            _queue.Add(new LaunchRequest(editor, runOnUi, tool));
        }

        private void EnsureWorker()
        {
            if (_worker != null)
                return;

            lock (_workerGate)
            {
                if (_worker != null)
                    return;

                _worker = new Thread(ProcessQueue) { IsBackground = true, Name = "AntidoteConnectixLauncher" };
                _worker.Start();
            }
        }

        // Drains launches one at a time so the blocking connect never overlaps another launch.
        private void ProcessQueue()
        {
            foreach (var request in _queue.GetConsumingEnumerable())
            {
                try
                {
                    ConnectAndLaunch(request);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Unexpected error processing an Antidote launch request.");
                }
            }
        }

        private void ConnectAndLaunch(LaunchRequest request)
        {
            try
            {
                ConnectixAgent agent;

                lock (_gate)
                {
                    // Reuse the single live connection; only (re)connect when there is none or it dropped.
                    if (_connection == null || !_connection.Transport.IsOpen)
                    {
                        Teardown();
                        _connection = ConnectWithRetryLocked(request.Editor, request.RunOnUi);
                        Logger.Info("Opened the Antidote connection.");
                    }
                    else
                    {
                        // Point the agent at the active document before the handshake reads it (segments/
                        // selection change between launches).
                        _connection.WordProcessor.SetActiveEditor(request.Editor);
                        Logger.Info("Reusing the Antidote connection for the active document.");
                    }

                    agent = _connection.Agent;
                }

                agent.Launch(request.Tool);
            }
            catch (ConnectixNotFoundException ex)
            {
                Logger.Error(ex, "AgentConnectix could not be located.");
                ShowError(PluginResources.AntidoteRegistryKeyNotFound_Error);
                Teardown();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to start the Antidote Connectix session.");
                ShowError(PluginResources.AntidoteCannotBeStarted_Error);
                Teardown();
            }
        }

        // Opens the connection, retrying if the endpoint aborts the socket during the handshake.
        // Must be called under _gate. Returns the live connection.
        private Connection ConnectWithRetryLocked(IEditorService editor, Action<Action> runOnUi)
        {
            Exception last = null;
            for (var attempt = 1; attempt <= ConnectAttempts; attempt++)
            {
                var wordProcessor = new StudioWordProcessorAgent(editor, runOnUi);
                var port = _locatePort();
                var transport = new ClientWebSocketTransport(port);
                var connection = new Connection(wordProcessor, transport,
                    new ConnectixAgent(transport, wordProcessor));
                transport.Closed += OnTransportClosed;

                try
                {
                    connection.Agent.Connect();
                    return connection;
                }
                catch (Exception ex) when (IsTransientConnect(ex))
                {
                    last = ex;
                    Logger.Warn(ex, "Antidote connection attempt {0}/{1} was aborted; retrying.",
                        attempt, ConnectAttempts);
                    try { connection.Agent.Dispose(); } catch { /* best effort */ }

                    if (attempt < ConnectAttempts)
                        Thread.Sleep(RetryBackoff);
                }
            }

            throw new InvalidOperationException(
                $"AgentConnectix aborted the connection after {ConnectAttempts} attempts.", last);
        }

        private static bool IsTransientConnect(Exception ex)
        {
            switch (ex)
            {
                case ConnectixNotFoundException _:
                    return false;
                case WebSocketException _:
                    return true;
                default:
                    return ex.InnerException is WebSocketException;
            }
        }

        // Antidote closed the channel (its pane/the app closed): drop the connection so the next launch
        // reconnects.
        private void OnTransportClosed() => Teardown();

        private void Teardown()
        {
            lock (_gate)
            {
                var connection = _connection;
                if (connection == null)
                    return;

                _connection = null;
                try
                {
                    connection.Agent.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Error while disposing the Antidote Connectix session.");
                }
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // The single live Connectix connection: its editor-facing agent, transport, and protocol agent.
        private sealed class Connection
        {
            public Connection(StudioWordProcessorAgent wordProcessor, IConnectixTransport transport,
                ConnectixAgent agent)
            {
                WordProcessor = wordProcessor;
                Transport = transport;
                Agent = agent;
            }

            public StudioWordProcessorAgent WordProcessor { get; }
            public IConnectixTransport Transport { get; }
            public ConnectixAgent Agent { get; }
        }

        // A queued launch: everything ConnectAndLaunch needs to serve it.
        private sealed class LaunchRequest
        {
            public LaunchRequest(IEditorService editor, Action<Action> runOnUi, string tool)
            {
                Editor = editor;
                RunOnUi = runOnUi;
                Tool = tool;
            }

            public IEditorService Editor { get; }
            public Action<Action> RunOnUi { get; }
            public string Tool { get; }
        }
    }
}
