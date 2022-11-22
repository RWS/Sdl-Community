using System.Globalization;
using TMX_Lib.TmxFormat;

namespace TMXTests
{
	public class TestParser
	{
		[Fact]
		public void Test1()
		{
			var parser = new TmxParser("..\\..\\..\\Samples\\#4.tmx");
			Assert.Equal(false, parser.HasError);
			var source = new CultureInfo(parser.Header.SourceLanguage);
			var target = new CultureInfo(parser.Header.TargetLanguage);
			Assert.Equal("INTRODUCTION", parser.TranslationUnits[0].Text(source).OriginalText);
			Assert.Equal("INTRODUCCIÓN", parser.TranslationUnits[0].Text(target).OriginalText);

			Assert.Equal("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.", parser.TranslationUnits[1].Text(source).OriginalText);
			Assert.Equal("Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.", parser.TranslationUnits[1].Text(target).OriginalText);
		}
	}
}