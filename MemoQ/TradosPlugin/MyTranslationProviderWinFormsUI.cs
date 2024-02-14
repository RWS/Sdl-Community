using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

using System.ServiceModel;
using TMProvider;

namespace TradosPlugin
{
    [TranslationProviderWinFormsUi(
        Id = "memoQTMPluginWinFormsUI",
        Name = "memoQTMPluginWinFormsUI",
        Description = "memoQTMPluginWinFormsUI")]
    public class MyTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        /// <summary>
        /// Opens the form to configure the servers and TMs for the first time.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            List<ITranslationProvider> providerList = new List<ITranslationProvider>();

            try
            {
                // languages
                List<string> projectLangs = new List<string>();
                foreach (LanguagePair lp in languagePairs)
                {
                    string mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.SourceCulture.Name);
                    // the code might not be in the list -> get the main language code
                    if (mqCode == null) mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.SourceCulture.Name.Substring(0, 2));
                    if (mqCode != null && !projectLangs.Contains(mqCode)) projectLangs.Add(mqCode);

                    mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.TargetCulture.Name);
                    // the code might not be in the list -> get the main language code
                    if (mqCode == null) mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.TargetCulture.Name.Substring(0, 2));
                    if (mqCode != null && !projectLangs.Contains(mqCode)) projectLangs.Add(mqCode);
                }

                using (SettingsForm frm = new SettingsForm(null, projectLangs))
                {
                    frm.ShowDialog();
                    // now only works for 1 single LT provider
                    if (frm.DialogResult == DialogResult.OK && frm.TranslationProvider != null)
                    {
                        MemoQTranslationProvider mQTRProv = frm.TranslationProvider as MemoQTranslationProvider;
                        if (mQTRProv == null) return null;
                        foreach (TMProviderBase p in mQTRProv.MemoQTMProviders)
                        {
                            credentialStore.AddCredential(new Uri(p.Settings.URL), new TranslationProviderCredential(p.Settings.UserName + " ;" + p.Settings.Password, true));
                        }
                        providerList.Add(frm.TranslationProvider);
                        return providerList.ToArray();
                    }
                    else
                    {
                        MessageBox.Show(frm, PluginResources.Settings_NoSave, PluginResources.Error_Warning);
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowMessage(e);
                return providerList.ToArray();
            }
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            // show dialog to browse the TMs:
            // when user has chosen (OK), create Tr. Provider
            try
            {
                CredentialStoreHelper.AddCredentialsToProviderFromStore(credentialStore, translationProvider as MemoQTranslationProvider);

                // languages
                List<string> projectLangs = new List<string>();
                foreach (LanguagePair lp in languagePairs)
                {
                    string mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.SourceCulture.Name);
                    if (!projectLangs.Contains(mqCode)) projectLangs.Add(mqCode);
                    mqCode = LanguageMapper.GetThreeLetterForMainLangIfDoesntExist(lp.TargetCulture.Name);
                    if (!projectLangs.Contains(mqCode)) projectLangs.Add(mqCode);
                }

                using (SettingsForm frm = new SettingsForm(translationProvider as MemoQTranslationProvider, projectLangs))
                {
                    frm.ShowDialog();
                    // now only works for 1 single LT provider                    
                    if (frm.DialogResult == DialogResult.OK && frm.TranslationProvider != null)
                    {
                        MemoQTranslationProvider mQTRProv = frm.TranslationProvider as MemoQTranslationProvider;
                        if (mQTRProv == null) return false;

                        credentialStore.Clear();
                        foreach (TMProviderBase p in mQTRProv.MemoQTMProviders)
                        {
                            credentialStore.AddCredential(new Uri(p.Settings.URL), new TranslationProviderCredential(p.Settings.UserName + " ;" + p.Settings.Password, true));
                            // the translationProvider is NOT a ref parameter -> don't change the reference
                            //translationProvider = mQTRProv; 
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowMessage(e);
                return false;
            }

        }

   

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            //MessageBox.Show("This would ask for credentials from the user.");
            return true;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo tpdi = new TranslationProviderDisplayInfo();
            tpdi.Name = PluginResources.Plugin_Name;
            tpdi.TooltipText = PluginResources.Plugin_Tooltip;
            // this appears below every hit in the translation results grid
            tpdi.SearchResultImage = TradosPlugin.Properties.Resources.Q_logo16x16;
            tpdi.TranslationProviderIcon = TradosPlugin.Properties.Resources.Q_logo16x16_icon;
            return tpdi;
        }

        public bool SupportsEditing
        {
            // if true displays the Settings...
            get { return true; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return true;
        }

        public string TypeDescription
        {
            get { return PluginResources.Plugin_TypeDescription; }
        }

        public string TypeName
        {
            get { return PluginResources.Plugin_TypeName; }
        }

        #endregion


    }
}
