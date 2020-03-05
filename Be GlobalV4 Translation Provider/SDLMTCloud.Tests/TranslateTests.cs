using System.Globalization;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Xunit;
using Segment = Sdl.LanguagePlatform.Core.Segment;

namespace SDLMTCloud.Tests
{
	public class TranslateTests
    {
		private static BeGlobalTranslationOptions _options = new BeGlobalTranslationOptions
		{
			AuthenticationMethod = "ClientLogin",
			ClientId = "QuUhDMikefBAOrpVT194f9wpHy5xFfAH",
			ClientSecret = "hnazS4WpwiPWVhvlHRUgSy_134XbM8kVWul8LUOs8nSot-1Iw-FnDX9yp48oWKU9"			
		};
		private static BeGlobalTranslationProvider _provider = new BeGlobalTranslationProvider(_options);
		private readonly LanguagePair chiEng = new LanguagePair(
			CultureInfo.GetCultureInfo("chi"),
			CultureInfo.GetCultureInfo("en-us"));

		[Theory]
		[InlineData("+0.15円/本")]
		public void GetTranslation(string text)
		{
			////if (Application.Current == null)
			////{
			////	new Application();
			////}

			//var segment = new Segment();
			//segment.Add(text);
			//var segments = new Segment[] { segment };
			//var languageDirection = new BeGlobalLanguageDirection(_provider, chiEng);
			
			//var results = languageDirection.TranslateSegments(segments);
		
			//var tu = new TranslationUnit
			//{
			//	SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
			//	TargetSegment = results == null ? new Segment() : results[0]
			//};
			//var searchResult = new SearchResult(tu);

			//Assert.Equal(text, searchResult?.MemoryTranslationUnit?.TargetSegment?.ToString());
		}
	}
}