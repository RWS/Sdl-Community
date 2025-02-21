using System;
using System.Text;

namespace Sdl.Community.IATETerminologyProvider.Helpers
{
	public class JwtHelper
	{
		public static DateTime? GetTokenExpiryDate(string token)
		{
			var tokenParts = token.Split('.');
			if (tokenParts.Length != 3)
			{
				throw new ArgumentException("Invalid JWT token format");
			}

			var payload = tokenParts[1];
			var decodedPayloadBytes = Convert.FromBase64String(payload);
			var decodedPayload = Encoding.UTF8.GetString(decodedPayloadBytes);

			var payloadJson = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(decodedPayload);
			var expiryDateUnix = (long)payloadJson.exp;

			var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).DateTime;
			return expiryDate;
		}
	}
}