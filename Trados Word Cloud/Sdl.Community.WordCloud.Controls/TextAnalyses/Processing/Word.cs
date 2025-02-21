using System.Collections.Generic;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Processing
{
    public class Word : IWord
    {
        public string Text { get; set; }
        public int Occurrences { get; set; }

        public Word()
        {

        }

        public Word(KeyValuePair<string, int> textOccurrencesPair)
            : this(textOccurrencesPair.Key, textOccurrencesPair.Value)
        {
        }

        public Word(string text, int occurrences)
        {
            Text = text;
            Occurrences = occurrences;
        }

        public int CompareTo(IWord other)
        {
            return this.Occurrences - other.Occurrences;
        }

        public string GetCaption()
        {
            return string.Format("{0} - occurrences", Occurrences);
        }
    }
}