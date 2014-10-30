using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Community.ControlledMTProviders.Integration;
using Sdl.Community.ControlledMTProviders.Provider.UI;

namespace Sdl.Community.ControlledMTProviders.Provider
{
    [TranslationProviderWinFormsUi(
        Id = "Sdl.Community.ControlledMTProvidersWinFormsUI",
        Name = "ControledMTProviders",
        Description = "ControledMTProviders")]
    public class ControlledMTProvidersWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            List<ITranslationProvider> mtProviders = new List<ITranslationProvider>();
            ControlledMTProvidersProvider controlledMTProvider = new ControlledMTProvidersProvider();

            using (MTProvidersDialog pd = new MTProvidersDialog(controlledMTProvider))
            {
                DialogResult result = pd.ShowDialog(owner);

                if(result != DialogResult.OK)
                {
                    return null;
                }

                var mtProvidersUi = pd.SelectedTranslationProvidersWinformsUI;

                if (mtProvidersUi.Count == 0) return null;
                foreach (var mtProviderUi in mtProvidersUi)
                {
                    var innerProviders = mtProviderUi.Browse(owner, languagePairs, credentialStore);
                    if (innerProviders != null && innerProviders.All(x => x.TranslationMethod == TranslationMethod.MachineTranslation))
                    {
                        mtProviders.AddRange(innerProviders);
                    }
                }
            }

            return new ITranslationProvider[] { new ControlledMTProvidersProvider(mtProviders) };
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var controlledMTProvider = (ControlledMTProvidersProvider)translationProvider;

            List<ITranslationProvider> mtProviders = new List<ITranslationProvider>();

            //move this to mmtProvider
          
            using (MTProvidersDialog pd = new MTProvidersDialog(controlledMTProvider))
            {
                DialogResult result = pd.ShowDialog(owner);

                if (result != DialogResult.OK)
                {
                    return false;
                }

                var mtProvidersUi = pd.SelectedTranslationProvidersWinformsUI;
                if (mtProvidersUi.Count == 0) return false;

                foreach (var mtProviderUi in mtProvidersUi)
                {
                    var innerProviders = mtProviderUi.Browse(owner, languagePairs, credentialStore);
                    if (innerProviders != null && innerProviders.All(x => x.TranslationMethod == TranslationMethod.MachineTranslation))
                    {
                        mtProviders.AddRange(innerProviders);
                    }
                }
            }
            controlledMTProvider.MTProviders = mtProviders;
            return true;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo
            {
                 Name = "Controlled MT Providers",
                 TooltipText = "Controlled MT Providers",
                 TranslationProviderIcon = PluginResources.icon
            };

            return info;
        }

        public bool SupportsEditing
        {
            get { return true; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.Equals(new Uri(ControlledMTProvidersProvider.ProviderUri).Scheme);
        }

        public string TypeDescription
        {
            get { return "Select MT providers that can be disabled with one click."; }
        }

        public string TypeName
        {
            get { return "Controlled MT Providers ..."; }
        }

        #endregion
    }
}
