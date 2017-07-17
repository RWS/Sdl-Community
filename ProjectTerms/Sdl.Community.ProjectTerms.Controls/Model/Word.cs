namespace Sdl.Community.ProjectTerms.Controls.Interfaces
{
    public class Word : IWord
    {
        public string Text { get; set; }
        public int Occurrences { get; set; }

        public Word(string text, int occurences)
        {
            Text = text;
            Occurrences = occurences;
        }
    }
}
