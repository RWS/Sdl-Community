using System;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class RevisionMarker
    {

        public enum RevisionType
        {
            Insert,
            Delete,
            Unchanged
        }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public RevisionType Type { get; set; }
        public RevisionMarker(string author, DateTime date, RevisionType type)
        {
            Author = author;
            Date = date;
            Type = type;
        }
    }
}
