using System;
using System.Collections.Generic;

namespace TMX_Lib.TmxFormat
{
	public class TmxHeader
    {
        public readonly string SourceLanguage ;
        public readonly string TargetLanguage ;
        public readonly List<string> Domains ;
        public readonly DateTime? CreationDate;
        public readonly string Author;
        public readonly string Xml;

        public TmxHeader(string sourceLanguage, string targetLanguage, List<string> domains, DateTime? creationDate, string author, string xml)
        {
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;
            Domains = domains;
            CreationDate = creationDate;
            Author = author;
            Xml = xml;
        }
    }
}
