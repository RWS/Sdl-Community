using System.Net;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalConnecter
	{  
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public bool UseClientAuthentication { get; set; }
		private readonly NormalizeSourceTextHelper _normalizeTextHelper;
		private readonly BeGlobalV4Translator _beGlobalTranslator;

		public BeGlobalConnecter(string clientId, string clientSecret, bool useClientAuthentication, string model, LanguagePair languageDirection)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			UseClientAuthentication = useClientAuthentication;
			_normalizeTextHelper = new NormalizeSourceTextHelper();

			var targetLanguage = _normalizeTextHelper.GetCorespondingLangCode(languageDirection.TargetCulture);
			var sourceLanguage = _normalizeTextHelper.GetCorespondingLangCode(languageDirection.SourceCulture);

			_beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", ClientId, ClientSecret,
				sourceLanguage, targetLanguage, model, UseClientAuthentication);
		}

		public string Translate(string sourceText)
		{ 
			sourceText = _normalizeTextHelper.NormalizeText(sourceText);	
			var translatedText = WebUtility.UrlDecode(_beGlobalTranslator.TranslateText(sourceText)); 
			translatedText = WebUtility.HtmlDecode(translatedText);	 
			return translatedText;
		}

	}
}
