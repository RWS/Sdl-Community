using System.Net;
using System.Net.Http;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
	public static class ExceptionExtensions
	{
		private const string HttpStatusCodeDataKey = "StatusCode";

		public static void SetHttpStatusCode(this HttpRequestException ex, HttpStatusCode statusCode)
		{
			ex.Data[HttpStatusCodeDataKey] = statusCode;
		}

		public static HttpStatusCode? GetHttpStatusCode(this HttpRequestException ex)
		{
			if (!ex.Data.Contains(HttpStatusCodeDataKey))
				return null;

			return (HttpStatusCode)ex.Data[HttpStatusCodeDataKey];
		}
	}
}
