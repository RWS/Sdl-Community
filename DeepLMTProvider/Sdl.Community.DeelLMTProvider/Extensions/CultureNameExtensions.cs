using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
	public static class CultureNameExtensions
	{
		public static string GetSourceLanguageCode(this LanguagePair languagePair) => languagePair.SourceCultureName.Split('-')[0];
		public static string GetTargetLanguageCode(this LanguagePair languagePair) => languagePair.TargetCultureName.Split('-')[0];
	}
}