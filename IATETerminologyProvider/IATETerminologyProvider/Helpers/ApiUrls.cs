using System.Web;

namespace Sdl.Community.IATETerminologyProvider.Helpers
{
	public class ApiUrls
	{
		private const string AuthBaseUrl = "https://iate.europa.eu/uac-api/auth";
		private const string UacBaseUrl = "https://iate.europa.eu/uac-api";
		private const string BaseApiUrl = "https://iate.europa.eu/em-api";
		private const string BaseUrl = "https://iate.europa.eu";

		public static string GetAccessTokenUri(string userName, string password)
		{
			return $"{AuthBaseUrl}/login?username={userName}&password={password}";
		}

		public static string GetCollectionsUri()
		{
			return $"{BaseApiUrl}/collections?expand=true&offset=0&limit=3000";
		}

		public static string GetDomainUri()
		{
			return $"{BaseApiUrl}/domains/_tree";
		}

		public static string GetExtendAccessTokenUri(string token)
		{
			return $"{AuthBaseUrl}/token?refresh_token={token}";
		}

		public static string GetTermTypeUri(string expand, string transLang, string limit, string offset)
		{
			return $"{BaseApiUrl}/inventories/_term-types?expand={expand}&trans_lang={transLang}&limit={limit}&offset={offset}";
		}

		public static string SearchAllUri(string currentSelection, string sourceLanguage)
		{
			var encodedString = HttpUtility.UrlEncode(currentSelection);
			var uri = $"{BaseUrl}/search/byUrl?term={encodedString}&sl={sourceLanguage}&tl=all";
			return uri;
		}

		public static string SearchSourceTargetUri(string currentSelection, string sourceLanguage, string targetLanguage)
		{
			var encodedString = HttpUtility.UrlEncode(currentSelection);
			var uri = $"{BaseUrl}/search/byUrl?term={encodedString}&sl={sourceLanguage}&tl={targetLanguage}";
			return uri;
		}

		public static string SearchUri(string expand, int limit)
		{
			return $"{BaseApiUrl}/entries/_search?expand={expand}&offset=0&limit={limit}";
		}

		public static string GetInstitutionsUri()
		{
			return $"{UacBaseUrl}/institutions?expand=true&offset=0&limit=3000";
		}
	}
}