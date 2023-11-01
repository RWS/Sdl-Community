using System;

namespace LanguageWeaverProvider.Model
{
	public class EdgeCredentials
    {
		public Uri Uri { get; set; }

		public string Scheme { get; set; }

		public string Host { get; set; }

		public string Port { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string ApiKey { get; set; }

		public void SetUri(string uriString)
		{
			if (uriString is null)
			{
				return;
			}

			Host = uriString;
			Uri = new Uri(uriString);
			Scheme = Uri.Scheme;
			Port = Uri.Port.ToString();
		}

		public void SetUri(Uri uri)
		{
			SetUri(uri.ToString());
		}
    }
}