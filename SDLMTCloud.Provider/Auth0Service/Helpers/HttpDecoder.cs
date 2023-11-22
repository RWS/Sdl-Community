using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Auth0Service.Helpers
{
	public static class HttpDecoder
	{
		public static JObject ParseJson(string json)
		{
			var parameters = JObject.Parse(json);
			return parameters;
		}

		public static Dictionary<string, string> ParseQuery(string query)
		{
			var parameters = HttpUtility.ParseQueryString(query);
			return parameters.AllKeys.ToDictionary(k => k, k => parameters[k]);
		}
	}
}