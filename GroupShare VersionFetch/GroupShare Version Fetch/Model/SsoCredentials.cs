using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Xml;

namespace Sdl.Community.GSVersionFetch.Model
{
    public class SsoData
    {
        public SsoData(string samlResponse, string authToken, bool isWindowsUser = false)
        {
            if (samlResponse != null && authToken != null && !isWindowsUser)
            {
                string saml;
                try
                {
                    saml = DecodeSaml(samlResponse);
                }
                catch (FormatException)
                {
                    saml = DecodeSaml(JObject.Parse(samlResponse)["SsoToken"].ToString());
                }

                ServiceUrl = GetServerUrl(saml);
                AuthToken = authToken;
            }
        }

        public string AuthToken { get; set; }

        public string ServiceUrl { get; set; }

        private static string DecodeSaml(string samlResponse)
        {
            var decodedSaml = Convert.FromBase64String(samlResponse);
            var splitSamlString = Encoding.ASCII.GetString(decodedSaml).Split('=', '&');
            var samlToken = Convert.FromBase64String(Uri.UnescapeDataString(splitSamlString[1]));
            var saml = Encoding.UTF8.GetString(samlToken);
            return saml;
        }

        private string GetServerUrl(string saml)
        {
            var xml = new XmlDocument();
            xml.LoadXml(saml);

            var destinationUrl = xml.DocumentElement?.GetAttribute("Destination");

            var index = -1;
            if (destinationUrl != null)
            {
                index = destinationUrl.LastIndexOf('/');
            }

            return index != -1 ? destinationUrl?.Substring(0, index + 1) : string.Empty;
        }
    }
}