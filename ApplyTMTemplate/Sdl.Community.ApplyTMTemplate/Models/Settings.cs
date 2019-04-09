namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Settings
	{
		public Settings(bool abbreviationsChecked, bool variablesChecked, bool ordinalFollowersChecked, bool segmentationRulesChecked)
		{
			AbbreviationsChecked = abbreviationsChecked;
			VariablesChecked = variablesChecked;
			OrdinalFollowersChecked = ordinalFollowersChecked;
			SegmentationRulesChecked = segmentationRulesChecked;
		}

		public bool AbbreviationsChecked { get; set; }

		public bool VariablesChecked { get; set; }

		public bool OrdinalFollowersChecked { get; set; }

		public bool SegmentationRulesChecked { get; set; }
	}
}