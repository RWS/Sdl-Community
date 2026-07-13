using System;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Transport seam over the Connectix WebSocket channel. Production uses a real WebSocket;
    /// tests use a fake that raises <see cref="FrameReceived"/> on demand and records sends.
    /// Raw payloads are single transport frames (see <c>Frame</c>).
    /// </summary>
    public interface IConnectixTransport
    {
        event Action<string> FrameReceived;

        /// <summary>Raised once when the channel is gone (server closed it or the receive loop ended).</summary>
        event Action Closed;

        bool IsOpen { get; }

        void Open();

        void Send(string frameJson);

        void Close();
    }
}
