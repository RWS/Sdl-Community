namespace IATETerminologyProvider.Helpers
{
	public class ApiUrls
	{
		public static string GetAccessTokenUri(string userName, string password)
		{
			return @"https://iate.europa.eu/uac-api/auth/login?username=" + userName + "&password=" + password;
		}

		public static string GetExtendAccessTokenUri(string token)
		{
			return @"https://iate.europa.eu/uac-api/auth/token?refresh_token=" + token;
		}

		public static string BaseUri(string expand, string offset, string limit)
		{
			return @"https://iate.europa.eu/em-api/entries/_search?expand=" + expand + "&offset=" + offset + "&limit=" + limit;
		}

		public static string GetDomainUri()
		{
			return @"https://iate.europa.eu/em-api/domains/_tree";
		}

		public static string GetTermTypeUri(string expand, string trans_lang, string limit, string offset)
		{
			return @"https://iate.europa.eu//em-api/inventories/_term-types?expand=" + expand + "&trans_lang=" + trans_lang + "&limit=" + limit + "&offset=" + offset;
		}

		public static string SearchAllURI(string currentSelection, string sourceLanguage)
		{
			return @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=all";
		}

		public static string SearchSourceTargetURI(string currentSelection, string sourceLanguage, string targetLanguage)
		{
			return @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=" + targetLanguage;
		}
	}
}