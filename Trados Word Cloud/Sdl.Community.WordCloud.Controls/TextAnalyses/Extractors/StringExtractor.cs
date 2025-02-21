using System.Collections.Generic;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors
{
    public class StringExtractor : BaseExtractor
    {
        private readonly string m_Text;

        public StringExtractor(string text, IProgressIndicator progressIndicator)
            : base(progressIndicator)
        {
            m_Text = text;
            ProgressIndicator = progressIndicator;
            ProgressIndicator.Maximum = m_Text.Length;
        }

        public override IEnumerable<string> GetWords()
        {
            return GetWords(m_Text);
        }

        protected override void OnCharPorcessed(char ch)
        {
            ProgressIndicator.Increment(1);
        }
    }
}
