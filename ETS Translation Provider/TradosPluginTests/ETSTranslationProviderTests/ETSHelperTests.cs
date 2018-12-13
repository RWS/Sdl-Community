using ETSLPConverter;
using ETSTranslationProvider;
using ETSTranslationProvider.ETSApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Globalization;
using System.Linq;

namespace TradosPluginTests
{
    [TestClass]
    public class ETSHelperTests
    {
        public ETSHelperTests()
        {
            apiKeyTranslationOptions = new TranslationOptions(new Uri(StringResource.ApiUrl));
            apiKeyTranslationOptions.UseBasicAuthentication = false;
            apiKeyTranslationOptions.ApiToken = StringResource.ApiKey;

            basicAuthTranslationOptions = new TranslationOptions(new Uri(StringResource.ApiUrl));
            basicAuthTranslationOptions.UseBasicAuthentication = true;
            userCredentials = new GenericCredentials(StringResource.Username, StringResource.Password);
        }

        private GenericCredentials userCredentials;
        private TranslationOptions apiKeyTranslationOptions;
        private TranslationOptions basicAuthTranslationOptions;

        private readonly LanguagePair engFraLP = new LanguagePair(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
        private readonly LanguagePair engInvLP = new LanguagePair(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo(0x007F));

        /// <summary>
        /// Tests that verifying the api token specified in the string resources file does not return an error
        /// </summary>
        [TestMethod]
        public void ETSApi_VerifyAPIKey_NoExceptionThrown()
        {
            GenericCredentials credentials = new GenericCredentials(null, null);
            credentials["API-Key"] = apiKeyTranslationOptions.ApiToken;
            credentials["UseApiKey"] = "true";
            try
            {
                ETSTranslatorHelper.VerifyBasicAPIToken(apiKeyTranslationOptions, credentials);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        /// <summary>
        /// Tests GetLanguagePairs and verifies that the ETS instance the tests are running on has EngFra as a
        /// language pair
        /// </summary>
        [TestMethod]
        public void ETSApi_FetchLPs_AtLeastFraEngReturned()
        {
            ETSLanguagePair[] lps = ETSTranslatorHelper.GetLanguagePairs(apiKeyTranslationOptions);
            if (!lps.Any())
                Assert.Fail("Expected at least one LP returned");
            // I realize not every ETS server will have german, but for sake of further tests, the host we choose to test with should.
            // Otherwise we'd have to code up an entire mock for the ETS API.
            if (!lps.Any(lp =>
                lp.SourceLanguageId.Equals("eng", StringComparison.OrdinalIgnoreCase)
                && lp.TargetLanguageId.Equals("fra", StringComparison.OrdinalIgnoreCase)
            ))
                Assert.Fail("Expected EngFra as one of the LPs");
        }

        /// <summary>
        /// Tests we are able to retrieve a non-null authentication token from the server using the user credentials
        /// </summary>
        [TestMethod]
        public void ETSApi_GetBasicAuthToken_GetsToken()
        {
            try
            {
                string token = ETSTranslatorHelper.GetAuthToken(basicAuthTranslationOptions, userCredentials);
                if (token == null)
                    Assert.Fail("Expected token, but got null");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        /// <summary>
        /// Verifies that ETSHelper is able to translate a basic string using both api key and user credentials
        /// </summary>
        [TestMethod]
        public void ETSApi_FetchTranslationNoTags_ValidTranslation()
        {

            XliffConverter.xliff xliffDocument = new XliffConverter.xliff(engFraLP.SourceCulture, engFraLP.TargetCulture);
            xliffDocument.AddSourceText(StringResource.BasicText);

            string translatedXliffText = ETSTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engFraLP, xliffDocument);
            XliffConverter.xliff translatedXliff = XliffConverter.Converter.ParseXliffString(translatedXliffText);

            Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
            Assert.AreEqual(
                   StringResource.BasicTranslation,
                   translatedXliff.GetTargetSegments()[0].ToString());

            string token = ETSTranslatorHelper.GetAuthToken(basicAuthTranslationOptions, userCredentials);
            basicAuthTranslationOptions.ApiToken = token;

            translatedXliffText = ETSTranslatorHelper.GetTranslation(basicAuthTranslationOptions, engFraLP, xliffDocument);
            translatedXliff = XliffConverter.Converter.ParseXliffString(translatedXliffText);

            Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
            Assert.AreEqual(
                   StringResource.BasicTranslation,
                   translatedXliff.GetTargetSegments()[0].ToString());
        }

        /// <summary>
        /// Tests that abnormal characters (such as emojis) are able to be translated correctly by ETS
        /// </summary>
        [TestMethod]
        public void ETSApi_FetchTranslationEmoji_ValidTranslation()
        {
            XliffConverter.xliff xliffDocument = new XliffConverter.xliff(engFraLP.SourceCulture, engFraLP.TargetCulture, encodeUTF16: true);

            xliffDocument.AddSourceText(StringResource.BasicEmojiTest);

            string translatedXliffText = ETSTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engFraLP, xliffDocument);
            XliffConverter.xliff translatedXliff = XliffConverter.Converter.ParseXliffString(translatedXliffText);
            Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
            Assert.AreEqual(
                   StringResource.BasicEmojiTranslation,
                   translatedXliff.GetTargetSegments()[0].ToString());
        }

        /// <summary>
        /// Verifies that we throw an exception when ETS returns an error (in this case, caused by an invalid LP)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ETSApi_FetchTranslation_InvalidTranslation()
        {
            XliffConverter.xliff xliffDocument = new XliffConverter.xliff(engFraLP.SourceCulture, engFraLP.TargetCulture);
            xliffDocument.AddSourceText(StringResource.BasicText);

            ETSTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engInvLP, xliffDocument);
        }

        /// <summary>
        /// Verifies that we're able to fetch the ETS code of culture codes
        /// </summary>
        [TestMethod]
        public void ETSCode_GetETSCode_ValidCode()
        {
            Assert.AreEqual("eng", engFraLP.SourceCulture.ToETSCode());
            Assert.AreEqual("fra", engFraLP.TargetCulture.ToETSCode());
            Assert.AreEqual("chi", CultureInfo.GetCultureInfo(0x0004).ToETSCode());
            Assert.AreEqual("cht", CultureInfo.GetCultureInfo(0x7C04).ToETSCode());
            Assert.AreEqual("ptb", CultureInfo.GetCultureInfo(0x0416).ToETSCode());
            Assert.AreEqual("fad", CultureInfo.GetCultureInfo("prs-af").ToETSCode());
        }

        /// <summary>
        /// Tests that if we try to get an ETS code using an invalid culture code, it returns null
        /// </summary>
        [TestMethod]
        public void ETSCode_GetETSCode_InvalidCode()
        {
            Assert.AreEqual(string.Empty, engInvLP.TargetCulture.ToETSCode());
        }


        /// <summary>
        /// Tests that we set the API version correctly depending on the host's api version
        /// </summary>
        public void ETSApi_SetETSApiVersion_v8()
        {
            apiKeyTranslationOptions.APIVersion = APIVersion.Unknown;
            ETSTranslatorHelper.SetETSApiVersion(apiKeyTranslationOptions);
            Assert.AreEqual(APIVersion.v2, apiKeyTranslationOptions.APIVersion);
        }
    }
}
