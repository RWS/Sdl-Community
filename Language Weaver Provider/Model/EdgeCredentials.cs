using LanguageWeaverProvider.Model.Interface;
using System;

namespace LanguageWeaverProvider.Model
{
	public class EdgeCredentials
    {
        public EdgeCredentials(){}
		public EdgeCredentials(string uri)
		{
			Host = uri;
			Uri = new Uri(uri);
			Scheme = Uri.Scheme;
			Port = Uri.Port.ToString();
		}

		public Uri Uri { get; set; }

		public string Scheme { get; set; }

		public string Host { get; set; }

		public string Port { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string ApiKey { get; set; }
    }
}