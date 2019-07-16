using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace IATETerminologyProvider.Helpers
{
	public static class Utils
	{
		public static string RemoveUriForbiddenCharacters(this string uriString)
		{
			var regex = new Regex(@"[$%+!*'(), ]");
			return regex.Replace(uriString, "");
		}

		public static string UppercaseFirstLetter(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}

		public static void AddDefaultParameters(HttpClient httpClient)
		{
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
			httpClient.DefaultRequestHeaders.Add("Origin", "https://iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
			httpClient.DefaultRequestHeaders.Add("Host", "iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
		}
	}
}