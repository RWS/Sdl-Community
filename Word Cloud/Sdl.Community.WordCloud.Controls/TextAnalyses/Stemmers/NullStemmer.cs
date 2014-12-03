using System.Globalization;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Stemmers
{
    public class NullStemmer : IWordStemmer
    {
        public string GetStem(string word)
        {
            return word.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
