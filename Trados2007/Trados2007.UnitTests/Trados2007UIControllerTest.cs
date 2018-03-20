namespace Trados2007.UnitTests
{
    using Sdl.Community.Trados2007.UI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using System.Windows.Forms;
    using Sdl.Community.Trados2007;

    [TestClass()]
    public class Trados2007UIControllerTest
    {
        /// <summary>
        ///A test for ShowDialog
        ///</summary>
        [TestMethod()]
        public void ShowDialogTest()
        {
            LanguagePair[] languagePairs = new[] { new LanguagePair("en-US", "de-DE"), new LanguagePair("ru-RU", "en-GB") };
            ITranslationProviderCredentialStore translationProviderCredentialStore = null; 
            var target = new MainDialog(languagePairs, translationProviderCredentialStore);


            IWin32Window owner = null;

            target.ShowDialog(owner);
        }
    }
}
