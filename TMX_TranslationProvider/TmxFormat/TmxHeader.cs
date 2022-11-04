using System;
using System.Collections.Generic;

namespace TMX_TranslationProvider.TmxFormat
{
	public class TmxHeader
    {
        public readonly string SourceLanguage ;
        public readonly string TargetLanguage ;
        public readonly List<string> Domains ;
        public readonly DateTime? CreationDate;
        public readonly string Author;

        public TmxHeader(string sourceLanguage, string targetLanguage, List<string> domains, DateTime? creationDate, string author)
        {
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;
            Domains = domains;
            CreationDate = creationDate;
            Author = author;
        }
    }
}
