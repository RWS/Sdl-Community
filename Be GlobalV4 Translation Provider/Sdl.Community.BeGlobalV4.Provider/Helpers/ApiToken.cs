using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class ApiToken
	{
		private static string _token;
		public static string GetToken(IRestRequest request, IRestClient client)
		{
			if (string.IsNullOrEmpty(_token))
			{
				var response = client.Execute(request);
				if (response.StatusCode != System.Net.HttpStatusCode.OK)
					throw new Exception("Acquiring token failed: " + response.Content);
				dynamic json = JsonConvert.DeserializeObject(response.Content);
				_token = json.accessToken;
			}
			return _token;
		}
	}
}
