using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Processing
{
    public struct WordGroup : IWord, IEnumerable<IWord>
    {
        private readonly IEnumerable<IWord> m_AssociatedWords;

        public WordGroup(string stem, IEnumerable<IWord> associatedWords)
            : this()
        {
            this.Stem = stem;
            this.m_AssociatedWords = associatedWords;
            this.Occurrences = this.m_AssociatedWords.Sum(word => word.Occurrences);
            this.Text = this.m_AssociatedWords.Max().Text;
        }

        public string Stem { get; set; }

        public string Text { get; private set; }

        public int Occurrences { get; private set; }

        public int CompareTo(IWord other)
        {
            return this.Occurrences - other.Occurrences;
        }

        public IEnumerator<IWord> GetEnumerator()
        {
            return m_AssociatedWords.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string GetCaption()
        {
            string caption =  string.Empty;
            return
                this
                .OrderByDescending(
                    word => word.Occurrences)
                .Aggregate(
                    caption, 
                    (s, word) => string.Format("{0}\r\n{1}\t{2}", s, word.Text, word.Occurrences));
        }
    }
}