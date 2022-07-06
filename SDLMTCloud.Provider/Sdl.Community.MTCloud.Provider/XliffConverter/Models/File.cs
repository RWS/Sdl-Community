using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Models
{
	public class File
	{
		private string _sourceLanguage;

		private CultureInfo _targetCulture;
		private string _targetLanguage;

		public File()
		{
			Body = new Body();
		}

		[XmlElement("body")]
		public Body Body { get; set; }

		[XmlElement("header")]
		public Header Header { get; set; }

		[XmlIgnore]
		public CultureInfo SourceCulture { get; set; }

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
	}
}