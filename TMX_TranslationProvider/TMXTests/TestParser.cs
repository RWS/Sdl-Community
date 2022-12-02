using System.Globalization;
using TMX_Lib.TmxFormat;

namespace TMXTests
{
	public class TestParser
	{
		[Fact]
		public void Test()
		{
			var parser = new TmxParser("..\\..\\..\\..\\SampleTestFiles\\#4.tmx");
			Assert.Equal(false, parser.HasError);
			var source = new CultureInfo(parser.Header.SourceLanguage);
			var target = new CultureInfo(parser.Header.TargetLanguage);
			var tus = parser.TryReadNextTUs();
			
			Assert.Equal("INTRODUCTION", tus[0].Texts[0].FormattedText);
			Assert.Equal("INTRODUCCIÓN", tus[0].Texts[1].FormattedText);

			Assert.Equal("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.",
				tus[1].Texts[0].FormattedText);
			Assert.Equal("Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.", 
				tus[1].Texts[1].FormattedText);
		}
	}
}