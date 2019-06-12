using System;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Application = System.Windows.Application;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	
	[TranslationProviderWinFormsUi(
		Id = "SDLBeGlobal(NMT)ProviderUi",
		Name = "SDLBeGlobal(NMT)ProviderUi",
		Description = "SDL BeGlobal (NMT) Translation Provider")]
	public class BeGlobalProviderUi  : ITranslationProviderWinFormsUI
	{
		public string TypeName => "SDL BeGlobal (NMT) Translation Provider"; 
		public string TypeDescription => "SDL BeGlobal (NMT) Translation Provider";
		public bool SupportsEditing => true;
		private readonly StudioCredentials _studioCredentials = new StudioCredentials();
		public static readonly Log Log = Log.Instance;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				AppItializer.EnsureInitializer();
				var options = new BeGlobalTranslationOptions();
				var token = string.Empty;

				Application.Current?.Dispatcher?.Invoke(() =>
				{
					token = _studioCredentials.GetToken();
				});
				if (!string.IsNullOrEmpty(token))
				{
					var beGlobalWindow = new BeGlobalWindow();
					var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, options, languagePairs);
					beGlobalWindow.DataContext = beGlobalVm;

					beGlobalWindow.ShowDialog();
					if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
					{
						var beGlobalService = new BeGlobalV4Translator(beGlobalVm.Options.Model);
						beGlobalVm.Options.BeGlobalService = beGlobalService;
						var provider = new BeGlobalTranslationProvider(options)
						{
							Options = beGlobalVm.Options
						};
						return new ITranslationProvider[] { provider };
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Browse: {e.Message}\n {e.StackTrace}");
			}
			return null;
		}

		[STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var editProvider = translationProvider as BeGlobalTranslationProvider;

				if (editProvider == null)
				{
					return false;
				}
				var token = string.Empty;
				AppItializer.EnsureInitializer();
				Application.Current?.Dispatcher?.Invoke(() =>
				{
					token = _studioCredentials.GetToken();
				});

				if (!string.IsNullOrEmpty(token))
				{
					var beGlobalWindow = new BeGlobalWindow();
					var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, editProvider.Options, languagePairs);
					beGlobalWindow.DataContext = beGlobalVm;

					beGlobalWindow.ShowDialog();
					if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
					{
						editProvider.Options = beGlobalVm.Options;
						return true;
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Edit window: {e.Message}\n {e.StackTrace}");
			}
			return false;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, BeGlobalTranslationProvider.ListTranslationProviderScheme,
				StringComparison.OrdinalIgnoreCase);
			return supportsProvider;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderDisplayInfo
			{
				Name = "SDL BeGlobal (NMT) Translation provider",
				TooltipText = "SDL BeGlobal (NMT) Translation provider",
				SearchResultImage = PluginResources.logoRes,
				TranslationProviderIcon = PluginResources.global
			};
			return info;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		} 
	}
}
