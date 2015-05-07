using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.Jobs.Model
{
    public class LanguagePair
    {
        public LanguagePair()
        {
            
        }
        public LanguagePair(Language source, Language target)
        {
            Source = source;
            Target = target;
        }

        public Language Source { get; set; }

        public Language Target { get; set; }

        public static LanguagePair CreateLanguagePair(string languagePair, List<Language> languages)
        {
            if (!languagePair.Contains("_"))
                throw new ArgumentException("language pair string must contain underline");
            var langStrings = languagePair.Split('_');
            if (langStrings.Length != 2) 
                throw new ArgumentException("language pair string format is invalid");

            return new LanguagePair
            {
                Source = languages.FirstOrDefault(x => x.LanguageCode == langStrings[0]),
                Target = languages.FirstOrDefault(x => x.LanguageCode == langStrings[1])
            };
        }

        public string Serialize()
        {
            return string.Format("{0}_{1}", Source.LanguageCode, Target.LanguageCode);
        }
    }
}
