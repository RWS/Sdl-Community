using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Documents.Records
{
    [Serializable]
    public class KeySpeedGroup
    {
        public DateTime? Start { get; set; }
        public DateTime? Stop { get; set; }
        public TimeSpan? Elapsed { get; set; }
        public double CharCount { get; set; }
        public double CharsPerSecond { get; set; }
        public double CharsPerMinute { get; set; }
        public double WordsPerMinute { get; set; }
        public List<KeyStroke> KeyStrokes { get; set; }
        public KeySpeedGroup()
        {
            Start = null;
            Stop = null;
            Elapsed = null;
            CharCount = 0;
            CharsPerSecond = 0;
            CharsPerMinute = 0;
            WordsPerMinute = 0;
            KeyStrokes = new List<KeyStroke>();
        }
    }
}
