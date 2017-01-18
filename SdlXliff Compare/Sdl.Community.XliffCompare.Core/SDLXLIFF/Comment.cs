using System;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class Comment
    {
        internal string Author { get; set; }

        internal DateTime Date { get; set; }

        internal bool DateSpecified { get; set; }

        internal string Severity { get; set; }

        internal string Text { get; set; }

        internal string Version { get; set; }

        internal Comment()
        {
            Author = "";
            Date = DateTime.Now;
            DateSpecified = false;
            Severity = "Low";
            Text = "";
            Version = "";
        }
    }
}
