using Sdl.Community.ApplyTMTemplate.ViewModels;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Settings : ModelBase
	{
		private bool _abbreviationsChecked;

		private bool _variablesChecked;

		private bool _ordinalFollowersChecked;

		private bool _segmentationRulesChecked;

		public Settings()
		{
			AbbreviationsChecked = true;
			VariablesChecked = true;
			OrdinalFollowersChecked = true;
			SegmentationRulesChecked = true;
			DatesChecked = true;
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

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
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

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool DatesChecked { get; set; }
	}
}