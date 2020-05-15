namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IRatingService
	{
		void IncreaseRating();
		void DecreaseRating();
		void SetRateOptionFromShortcuts(string optionName);
		void SetOptionTooltip(string optionName, string tooltip);
	}
}
