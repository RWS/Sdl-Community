using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class RateItViewModel:BaseViewModel
	{
		private ITranslationService _translationService;

		public RateItViewModel(ITranslationService translationService)
		{
			_translationService = translationService;
			if (_translationService != null)
			{
				_translationService.TranslationReceived -= _translationService_TranslationReceived;
				_translationService.TranslationReceived += _translationService_TranslationReceived;
			}
		}

		private void _translationService_TranslationReceived(Feedback translationFeedback)
		{
			
		}
	}
}
