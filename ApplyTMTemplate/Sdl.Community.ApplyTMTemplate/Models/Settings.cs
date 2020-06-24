using Sdl.Community.ApplyTMTemplate.ViewModels;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Settings : ModelBase
	{
		private bool _abbreviationsChecked;

		private bool _ordinalFollowersChecked;
		private bool _recognizersChecked;
		private bool _segmentationRulesChecked;
		private bool _variablesChecked;
		private bool _wordCountFlagsChecked;
		private bool _timesChecked;
		private bool _numbersChecked;
		private bool _measurementsChecked;
		private bool _datesChecked;
		private bool _currenciesChecked;

		public Settings()
		{
			AbbreviationsChecked = true;
			VariablesChecked = true;
			OrdinalFollowersChecked = true;
			SegmentationRulesChecked = true;
			DatesChecked = true;
			TimesChecked = true;
			NumbersChecked = true;
			MeasurementsChecked = true;
			CurrenciesChecked = true;
			RecognizersChecked = true;
			WordCountFlagsChecked = true;
		}

		public bool AbbreviationsChecked
		{
			get => _abbreviationsChecked;
			set
			{
				_abbreviationsChecked = value;
				OnPropertyChanged();
			}
		}

		public bool CurrenciesChecked
		{
			get => _currenciesChecked;
			set
			{
				_currenciesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool DatesChecked
		{
			get => _datesChecked;
			set
			{
				_datesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool MeasurementsChecked
		{
			get => _measurementsChecked;
			set
			{
				_measurementsChecked = value;
				OnPropertyChanged();
			}
		}

		public bool NumbersChecked
		{
			get => _numbersChecked;
			set
			{
				_numbersChecked = value;
				OnPropertyChanged();
			}
		}

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged();
			}
		}

		public bool RecognizersChecked
		{
			get => _recognizersChecked;
			set
			{
				_recognizersChecked = value;
				OnPropertyChanged();
			}
		}

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool TimesChecked
		{
			get => _timesChecked;
			set
			{
				_timesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool WordCountFlagsChecked
		{
			get => _wordCountFlagsChecked;
			set
			{
				_wordCountFlagsChecked = value;
				OnPropertyChanged();
			}
		}
	}
}