using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Utils;

namespace TMXTests
{
	public class TestGeneral
	{
		[Fact]
		public void TestCompareLanguages()
		{
			Assert.True(CompareLanguages.Equivalent("en", "en"));
			Assert.True(CompareLanguages.Equivalent("en", "EN"));
			Assert.True(CompareLanguages.Equivalent("en", "EN-us"));
			Assert.True(CompareLanguages.Equivalent("En", "en-us"));
			Assert.True(CompareLanguages.Equivalent("en", "en-Gb"));
			Assert.True(CompareLanguages.Equivalent("EN", "en-Gb"));
			Assert.True(CompareLanguages.Equivalent("EN-gb", "en-Gb"));

			Assert.True(!CompareLanguages.Equivalent("EN-", "en-Gb"));
			Assert.True(!CompareLanguages.Equivalent("en-us", "en-Gb"));
			LanguageArray languages = new LanguageArray
			{
				Languages = new[] { "en", "FR", "gr-GR" }
			};
			Assert.True(languages.TryGetEquivalentLanguage("EN-us") == "en");
			Assert.True(languages.TryGetEquivalentLanguage("EN-gb") == "en");
			Assert.True(languages.TryGetEquivalentLanguage("fr-fr") == "FR");
			Assert.True(languages.TryGetEquivalentLanguage("fr") == "FR");
			Assert.True(languages.TryGetEquivalentLanguage("fr-ff") == "FR");
			Assert.True(languages.TryGetEquivalentLanguage("gr-gr") == "gr-GR");
			Assert.True(languages.TryGetEquivalentLanguage("gr") == "gr-GR");
			Assert.True(languages.TryGetEquivalentLanguage("GR") == "gr-GR");
			Assert.True(languages.TryGetEquivalentLanguage("GR-gg") == null);
			Assert.True(languages.TryGetEquivalentLanguage("de") == null);
			Assert.True(languages.TryGetEquivalentLanguage("de-DE") == null);
			Assert.True(languages.TryGetEquivalentLanguage("it") == null);
			Assert.True(languages.TryGetEquivalentLanguage("ro") == null);
		}
	}
}
