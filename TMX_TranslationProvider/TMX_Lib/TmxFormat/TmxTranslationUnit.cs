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

        public string SourceText => SourceSegment().OriginalText;
        public string TargetText => TargetSegment().OriginalText;
        public string SourceFormattedText => SourceSegment().OriginalText;
        public string TargetFormattedText => TargetSegment().OriginalText;

		public List<TmxFormattedTextPart> Source = new List<TmxFormattedTextPart>();
		public List<TmxFormattedTextPart> Target = new List<TmxFormattedTextPart>();

		private static TextSegment ToSegment(List<TmxFormattedTextPart> formattedText) =>
			new TextSegment(string.Join("", formattedText.Where(part => part.Text != "").Select(part => part.Text)));

		private TextSegment SourceSegment() => _sourceSegment ?? (_sourceSegment = ToSegment(Source));
		private TextSegment TargetSegment() => _targetSegment ?? (_targetSegment = ToSegment(Target));

		// FIXME I will implement this properly for multiple languages
		public TextSegment Text(CultureInfo language) => language.IsoLanguageName().Equals(SourceLanguage, StringComparison.OrdinalIgnoreCase)
			? SourceSegment() : TargetSegment();

		public bool HasLanguage(CultureInfo language) => true;

		private TextSegment _sourceSegment, _targetSegment;

		public SimpleResult ToSimpleResult(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var sr = new SimpleResult
			{
				TranslateTime = TranslateTime ?? DateTime.MinValue,
				ConfirmationLevel = ConfirmationLevel, 
				Origin = TranslationUnitOrigin.TM,
				// FIXME
				Source = new List<TextPart> { new TextPart { Text = SourceSegment().OriginalText }},
				// FIXME
				Target = new List<TextPart> { new TextPart { Text = TargetSegment().OriginalText } },
			};
			return sr;
		}
	}
}
