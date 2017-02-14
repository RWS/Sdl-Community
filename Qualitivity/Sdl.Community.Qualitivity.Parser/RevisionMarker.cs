using System;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class RevisionMarker
    {
        public enum RevisionType
        {
            Insert,
            Delete,
            Unchanged
        }
        public string Id { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public RevisionType RevType { get; set; }
        public RevisionMarker(string id, string author, DateTime date, RevisionType revType)
        {
            Id =id;
            Author = author;
            Date = date;
            RevType = revType;
        }
    }
}
