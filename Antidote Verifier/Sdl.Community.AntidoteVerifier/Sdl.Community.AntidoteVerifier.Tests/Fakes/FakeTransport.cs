using System;
using System.Collections.Generic;
using Sdl.Community.AntidoteVerifier.Connectix;

namespace Sdl.Community.AntidoteVerifier.Tests.Fakes
{
    /// <summary>
    /// In-memory <see cref="IConnectixTransport"/>. Records every frame the agent sends and lets a test
    /// push raw frames in as if they came from Antidote.
    /// </summary>
    public sealed class FakeTransport : IConnectixTransport
    {
        public List<string> Sent { get; } = new List<string>();

        public bool IsOpen { get; private set; }

        public event Action<string> FrameReceived;

        public event Action Closed;

        public void Open() => IsOpen = true;

        public void Close() => IsOpen = false;

        public void Send(string frameJson) => Sent.Add(frameJson);

        public void PushFrame(string frameJson) => FrameReceived?.Invoke(frameJson);

        public void RaiseClosed() => Closed?.Invoke();
    }
}
