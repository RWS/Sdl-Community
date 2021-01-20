﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.Provider;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeTranslationProviderTests
{
	[TestClass]
    public class TranslationOptionsTests
    {
        /// <summary>
        /// Tests the translation options' default values are what we desire
        /// </summary>
        [TestMethod]
        public void Constructor_Parameterless_NullOrEmptyValues()
        {
            var options = new TranslationOptions();

            Assert.AreEqual(options.Host, null);
            Assert.AreEqual(options.ApiToken, null);
            Assert.AreEqual(options.Port, 8001);
        }

        /// <summary>
        /// Tests the default uri in translation options is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void Constructor_Parameterless_ThrowsException()
        {
            var options = new TranslationOptions();
            var invalidUri = options.Uri;
        }

        /// <summary>
        /// Tests that when you enter in a uri, it resolves it via DNS to the actual uri,
        /// eg "localhost" resolves to "wgitbuild#"
        /// </summary>
        [TestMethod]
        public void Constructor_Parameters_ValidValues()
        {
            var options = new TranslationOptions(new Uri(UTStringResource.ApiUrl));

            string host = UTStringResource.ApiHost;
            int port = 8001;

            Assert.AreEqual(options.Port, port);
            // localhost should be resolved via DNS to wgitbuild#
            Assert.AreNotEqual(options.Uri.ToString(), UTStringResource.ApiUrl);
        }
    }
}