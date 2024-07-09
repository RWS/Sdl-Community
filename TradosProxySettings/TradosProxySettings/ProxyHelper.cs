using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TradosProxySettings.Model;

namespace TradosProxySettings
{
    public class ProxyHelper
    {
        public static HttpClientHandler GetHttpClientHandler(ProxySettings proxySettings,
            bool acceptAllCertificates = false, HttpClientHandler httpClientHandler = null)
        {
            var httpHandler = httpClientHandler ?? new HttpClientHandler();

            if (proxySettings.IsEnabled)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri($"{proxySettings.Address}:{proxySettings.Port}"),
                    BypassProxyOnLocal = proxySettings.BypassProxyOnLocal,
                    UseDefaultCredentials = proxySettings.UseDefaultCredentials
                };

                if (!proxySettings.UseDefaultCredentials &&
                    !string.IsNullOrEmpty(proxySettings.Username) &&
                    !string.IsNullOrEmpty(proxySettings.Password))
                {
                    proxy.Credentials = string.IsNullOrEmpty(proxySettings.Domain)
                        ? new NetworkCredential(proxySettings.Username, proxySettings.Password)
                        : new NetworkCredential(proxySettings.Username, proxySettings.Password, proxySettings.Domain);
                }

                httpHandler.Proxy = proxy;
                httpHandler.UseProxy = true;
            }
            else
            {
                httpHandler.UseProxy = false;
            }

            if (acceptAllCertificates)
            {
                httpHandler.ServerCertificateCustomValidationCallback = (HttpRequestMessage request,
                    X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
            }

            return httpHandler;
        }
    }
}
