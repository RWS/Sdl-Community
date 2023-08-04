using System.Globalization;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Models
{
	public class File
	{		
		private string _sourceLanguage;
		
		private string _targetLanguage;
		
		private CultureInfo _targetCulture;

		public File()
		{
			Body = new Body();
		}

		[XmlAttribute("source-language")]
		public string SourceLanguage
		{
			get => _sourceLanguage;
			set
			{
				_sourceLanguage = value;

				// verify TargetCulture isn't the desired value before setting,
				// else you run the risk of infinite loop
				var newSourceCulture = CultureInfo.GetCultureInfo(value);
				if (!Equals(SourceCulture, newSourceCulture))
				{
					SourceCulture = newSourceCulture;
				}
			}
		}
		
		[XmlIgnore]
		public CultureInfo SourceCulture { get; set; }

		[XmlAttribute("target-language")]
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
					TargetLanguage = newTargetLanguage;
			}
		}

		[XmlElement("header")]
		public Header Header { get; set; }

		[XmlElement("body")]
		public Body Body { get; set; }
	}
}
