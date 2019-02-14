using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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
		private readonly string _model;
		private readonly NormalizeSourceTextHelper _normalizeTextHelper;

		public BeGlobalConnecter(string clientId,string clientSecret, bool useClientAuthentication, string model)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			UseClientAuthentication = useClientAuthentication;
			_model = model;
			_normalizeTextHelper = new NormalizeSourceTextHelper();	
		}

		public string Translate(LanguagePair languageDirection, string sourceText)
		{ 
			var targetLanguage = _normalizeTextHelper.GetCorespondingLangCode(languageDirection.TargetCulture);
			var sourceLanguage = _normalizeTextHelper.GetCorespondingLangCode(languageDirection.SourceCulture);
																														  
			sourceText = _normalizeTextHelper.NormalizeText(sourceText);	

			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", ClientId, ClientSecret,
				sourceLanguage, targetLanguage, _model, UseClientAuthentication);

			var translatedText = HttpUtility.UrlDecode(beGlobalTranslator.TranslateText(sourceText)); 
			translatedText = HttpUtility.HtmlDecode(translatedText);	 

			return translatedText;
		}

	}
}
