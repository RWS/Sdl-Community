using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TradosProxySettings;
using TradosProxySettings.Model;

namespace TradosProxySettingConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ipNoProxy = await GetIPNoProxy();
            var ipProxy = await GetIPFromProxy();

            Console.ReadLine();
        }

        private static async Task<string> GetIPNoProxy()
        {
            var client = new HttpClient();

            try
            {
                // perform an async GET request to HttpBin
                using HttpResponseMessage response = await client.GetAsync("https://httpbin.org/ip");

                // extract the request response and print it
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);

                return responseContent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request failed with error: ", e.Message);
            }

            return null;
        }

        private static async Task<string> GetIPFromProxy()
        {
            var proxySettings = new ProxySettings
            {
                IsEnabled = true,
                Address = "http://localhost",
                Port = 8080
            };

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(proxySettings, true);
            var client = new HttpClient(httpClientHandler);

            try
            {
                using var response = await client.GetAsync("https://httpbin.org/ip");
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseContent);
                return responseContent;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request failed with error: ", e.Message);
            }

            return null;
        }
    }
}
