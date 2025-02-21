namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Stemmers.En
{
    public class PorterStemmer : IWordStemmer
    {
        public string GetStem(string word)
        {
            return new PorterStem(word).ToString();
        }
    }
}
