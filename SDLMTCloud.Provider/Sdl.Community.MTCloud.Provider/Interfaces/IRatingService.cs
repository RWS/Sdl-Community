using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Service.Interface;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IRatingService
	{
		void IncreaseRating();
		void DecreaseRating();
		void SetRateOptionFromShortcuts(string optionName);
		void SetTranslationService(IFeedbackService translationService);
		bool IsSendFeedbackEnabled { get; set; }
		bool? AutoSendFeedback { get; set; }
		ICommand SendFeedbackCommand { get; }
	}
}
