namespace Trados2007.UnitTests
{
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using Sdl.Community.Trados2007;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class ServerBasedTrados2007TranslationMemoryTest
    {
        [TestMethod()]
        public void ServerBasedTrados2007TranslationMemory_Constructor_OpensExisting()
        {
            string serverAdress = "VQ01EN-S3E-08";
            string tmName = "General04_ENus_DEde";
            string container = "2007_TMSERVER_TMC2";
            string userName = @"Andy";
            string password = @"Andy";
            TranslationMemoryAccessMode acessMode = TranslationMemoryAccessMode.Maintenance;

            var target = new ServerBasedTrados2007TranslationMemory(serverAdress, tmName, container, userName, password, acessMode);

            Assert.IsNotNull(target);
            Assert.IsTrue(target.IsReadOnly);
            Assert.AreEqual(new LanguagePair("en-US", "de-DE"), target.LanguageDirection);
            Assert.AreEqual("SDL Trados 2007 General04_ENus_DEde", target.Name);
            Assert.IsTrue(target.SupportsConcordanceSearch);
            Assert.IsTrue(target.SupportsDocumentSearches);
            Assert.IsTrue(target.SupportsFilters);
            Assert.IsTrue(target.SupportsFuzzySearch);
            Assert.IsTrue(target.SupportsMultipleResults);
            Assert.IsTrue(target.SupportsPenalties);
            Assert.IsTrue(target.SupportsPlaceables);
            Assert.IsTrue(target.SupportsScoring);
            Assert.IsTrue(target.SupportsSearchForTranslationUnits);
            Assert.IsTrue(target.SupportsSourceConcordanceSearch);
            Assert.IsTrue(target.SupportsStructureContext);
            Assert.IsTrue(target.SupportsTaggedInput);
            Assert.IsFalse(target.SupportsTargetConcordanceSearch);
            Assert.IsTrue(target.SupportsTranslation);
            Assert.IsFalse(target.SupportsUpdate);
            Assert.IsTrue(target.SupportsWordCounts);
            Assert.AreEqual(TranslationMethod.TranslationMemory, target.TranslationMethod);
            Assert.IsTrue(target.SupportsLanguageDirection(new LanguagePair("en-US", "de-DE")));
            Assert.IsNotNull(target.Uri);
        }
    }
}
