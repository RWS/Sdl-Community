using System;
using ETSTranslationProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradosPluginTests
{
	[TestClass]
	public class TranslationProviderTests
	{
		public TranslationProviderTests()
		{
			apiKeyOptions = new TranslationOptions(new Uri(StringResource.ApiUrl));
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