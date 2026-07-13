using System.Text;

namespace Sdl.Community.AntidoteVerifier.Connectix.Protocol
{
    /// <summary>
    /// Reassembles multi-frame messages. Frames of one message arrive consecutively, numbered
    /// 0..totalFrame-1; once every slot is filled the concatenated payload is a complete JSON message.
    /// Mirrors the reference agent's <c>_ingest</c> buffer.
    /// </summary>
    public sealed class FrameAssembler
    {
        private string[] _buffer = new string[0];
        private int _received;

        /// <summary>
        /// Adds a frame. Returns the full message string once the final frame of the group has been
        /// received, otherwise <c>null</c>.
        /// </summary>
        public string Add(Frame frame)
        {
            if (frame == null) return null;

            if (_buffer.Length != frame.TotalFrame)
            {
                _buffer = new string[frame.TotalFrame < 0 ? 0 : frame.TotalFrame];
                _received = 0;
            }

            if (frame.IdFrame < 0 || frame.IdFrame >= _buffer.Length)
                return null;

            if (_buffer[frame.IdFrame] == null)
                _received++;
            _buffer[frame.IdFrame] = frame.Data ?? string.Empty;

            if (_received < _buffer.Length || _buffer.Length == 0)
                return null;

            var sb = new StringBuilder();
            foreach (var part in _buffer)
                sb.Append(part);

            _buffer = new string[0];
            _received = 0;
            return sb.ToString();
        }
    }
}
