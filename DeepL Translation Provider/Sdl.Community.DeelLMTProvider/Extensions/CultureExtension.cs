using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
    public static class CultureExtension
    {
        public static bool Equivalent(this CultureCode cultureCode, string language)
        {
            var culture = new CultureInfo(cultureCode);
            var ietfLanguageTag = culture.IetfLanguageTag.ToLowerInvariant();
            var twoLetterIso = culture.TwoLetterISOLanguageName.ToLowerInvariant();

            return language == ietfLanguageTag || language == twoLetterIso;
        }
        
        public static string GetSourceLanguageCode(this LanguagePair languagePair) => languagePair.SourceCultureName.Split('-')[0];
        public static string GetTargetLanguageCode(this LanguagePair languagePair) => languagePair.TargetCultureName.Split('-')[0];
    }
}