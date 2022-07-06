using System.Windows.Input;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IRatingService
	{
		bool? AutoSendFeedback { get; set; }

		bool IsSendFeedbackEnabled { get; set; }

		ICommand SendFeedbackCommand { get; }

		void DecreaseRating();

		void IncreaseRating();

		void SetRateOptionFromShortcuts(string optionName);

		void SetTranslationService(ITranslationService translationService);
	}
}