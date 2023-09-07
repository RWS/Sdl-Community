using System;
using System.Windows.Input;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.ViewModel.Interface
{
	public interface ICredentialsViewModel
	{
		ITranslationOptions TranslationOptions { get; set; }

		AuthenticationType AuthenticationType { get; set; }

		ICommand BackCommand { get; }

		ICommand ClearCommand { get; }

		ICommand SignInCommand { get; }

		ICommand SelectAuthenticationTypeCommand { get; }

		event EventHandler CloseRequested;

		event EventHandler StartLoginProcess;

		event EventHandler StopLoginProcess;
	}
}