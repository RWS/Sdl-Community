using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.Provider;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Community.MTEdge.LPConverter;
using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.Community.MTEdge.Provider.XliffConverter.Models;
using Converter = Sdl.Community.MTEdge.Provider.XliffConverter.Converter.Converter;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeTranslationProviderTests
{
	[TestClass]
	public class SDLMTEdgeHelperTests
	{
		public SDLMTEdgeHelperTests()
		{
			apiKeyTranslationOptions = new TranslationOptions(new Uri(UTStringResource.ApiUrl));
			apiKeyTranslationOptions.UseBasicAuthentication = false;
			apiKeyTranslationOptions.ApiToken = UTStringResource.ApiKey;

			basicAuthTranslationOptions = new TranslationOptions(new Uri(UTStringResource.ApiUrl));
			basicAuthTranslationOptions.UseBasicAuthentication = true;
			userCredentials = new GenericCredentials(UTStringResource.Username, UTStringResource.Password);
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
		public void MtEdgeApi_VerifyAPIKey_NoExceptionThrown()
		{
			var credentials = new GenericCredentials(null, null);
			credentials["API-Key"] = apiKeyTranslationOptions.ApiToken;
			credentials["UseApiKey"] = "true";
			try
			{
				SDLMTEdgeTranslatorHelper.VerifyBasicAPIToken(apiKeyTranslationOptions, credentials);
			}
			catch (Exception ex)
			{
				Assert.Fail("Expected no exception, but got: " + ex.Message);
			}
		}

		/// <summary>
		/// Tests GetLanguagePairs and verifies that the MTEdge instance the tests are running on has EngFra as a
		/// language pair
		/// </summary>
		[TestMethod]
		public void MTEdgeApi_FetchLPs_AtLeastFraEngReturned()
		{
			var lps = SDLMTEdgeTranslatorHelper.GetLanguagePairs(apiKeyTranslationOptions);
			if (!lps.Any())
			{
				Assert.Fail("Expected at least one LP returned");
			}

			// I realize not every MTEdge server will have german, but for sake of further tests, the host we choose to test with should.
			// Otherwise we'd have to code up an entire mock for the MTEdge API.
			if (!lps.Any(lp =>
				lp.SourceLanguageId.Equals("eng", StringComparison.OrdinalIgnoreCase)
				&& lp.TargetLanguageId.Equals("fra", StringComparison.OrdinalIgnoreCase)
			))
			{
				Assert.Fail("Expected EngFra as one of the LPs");
			}
		}

		/// <summary>
		/// Tests we are able to retrieve a non-null authentication token from the server using the user credentials
		/// </summary>
		[TestMethod]
		public void MTEdgeApi_GetBasicAuthToken_GetsToken()
		{
			try
			{
				var token = SDLMTEdgeTranslatorHelper.GetAuthToken(basicAuthTranslationOptions, userCredentials);
				if (token == null)
				{
					Assert.Fail("Expected token, but got null");
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Expected no exception, but got: " + ex.Message);
			}
		}

		/// <summary>
		/// Verifies that MTEdgeHelper is able to translate a basic string using both api key and user credentials
		/// </summary>
		[TestMethod]
		public void MTEdgeApi_FetchTranslationNoTags_ValidTranslation()
		{
			var file = new File
			{
				SourceCulture = engFraLP.SourceCulture,
				TargetCulture = engFraLP.TargetCulture
			};

			var xliffDocument = new Xliff
			{
				File = file
			};

			xliffDocument.AddSourceText(UTStringResource.BasicText);

			var translatedXliffText = SDLMTEdgeTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engFraLP, xliffDocument);
			var translatedXliff = Converter.ParseXliffString(translatedXliffText);

			Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
			Assert.AreEqual(UTStringResource.BasicTranslation, translatedXliff.GetTargetSegments()[0].ToString());

			var token = SDLMTEdgeTranslatorHelper.GetAuthToken(basicAuthTranslationOptions, userCredentials);
			basicAuthTranslationOptions.ApiToken = token;

			translatedXliffText = SDLMTEdgeTranslatorHelper.GetTranslation(basicAuthTranslationOptions, engFraLP, xliffDocument);
			translatedXliff = Converter.ParseXliffString(translatedXliffText);

			Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
			Assert.AreEqual(UTStringResource.BasicTranslation, translatedXliff.GetTargetSegments()[0].ToString());
		}

		/// <summary>
		/// Tests that abnormal characters (such as emojis) are able to be translated correctly by MTEdge
		/// </summary>
		[TestMethod]
		public void MTEdgeApi_FetchTranslationEmoji_ValidTranslation()
		{
			var file = new File
			{
				SourceCulture = engFraLP.SourceCulture,
				TargetCulture = engFraLP.TargetCulture
			};

			var xliffDocument = new Xliff
			{
				File = file
			};
			xliffDocument.AddSourceText(UTStringResource.BasicEmojiTest);
			xliffDocument.Version = "v1.0";
			var translatedXliffText = SDLMTEdgeTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engFraLP, xliffDocument);
			var translatedXliff = Converter.ParseXliffString(translatedXliffText);
			Assert.IsTrue(translatedXliff.GetTargetSegments().Any());
			Assert.AreEqual(UTStringResource.BasicEmojiTranslation, translatedXliff.GetTargetSegments()[0].ToString());
		}

		/// <summary>
		/// Verifies that we throw an exception when MTEdge returns an error (in this case, caused by an invalid LP)
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void MTEdgeApi_FetchTranslation_InvalidTranslation()
		{
			var file = new File
			{
				SourceCulture = engFraLP.SourceCulture,
				TargetCulture = engFraLP.TargetCulture
			};

			var xliffDocument = new Xliff
			{
				File = file
			};
			xliffDocument.AddSourceText(UTStringResource.BasicText);
			SDLMTEdgeTranslatorHelper.GetTranslation(apiKeyTranslationOptions, engInvLP, xliffDocument);
		}

		/// <summary>
		/// Verifies that we're able to fetch the MTEdge code of culture codes
		/// </summary>
		[TestMethod]
		public void MTEdgeCode_GetMtEdgeCode_ValidCode()
		{
			Assert.AreEqual("eng", engFraLP.SourceCulture.ToMTEdgeCode());
			Assert.AreEqual("fra", engFraLP.TargetCulture.ToMTEdgeCode());
			Assert.AreEqual("chi", CultureInfo.GetCultureInfo(0x0004).ToMTEdgeCode());
			Assert.AreEqual("cht", CultureInfo.GetCultureInfo(0x7C04).ToMTEdgeCode());
			Assert.AreEqual("ptb", CultureInfo.GetCultureInfo(0x0416).ToMTEdgeCode());
			Assert.AreEqual("fad", CultureInfo.GetCultureInfo("prs-af").ToMTEdgeCode());
		}

		/// <summary>
		/// Tests that if we try to get an MTEdge code using an invalid culture code, it returns null
		/// </summary>
		[TestMethod]
		public void MTEdgeCode_GetMtEdgeCode_InvalidCode()
		{
			Assert.AreEqual(string.Empty, engInvLP.TargetCulture.ToMTEdgeCode());
		}

		/// <summary>
		/// Tests that we set the API version correctly depending on the host's api version
		/// </summary>
		public void MTEdgeApi_SetMtEdgeApiVersion_v8()
		{
			apiKeyTranslationOptions.ApiVersion = APIVersion.Unknown;
			SDLMTEdgeTranslatorHelper.SetMtEdgeApiVersion(apiKeyTranslationOptions);
			Assert.AreEqual(APIVersion.v2, apiKeyTranslationOptions.ApiVersion);
		}
	}
}