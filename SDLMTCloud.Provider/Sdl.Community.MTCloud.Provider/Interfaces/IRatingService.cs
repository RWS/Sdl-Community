using System.Windows.Input;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IRatingService
	{
		void IncreaseRating();
		void DecreaseRating();
		void SetRateOptionFromShortcuts(string optionName);
		void SetTranslationService(ITranslationService translationService);
		bool IsSendFeedbackEnabled { get; set; }
		bool? AutoSendFeedback { get; set; }
		ICommand SendFeedbackCommand { get; }
	}
}
