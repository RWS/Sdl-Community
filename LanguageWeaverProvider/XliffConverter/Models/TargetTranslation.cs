using System.Globalization;
using System.Xml.Serialization;
using LanguageWeaverProvider.XliffConverter.SegmentParser;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.Models
{
	public class TargetTranslation
	{		
		private CultureInfo _targetCulture;
		
		private string _targetLanguage;

		[XmlAttribute("xml:lang")]
		public string TargetLanguage
		{
			get => _targetLanguage;
			set
			{
				_targetLanguage = value;

				// verify TargetCulture isn't the desired value before setting,
				// else you run the risk of infinite loop
				var newTargetCulture = CultureInfo.GetCultureInfo(value);
				if (!Equals(TargetCulture, newTargetCulture))
				{
					TargetCulture = newTargetCulture;
				}
			}
		}

		[XmlIgnore]
		public CultureInfo TargetCulture
		{
			get => _targetCulture;
			set
			{
				_targetCulture = value;

				// verify targetLanguage isn't the desired value before setting,
				// else you run the risk of infinite loop
				var newTargetLanguage = value.ToString();
				if (TargetLanguage != newTargetLanguage)
				{
					TargetLanguage = newTargetLanguage;
				}
			}
		}

		[XmlText]
		public string Text { get; set; }

		[XmlIgnore]
		public Segment TargetSegment => Parser.ParseLine(Converter.Converter.RemoveXliffTags(Text));
	}
}
