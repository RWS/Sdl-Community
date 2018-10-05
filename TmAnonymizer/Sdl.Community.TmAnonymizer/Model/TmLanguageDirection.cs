using System.Globalization;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TmLanguageDirection : ModelBase
	{		
		private CultureInfo _source;
		private CultureInfo _target;		
		private int _translationUnits;

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

		public int TranslationUnits
		{

			get => _translationUnits;
			set
			{
				_translationUnits = value;
				OnPropertyChanged(nameof(TranslationUnits));
			}
		}		
	}
}
