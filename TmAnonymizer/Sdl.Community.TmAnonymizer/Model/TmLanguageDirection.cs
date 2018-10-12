using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TmLanguageDirection : ModelBase
	{		
		private CultureInfo _source;
		private CultureInfo _target;		
		private int _translationUnitsCount;		

		public CultureInfo Source
		{
			get => _source;
			set
			{
				_source = value;
				OnPropertyChanged(nameof(Source));
			}
		}

		public CultureInfo Target
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
		public List<TmTranslationUnit> TranslationUnits { get; set; }
	}
}
