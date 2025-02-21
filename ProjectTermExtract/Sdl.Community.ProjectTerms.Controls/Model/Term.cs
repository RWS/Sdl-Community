namespace Sdl.Community.ProjectTerms.Controls.Interfaces
{
    public class Term : ITerm
    {
        public string Text { get; set; }
        public int Occurrences { get; set; }

        public Term() { }

        public Term(string text, int occurences)
        {
            Text = text;
            Occurrences = occurences;
        }
    }
}
