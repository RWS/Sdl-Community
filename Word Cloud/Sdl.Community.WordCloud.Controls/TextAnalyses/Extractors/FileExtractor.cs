using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors
{
    public class FileExtractor : BaseExtractor
    {
        private readonly FileInfo m_FileInfo;

        public FileExtractor(FileInfo fileInfo , IProgressIndicator progressIndicator)
            : base(progressIndicator)
        {
            m_FileInfo = fileInfo;
        }

        public override IEnumerable<string> GetWords()
        {
            using (StreamReader reader = m_FileInfo.OpenText())
            {
                foreach (string word in GetWords(reader))
                {
                    yield return word;
                }
            }
        }

        protected IEnumerable<string> GetWords(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                IEnumerable<string> wordsInLine = GetWords(line);
                foreach (string word in wordsInLine)
                {
                    yield return word;
                }
                OnLinePorcessed(line);
            }
        }

        protected override void OnLinePorcessed(string line)
        {
            ProgressIndicator.Increment(1);
        }
    }
}
