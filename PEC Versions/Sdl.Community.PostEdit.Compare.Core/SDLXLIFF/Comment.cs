using System;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class Comment
    {
        public string Author { get; set; }

        public DateTime Date { get; set; }

        public bool DateSpecified { get; set; }

        public string Severity { get; set; }

        public string Text { get; set; }

        public string Version { get; set; }

        public Comment()
        {
            Author = string.Empty;
            Date = DateTime.Now;
            DateSpecified = false;
            Severity = "Low";
            Text = string.Empty;
            Version = string.Empty;
        }
    }
}
