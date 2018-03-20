using Sdl.Community.Trados2007;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.Core;

namespace Trados2007.UnitTests
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    ///This is a test class for FileBasedTrados2007LanguageDirectionTest and is intended
    ///to contain all FileBasedTrados2007LanguageDirectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileBasedTrados2007LanguageDirectionTest
    {
        private string testDir = AppDomain.CurrentDomain.BaseDirectory;

        [DeploymentItem("Files/EN-US_DE-DE.tmw")]
        [DeploymentItem("Files/EN-US_DE-DE.iix")]
        [DeploymentItem("Files/EN-US_DE-DE.mdf")]
        [DeploymentItem("Files/EN-US_DE-DE.mtf")]
        [DeploymentItem("Files/EN-US_DE-DE.mwf")]
        private FileBasedTrados2007LanguageDirection GetLanguageDirection()
        {
            var provider = new FileBasedTrados2007TranslationMemory(testDir + "\\Files\\EN-US_DE-DE.tmw");
            FileBasedTrados2007LanguageDirection target = new FileBasedTrados2007LanguageDirection(provider);

            return target;
        }

        [TestMethod()]
        public void FileBasedTrados2007LanguageDirection_Constructor_OK()
        {
            var direction = this.GetLanguageDirection();

            // could not test searches via unit tests - LanguageResourceFileNotFound exception is thrown.
            // Possible workaround - use template method to replace tokenizer.
            Assert.IsNotNull(direction.TranslationProvider);
        }
    }
}
