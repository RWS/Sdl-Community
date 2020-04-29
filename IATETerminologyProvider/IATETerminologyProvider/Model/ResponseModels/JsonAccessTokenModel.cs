using System.Collections.Generic;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class JsonAccessTokenModel
	{
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		public List<string> Tokens { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }
	}
}
