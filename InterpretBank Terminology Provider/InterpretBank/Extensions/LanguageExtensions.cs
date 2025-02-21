using Sdl.Core.Globalization;

namespace InterpretBank.Extensions
{
    public static class LanguageExtensions
    {
        public static string GetInterpretBankLanguageName(this Language language) => language?.DisplayName.Split(' ')[0];
    }
}