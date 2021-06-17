using Sdl.Community.MtEnhancedProvider.Service;
using Xunit;

namespace Sdl.Community.MtEnhancedProvider.Tests
{
	public class MtTranslationProviderTagPlacerTests
	{
		private readonly MtTranslationProviderTagPlacer _tagPlacer;
		/// <summary>
		/// In the plugin tags are marked with "```" symbol. There are 3 types of tags starting tag <tg1>, closing tag </tg1>, standalone/placeholder/lockedcontent tag <tg1/>
		/// </summary>
		public MtTranslationProviderTagPlacerTests()
		{
			var htmlUtil = new HtmlUtil();
			_tagPlacer = new MtTranslationProviderTagPlacer(htmlUtil);
		}

		[Theory]
		[InlineData(
			"Я использую MT Enhanced Plugin, чтобы <tg317fcf33-f73f-4acf-82f9-6a9710976ad8/> включить Microsoft MT.",
			"Я использую MT Enhanced Plugin, чтобы ```<tg317fcf33-f73f-4acf-82f9-6a9710976ad8/>``` включить Microsoft MT.")]
		[InlineData(@"Я использую MT Enhanced Plugin,<tg317fcf33-f73f-4acf-82f9-6a9710976ad8> чтобы <tg317fcf33-f73f-4acf-82f9-6a9710976ad8/> включить Microsoft MT.",
			"Я использую MT Enhanced Plugin,```<tg317fcf33-f73f-4acf-82f9-6a9710976ad8>``` чтобы ```<tg317fcf33-f73f-4acf-82f9-6a9710976ad8/>``` включить Microsoft MT.")]
		[InlineData(
			"Моя языковая пара - Ru -> Es (международный испанский), и русский <tg919d508d-4d2d-4b55-955f-42b00c927e71/> источник содержит английские фразы, которые мне не нужно переводить.",
			"Моя языковая пара - Ru -> Es (международный испанский), и русский ```<tg919d508d-4d2d-4b55-955f-42b00c927e71/>``` источник содержит английские фразы, которые мне не нужно переводить.")]
		public void MarkTags_GuidId(string translatedText, string expectedText)
		{
			var markedTags = _tagPlacer.MarkTags(translatedText,_tagPlacer.GuidTagIdRegex);

			Assert.Equal(expectedText, markedTags);
		}

		[Theory]
		[InlineData(
			"You can also use it for colours like <tg11>red</tg11>, or <tg14>orange</tg14>, <tg17>yellow</tg17>, <tg20>green</tg20>, <tg23>blue</tg23>, <tg26>indigo</tg26> and <tg29>violet</tg29>. you",
			"You can also use it for colours like ```<tg11>```red```</tg11>```, or ```<tg14>```orange```</tg14>```, ```<tg17>```yellow```</tg17>```, ```<tg20>```green```</tg20>```, ```<tg23>```blue```</tg23>```, ```<tg26>```indigo```</tg26>``` and ```<tg29>```violet```</tg29>```. you")]
		[InlineData(
			"You can use QuickPlace to insert formatting, like <tg32>bold</tg32> and <tg35>italic</tg35> text, or even groups of formatting like this <tg38>bold/italic/underlined</tg38>.",
			"You can use QuickPlace to insert formatting, like ```<tg32>```bold```</tg32>``` and ```<tg35>```italic```</tg35>``` text, or even groups of formatting like this ```<tg38>```bold/italic/underlined```</tg38>```.")]
		[InlineData(
			"You can also use the same shortcut to handle other tags like these, <tg61/>, <tg63/> known as placeholder tags.",
			"You can also use the same shortcut to handle other tags like these, ```<tg61/>```, ```<tg63/>``` known as placeholder tags.")]
		public void MarkTags_SimpleId(string translatedText, string expectedText)
		{
			var markedTags = _tagPlacer.MarkTags(translatedText,_tagPlacer.SimpleTagRegex);

			Assert.Equal(expectedText, markedTags);
		}
	}
}
