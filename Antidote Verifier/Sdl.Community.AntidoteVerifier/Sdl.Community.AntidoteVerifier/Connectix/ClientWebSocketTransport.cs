using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Real <see cref="IConnectixTransport"/> over a <see cref="ClientWebSocket"/> connected to the
    /// AgentConnectix server at <c>ws://localhost:PORT</c>. A background loop reassembles each WebSocket
    /// message and raises <see cref="FrameReceived"/> with the whole frame payload; sends are serialized
    /// because <see cref="ClientWebSocket"/> forbids concurrent send operations. This is an I/O boundary
    /// and is exercised by integration/manual testing rather than unit tests.
    /// </summary>
    public sealed class ClientWebSocketTransport : IConnectixTransport
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly int _port;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private ClientWebSocket _socket;
        private Task _receiveLoop;
        private int _closedRaised;

        public ClientWebSocketTransport(int port)
        {
            _port = port;
        }

        public event Action<string> FrameReceived;

        public event Action Closed;

        public bool IsOpen => _socket?.State == WebSocketState.Open;

        public void Open()
        {
            _socket = new ClientWebSocket();
            var uri = new Uri("ws://localhost:" + _port);
            _socket.ConnectAsync(uri, _cts.Token).GetAwaiter().GetResult();
            _receiveLoop = Task.Run(() => ReceiveLoopAsync(_cts.Token));
        }

        public void Send(string frameJson)
        {
            if (_socket == null) return;

            _sendLock.Wait();
            try
            {
                Logger.Debug("-> AgentConnectix: {0}", frameJson);
                var bytes = Encoding.UTF8.GetBytes(frameJson);
                _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token)
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to send frame to AgentConnectix.");
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            var buffer = new byte[8192];
            try
            {
                while (!token.IsCancellationRequested && _socket.State == WebSocketState.Open)
                {
                    var message = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), token)
                            .ConfigureAwait(false);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await CloseSocketAsync().ConfigureAwait(false);
                            return;
                        }

                        message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    while (!result.EndOfMessage);

                    var frame = message.ToString();
                    Logger.Debug("<- AgentConnectix: {0}", frame);
                    FrameReceived?.Invoke(frame);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on Close().
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AgentConnectix receive loop terminated unexpectedly.");
            }
            finally
            {
                RaiseClosed();
            }
        }

        private void RaiseClosed()
        {
            if (Interlocked.Exchange(ref _closedRaised, 1) == 0)
                Closed?.Invoke();
        }

        private async Task CloseSocketAsync()
        {
            try
            {
                if (_socket != null && (_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseReceived))
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Error while closing AgentConnectix WebSocket.");
            }
        }

        public void Close()
        {
            try
            {
                _cts.Cancel();
                CloseSocketAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Error during AgentConnectix transport shutdown.");
            }
            finally
            {
                _socket?.Dispose();
                _socket = null;
            }
        }
    }
}
