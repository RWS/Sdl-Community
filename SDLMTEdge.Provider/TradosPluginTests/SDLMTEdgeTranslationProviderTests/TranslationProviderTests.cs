using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.Provider;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeTranslationProviderTests
{
	[TestClass]
	public class TranslationProviderTests
	{
		public TranslationProviderTests()
		{
			apiKeyOptions = new TranslationOptions(new Uri(UTStringResource.ApiUrl));
		}

		TranslationOptions apiKeyOptions;

		/// <summary>
		/// Tests that the api key in the provider is the one specified in the options
		/// </summary>
		[TestMethod]
		public void Provider_ApiKey_UrisEqual()
		{
			var provider = new TranslationProvider(apiKeyOptions);
			Assert.AreEqual(provider.Uri, apiKeyOptions.Uri);
		}
	}
}