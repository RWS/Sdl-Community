using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTEdge.Provider.Interface
{
	public interface ITranslationOptions
	{
		public Uri Uri { get; }

		public int Port { get; set; }

		public string Host { get; set; }

		public string ApiToken { get; set; }

		public APIVersion ApiVersion { get; set; }

		public string ApiVersionString { get; }

		public bool PersistCredentials { get; set; }

		public bool UseBasicAuthentication { get; set; }

		public bool UseApiKey { get; set; }

		public bool UseAuth0SSO { get; set; }

		public bool RequiresSecureProtocol { get; set; }

		public Dictionary<CultureInfo, MTEdgeLanguagePair> LanguagePairPreferences { get; set; }

		public List<TradosToMTEdgeLanguagePair> LanguageMapping { get; set; }

		public void SetDictionaries(TradosToMTEdgeLanguagePair[] languagePairChoices);

		public TradosToMTEdgeLanguagePair[] SetPreferredLanguages(LanguagePair[] languagePairs);

		public void SetLanguageMapping(List<TradosToMTEdgeLanguagePair> languagePairChoices);
	}
}