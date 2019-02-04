using ETSTranslationProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TradosPluginTests
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
            TranslationOptions options = new TranslationOptions();
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
            TranslationOptions options = new TranslationOptions();
            Uri invalidUri = options.Uri;
        }

        /// <summary>
        /// Tests that when you enter in a uri, it resolves it via DNS to the actual uri,
        /// eg "localhost" resolves to "wgitbuild#"
        /// </summary>
        [TestMethod]
        public void Constructor_Parameters_ValidValues()
        {
            TranslationOptions options = new TranslationOptions(new Uri(StringResource.ApiUrl));

            string host = StringResource.ApiHost;
            int port = 8001;
            Assert.AreEqual(options.Port, port);
            // localhost should be resolved via DNS to wgitbuild#
            Assert.AreNotEqual(options.Uri.ToString(), StringResource.ApiUrl);
        }
    }
}
