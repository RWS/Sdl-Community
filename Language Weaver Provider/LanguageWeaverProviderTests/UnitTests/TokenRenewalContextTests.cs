using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LanguageWeaverProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// CRQ-46950: reading the provider options through Studio's controllers installs a
    /// WindowsFormsSynchronizationContext on the Send Feedback batch thread, and nothing ever pumps it. Token renewal
    /// resumed on that context, so once Send Feedback awaited the renewal the batch hung at 92 % for good.
    /// The renewal here is the Edge credentials sign-in against a closed loopback port: real asynchronous I/O that
    /// fails fast, with no external host.
    /// </summary>
    public class TokenRenewalContextTests
    {
        public static TheoryData<string> EntryPoints => new() { nameof(Service.ValidateTokenAsync), nameof(Service.ValidateAndUpdateTokenAsync) };

        [Theory]
        [MemberData(nameof(EntryPoints))]
        public async Task Renewal_OnThreadWhoseContextNeverRunsContinuations_Completes(string entryPoint)
        {
            var options = new TranslationOptions
            {
                PluginVersion = PluginVersion.LanguageWeaverEdge,
                AuthenticationType = AuthenticationType.EdgeCredentials,
                EdgeCredentials = new EdgeCredentials { Uri = new Uri($"http://127.0.0.1:{ClosedPort()}"), UserName = "user", Password = "password" },
                AccessToken = new AccessToken { TokenType = "Bearer", ExpiresAt = 1000 }
            };

            Task validation;
            var testContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new NeverRunsContext());
            try
            {
                validation = entryPoint == nameof(Service.ValidateTokenAsync)
                    ? Service.ValidateTokenAsync(options, false)
                    : Service.ValidateAndUpdateTokenAsync(options, null);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(testContext);
            }

            var finished = await Task.WhenAny(validation, Task.Delay(TimeSpan.FromSeconds(15)));

            Assert.Same(validation, finished);
        }

        private static int ClosedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        // Stands in for Studio's WindowsFormsSynchronizationContext on a thread with no message loop.
        private sealed class NeverRunsContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state) { }

            public override void Send(SendOrPostCallback d, object state) { }
        }
    }
}
