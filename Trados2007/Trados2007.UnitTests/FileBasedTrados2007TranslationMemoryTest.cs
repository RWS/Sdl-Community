namespace Trados2007.UnitTests
{
    using System.Globalization;
    using System.IO;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using Sdl.TranslationStudio.Plugins.Trados2007;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class FileBasedTrados2007TranslationMemoryTest
    {
        private string testDir = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        [DeploymentItem("Files/EN-US_DE-DE.tmw")]
        [DeploymentItem("Files/EN-US_DE-DE.iix")]
        [DeploymentItem("Files/EN-US_DE-DE.mdf")]
        [DeploymentItem("Files/EN-US_DE-DE.mtf")]
        [DeploymentItem("Files/EN-US_DE-DE.mwf")]
        public void FileBasedTrados2007TranslationMemory_Constructor_OpensFileTm()
        {
            FileBasedTrados2007TranslationMemory provider = new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\EN-US_DE-DE.tmw");

            Assert.IsTrue(provider.IsReadOnly);
            Assert.IsNotNull(provider.LanguageDirection);
            Assert.AreEqual("en-US", provider.LanguageDirection.SourceCultureName);
            Assert.AreEqual("de-DE", provider.LanguageDirection.TargetCultureName);
            Assert.AreEqual(@"EN-US_DE-DE", provider.Name);
            Assert.AreEqual(@"SDL Trados 2007 Translation Provider", provider.StatusInfo.StatusMessage);
            Assert.IsTrue(provider.SupportsConcordanceSearch);
            Assert.IsTrue(provider.SupportsDocumentSearches);
            Assert.IsTrue(provider.SupportsFilters);
            Assert.IsTrue(provider.SupportsFuzzySearch);
            Assert.IsTrue(provider.SupportsLanguageDirection(new LanguagePair("en-US", "de-DE")));
            Assert.IsTrue(provider.SupportsMultipleResults);
            Assert.IsTrue(provider.SupportsPenalties);
            Assert.IsTrue(provider.SupportsPlaceables);
            Assert.IsTrue(provider.SupportsScoring);
            Assert.IsTrue(provider.SupportsSearchForTranslationUnits);
            Assert.IsTrue(provider.SupportsSourceConcordanceSearch);
            Assert.IsTrue(provider.SupportsStructureContext);
            Assert.IsTrue(provider.SupportsTaggedInput);
            Assert.IsFalse(provider.SupportsTargetConcordanceSearch);
            Assert.IsTrue(provider.SupportsTranslation);
            Assert.IsFalse(provider.SupportsUpdate);
            Assert.IsTrue(provider.SupportsWordCounts);
            Assert.AreEqual(TranslationMethod.TranslationMemory, provider.TranslationMethod);
            Assert.IsNotNull(provider.Uri);
        }

        [TestMethod]
        [DeploymentItem("Files/McD-En-US-EN-US_DE-DE.tmw")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileBasedTrados2007TranslationMemory_Constructor_ThrowsFileNotFoundException()
        {
            FileBasedTrados2007TranslationMemory provider = new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\McD-En-US-EN-US_DE-DE.tmw");

            Assert.Fail("Should throw exception before this step.");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void FileBasedTrados2007TranslationMemory_Constructor_FailsOnMissingFile()
        {
            FileBasedTrados2007TranslationMemory provider = new FileBasedTrados2007TranslationMemory(testDir + "\\MysticFolder\\EN-US_DE-DE.tmw");
        }

        [TestMethod]
        [DeploymentItem("Files/EN-US_DE-DE.tmw")]
        [DeploymentItem("Files/EN-US_DE-DE.iix")]
        [DeploymentItem("Files/EN-US_DE-DE.mdf")]
        [DeploymentItem("Files/EN-US_DE-DE.mtf")]
        [DeploymentItem("Files/EN-US_DE-DE.mwf")]
        public void FileBasedTrados2007TranslationMemory_SerializeState_DoesNothing()
        {
            FileBasedTrados2007TranslationMemory provider = new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\EN-US_DE-DE.tmw");

            string actual = provider.SerializeState();

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        [DeploymentItem("Files/EN-US_DE-DE.tmw")]
        [DeploymentItem("Files/EN-US_DE-DE.iix")]
        [DeploymentItem("Files/EN-US_DE-DE.mdf")]
        [DeploymentItem("Files/EN-US_DE-DE.mtf")]
        [DeploymentItem("Files/EN-US_DE-DE.mwf")]
        public void FileBasedTrados2007TranslationMemory_LoadState_DoesNothing()
        {
            FileBasedTrados2007TranslationMemory provider =
                new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\EN-US_DE-DE.tmw");

            provider.LoadState(string.Empty);
        }

        [TestMethod]
        [DeploymentItem("Files/EN-US_DE-DE.tmw")]
        [DeploymentItem("Files/EN-US_DE-DE.iix")]
        [DeploymentItem("Files/EN-US_DE-DE.mdf")]
        [DeploymentItem("Files/EN-US_DE-DE.mtf")]
        [DeploymentItem("Files/EN-US_DE-DE.mwf")]
        public void FileBasedTrados2007TranslationMemory_GetLanguageDirection_ReturnsCorrectForSupported()
        {
            FileBasedTrados2007TranslationMemory provider = new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\EN-US_DE-DE.tmw");

            var actual = provider.GetLanguageDirection(new LanguagePair("en-US", "de-DE"));

            // note: checking only TP-related stuff. LD-related stuff is checked in other tests
            Assert.AreEqual(CultureInfo.GetCultureInfo(1033), actual.SourceLanguage);
            Assert.AreEqual(CultureInfo.GetCultureInfo(1031), actual.TargetLanguage);
            Assert.AreEqual(provider, actual.TranslationProvider);
        }
    }
}
