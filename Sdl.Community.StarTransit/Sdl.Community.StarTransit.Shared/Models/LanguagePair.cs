using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class LanguagePair
    {
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public List<StarTranslationMemoryMetadata> StarTranslationMemoryMetadatas { get; set; }
        public bool HasTm { get; set; }
        public List<string> SourceFile { get; set; }
        public List<string> TargetFile { get; set; }
        public bool CreateNewTm { get; set; }
        public bool ChoseExistingTm { get; set; }
        public string TmPath { get; set; }
        public string TmName { get; set; }
        public string PairNameIso { get; set; }
        public string PairName { get; set; }
    }
}
