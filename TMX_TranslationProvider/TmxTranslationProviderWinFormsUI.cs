using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TMX_Lib.Search;
using TMX_UI.View;
using TMX_UI.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TMX_TranslationProvider
{
	[TranslationProviderWinFormsUi(Id = "TmxTranslationProviderWinFormsUI",
								   Name = "TmxTranslationProviderWinFormsUI",
								   Description = "TmxTranslationProviderWinFormsUI")]
	public class TmxTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{

		internal static Form GetParentForm()
		{
			Form parentForm = null;
			foreach (Control form in Application.OpenForms)
			{
				if (string.Compare(form.Name, "SettingsDialogForm", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return form as Form;
				}
				if (string.Compare(form.Name, "StudioWindowForm", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					parentForm = form as Form;
				}
			}

			return parentForm;
		}

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var form = new OptionsView();
			var interopHelper = new System.Windows.Interop.WindowInteropHelper(form);
			interopHelper.Owner = GetParentForm().Handle;

			if (form.ShowDialog() != true)
				return null;
			var options = new TmxTranslationsOptions();
			var provider = new TmxTranslationProvider(options);
			provider.UpdateOptions(form.ViewModel);
			return new []{ provider };
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			if (translationProvider is TmxTranslationProvider tmxProvider)
			{
				var form = new OptionsView(tmxProvider.Options.Databases, tmxProvider.Options.CareForLocale);
				var interopHelper = new System.Windows.Interop.WindowInteropHelper(form);
				interopHelper.Owner = GetParentForm().Handle;

				if (form.ShowDialog() == true)
				{
					tmxProvider.UpdateOptions(form.ViewModel);
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
			var friendly = new TmxTranslationsOptions(translationProviderUri).FriendlyName;

			var icon = PluginResources.TMX_TM_Provider;
			return new TranslationProviderDisplayInfo
			{
				Name = $"{PluginResources.Plugin_NiceName} {friendly}",
				TranslationProviderIcon = icon,
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
