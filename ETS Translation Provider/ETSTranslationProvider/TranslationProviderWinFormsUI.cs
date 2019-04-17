using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	[TranslationProviderWinFormsUi(
        Id = "TranslationProviderWinFormsUI",
        Name = "TranslationProviderWinFormsUI",
        Description = "TranslationProviderWinFormsUI")]
    public class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        /// <summary>
        /// Show the plug-in settings form when the user is adding the translation provider plug-in
        /// through the GUI of SDL Trados Studio
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            Log.Logger.Trace("");
            var dialog = new ProviderConfDialog(new TranslationOptions(), credentialStore, languagePairs);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                return new ITranslationProvider[] { new TranslationProvider(dialog.Options) };
            }
            return null;
        }

        /// <summary>
        /// Determines whether the plug-in settings can be changed
        /// by displaying the Settings button in SDL Trados Studio.
        /// </summary>
        public bool SupportsEditing
        {
            get { return true; }
        }

        /// <summary>
        /// If the plug-in settings can be changed by the user,
        /// SDL Trados Studio will display a Settings button.
        /// By clicking this button, users raise the plug-in user interface,
        /// in which they can modify any applicable settings
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProvider"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        public bool Edit(
			IWin32Window owner,
			ITranslationProvider translationProvider,
			LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
        {
            Log.Logger.Trace("");
            var editProvider = translationProvider as TranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            var dialog = new ProviderConfDialog(editProvider.Options, credentialStore, languagePairs);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                editProvider.Options = dialog.Options;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Can be used in implementations in which a user login is required, e.g.
        /// for connecting to an online translation provider.
        /// Trados Studio fires calls method when a TranslationProviderAuthenticationException is thrown (e.g. in the TranslationProviderFactory class)
        /// If credentials are not required simply set
        /// this member to return always True.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            Log.Logger.Trace("");
            var options = new TranslationOptions(translationProviderUri);

			//only show controls for setting credentials, as that is the only thing that will end up getting saved
			var dialog = new ProviderConfDialog(options, credentialStore, null);
			dialog.DisplayForCredentialsOnly(); 

            return (dialog.ShowDialog(owner) == DialogResult.OK);
        }

		/// <summary>
		/// Used for displaying the plug-in info such as the plug-in name,
		/// tooltip, and icon.
		/// </summary>
		/// <param name="translationProviderUri"></param>
		/// <param name="translationProviderState"></param>
		/// <returns></returns>
		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			Log.Logger.Trace("");
			var info = new TranslationProviderDisplayInfo
			{
				Name = PluginResources.Plugin_NiceName,
				TranslationProviderIcon = PluginResources.icon_icon,
				TooltipText = PluginResources.Plugin_Tooltip,
				SearchResultImage = PluginResources.icon_symbol,
			};
			return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            Log.Logger.Trace("");
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException("translationProviderUri", "URI not supported by the plug-in.");
			}
            return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription
        {
            get { return PluginResources.Plugin_Description; }
        }

        public string TypeName
        {
            get { return PluginResources.Plugin_NiceName; }
        }
    }
}