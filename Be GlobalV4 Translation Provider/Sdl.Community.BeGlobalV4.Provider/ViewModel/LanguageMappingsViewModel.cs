using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{
		private string _messageVisibility;

		public LanguageMappingsViewModel(BeGlobalTranslationOptions options)
		{
			MessageVisibility = "Collapsed";
		}

		public string MessageVisibility
		{
			get => _messageVisibility;
			set
			{
				_messageVisibility = value;
				OnPropertyChanged();
			}
		}
	}
}
