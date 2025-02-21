using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class TmLanguageDirection : ModelBase
	{		
		private string _source;
		private string _target;		
		private int _translationUnitsCount;		

		public string Source
		{
			get => _source;
			set
			{
				_source = value;
				OnPropertyChanged(nameof(Source));
			}
		}	

		public string Target
		{
			get => _target;
			set
			{				
				_target = value;
				OnPropertyChanged(nameof(Target));
			}
		}

		public int TranslationUnitsCount
		{

			get => _translationUnitsCount;
			set
			{
				_translationUnitsCount = value;
				OnPropertyChanged(nameof(TranslationUnitsCount));
			}
		}

		[JsonIgnore]
		[XmlIgnore]
		public string SourceDisplayName => !string.IsNullOrEmpty(Source) ? new CultureInfo(Source).DisplayName : string.Empty;

		[JsonIgnore]
		[XmlIgnore]
		public string TargetDisplayName => !string.IsNullOrEmpty(Target) ? new CultureInfo(Target).DisplayName : string.Empty;

		[JsonIgnore]
		[XmlIgnore]
		public List<TmTranslationUnit> TranslationUnits { get; set; }
	}
}
