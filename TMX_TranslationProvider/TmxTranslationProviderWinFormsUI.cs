using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TMX_TranslationProvider
{
	[TranslationProviderWinFormsUi(Id = "TmxTranslationProviderWinFormsUI",
								   Name = "TmxTranslationProviderWinFormsUI",
								   Description = "TmxTranslationProviderWinFormsUI")]
	public class TmxTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var form = new TmxOptionsForm(new TmxTranslationsOptions());
			if (form.ShowDialog(owner) != DialogResult.OK)
				return null;
			var provider = new TmxTranslationProvider(form.Options);
			return new []{ provider};
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is TmxTranslationProvider tmxProvider)
			{
				var form = new TmxOptionsForm(tmxProvider.Options);
				if (form.ShowDialog(owner) == DialogResult.OK)
				{
					tmxProvider.Options = form.Options;
					return true;
				}
			}

			return false;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			return false;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var fullFileName = new TmxTranslationsOptions(translationProviderUri).FileName;
			var friendly = "";
			if (File.Exists(fullFileName))
			{
				var fileName = Path.GetFileName(fullFileName);
				var folder = Path.GetDirectoryName(fullFileName);
				friendly = $" - {fileName} ({folder})";
			}
			return new TranslationProviderDisplayInfo
			{
				Name = $"{PluginResources.Plugin_NiceName} {friendly}",
				TranslationProviderIcon = null,
				TooltipText = PluginResources.Plugin_Tooltip,
				SearchResultImage = null,
			};
		}

		public bool SupportsEditing => true;

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return translationProviderUri.Scheme.Equals(TmxTranslationProvider.ProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public string TypeDescription => PluginResources.Plugin_Description;
		public string TypeName => PluginResources.Plugin_NiceName;

	}
}
