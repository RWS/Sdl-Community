using System;
using System.Windows.Forms;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
								   Name = "Translation_Provider_Plug_inWinFormsUI",
								   Description = "Translation_Provider_Plug_inWinFormsUI")]
	internal class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new TranslationOptions();
			var mainViewModel = new MainViewModel(options);
			var mainView = new MainWindowView { DataContext = mainViewModel };
			var dialogResult = mainView.ShowDialog();
			if ((bool)dialogResult)
			{
				return null;
			}

			var translationProvider = new TranslationProvider(options);
			return new ITranslationProvider[] { translationProvider };
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		private MainWindowViewModel ShowRequestedView(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore, ITranslationOptions loadOptions, bool showSettingsView = false)
		{
			SetSavedCredentialsOnUi(credentialStore, loadOptions);
			var mainWindowViewModel = new MainWindowViewModel(loadOptions, credentialStore, languagePairs, showSettingsView);
			var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };
			mainWindowViewModel.CloseEventRaised += () =>
			{
				UpdateProviderCredentials(credentialStore, loadOptions);
				mainWindowView.Close();
			};

			mainWindowView.ShowDialog();
			return mainWindowViewModel;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			throw new NotImplementedException();
		}

		public bool SupportsEditing
		{
			get { throw new NotImplementedException(); }
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			throw new NotImplementedException();
		}

		public string TypeDescription
		{
			get { throw new NotImplementedException(); }
		}

		public string TypeName
		{
			get { throw new NotImplementedException(); }
		}
	}
}