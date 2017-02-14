using System;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class Comment: ICloneable
    {
        public string Author { get; set; }
        public DateTime? Date { get; set; }        
        public string Severity { get; set; }
        public string Text { get; set; }
        public string Version { get; set; }

        public Comment()
        {
            Author = string.Empty;
            Date = null;            
            Severity = "Low";
            Text = string.Empty;
            Version = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
