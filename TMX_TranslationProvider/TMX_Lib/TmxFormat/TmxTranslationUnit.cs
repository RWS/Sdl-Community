using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Search.Result;
using TMX_Lib.Search.SearchSegment;
using TMX_Lib.Utils;

namespace TMX_Lib.TmxFormat
{
	public class TmxText
	{
		public string Language;
		public string Text;
		public string FormattedText;
	}
	public class TmxTranslationUnit
	{
		public DateTime? CreationTime;
		public string CreationAuthor;
		public DateTime? ChangeTime;
		public string ChangeAuthor;

		public DateTime? TranslateTime => ChangeTime ?? CreationTime;

		public string XmlProperties = "";
		public string TuAttributes = "";

		public string SourceLanguage, TargetLanguage;

		// Example: <prop type="x-Domain:SinglePicklist">Construction</prop>
		public string Domain = "";

        public ConfirmationLevel ConfirmationLevel = ConfirmationLevel.Draft;

        public List<TmxText> Texts = new List<TmxText>();

        public TmxText Text(CultureInfo language) =>
			Texts.First(t => t.Language.Equals(language.IsoLanguageName(), StringComparison.OrdinalIgnoreCase));
		
		public bool HasLanguage(CultureInfo language) => true;

		public SimpleResult ToSimpleResult(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var sr = new SimpleResult
			{
				TranslateTime = TranslateTime ?? DateTime.MinValue,
				ConfirmationLevel = ConfirmationLevel, 
				Origin = TranslationUnitOrigin.TM,
				// FIXME
				Source = new List<TextPart> { new TextPart { Text = Text(sourceLanguage).Text }},
				// FIXME
				Target = new List<TextPart> { new TextPart { Text = Text(targetLanguage).Text } },
			};
			return sr;
		}
	}
}
