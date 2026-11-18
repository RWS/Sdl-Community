using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LanguageWeaverProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class ResourceListTimeoutTests
    {
        private const int PairCount = 5000;
        private const int ChunkCount = 105;
        private static readonly TimeSpan ChunkDelay = TimeSpan.FromSeconds(1);

        [Fact]
        public async Task GetResources_LanguagePairsSlowerThanDefaultHttpTimeout_Succeeds()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                var server = ServeSlowlyAsync(listener, BuildLanguagePairsJson());
                var token = new AccessToken
                {
                    Token = "token",
                    TokenType = "Bearer",
                    AccountId = "account",
                    BaseUri = new Uri($"http://127.0.0.1:{port}/")
                };

                var stopwatch = Stopwatch.StartNew();
                var pairs = await CloudService.GetResources<PairModel>(token, CloudResources.LanguagePairs);
                stopwatch.Stop();
                await server;

                Assert.True(stopwatch.Elapsed > TimeSpan.FromSeconds(100), $"Download took only {stopwatch.Elapsed}");
                Assert.Equal(PairCount, pairs.Count);
                Assert.Equal("pair-4999", pairs.Last().Name);
            }
            finally
            {
                listener.Stop();
            }
        }

        private static byte[] BuildLanguagePairsJson()
        {
            var pairs = Enumerable.Range(0, PairCount).Select(i =>
                $"{{\"name\":\"pair-{i}\",\"displayName\":\"Pair {i}\",\"sourceLanguageId\":\"eng\",\"targetLanguageId\":\"jpn\",\"model\":\"generic\",\"active\":\"true\"}}");
            return Encoding.UTF8.GetBytes($"{{\"languagePairs\":[{string.Join(",", pairs)}]}}");
        }

        private static async Task ServeSlowlyAsync(TcpListener listener, byte[] body)
        {
            using var client = await listener.AcceptTcpClientAsync();
            using var stream = client.GetStream();

            var request = new StringBuilder();
            var buffer = new byte[4096];
            while (!request.ToString().Contains("\r\n\r\n"))
            {
                var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0) return;
                request.Append(Encoding.ASCII.GetString(buffer, 0, read));
            }

            var headers = Encoding.ASCII.GetBytes(
                $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(headers, 0, headers.Length);

            var chunkSize = (body.Length + ChunkCount - 1) / ChunkCount;
            for (var offset = 0; offset < body.Length; offset += chunkSize)
            {
                await stream.WriteAsync(body, offset, Math.Min(chunkSize, body.Length - offset));
                await stream.FlushAsync();
                await Task.Delay(ChunkDelay);
            }
        }
    }
}
