using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Net.Http;

namespace Sdl.Community.GSVersionFetch
{
    [ApplicationInitializer]
    public class AppInitializer : IApplicationInitializer
    {
        private static HttpClient _client;
        private static IStudioEventAggregator _eventAggregator;

        public static HttpClient Client
            => _client ??= new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        private static IStudioEventAggregator EventAggregator => _eventAggregator ??= SdlTradosStudio.Application
            .GetService<IStudioEventAggregator>();

        public static void PublishEvent<TEvent>(TEvent sampleEvent)
        {
            EventAggregator?.Publish(sampleEvent);
        }

        public static IDisposable Subscribe<T>(Action<T> action)
        {
            return EventAggregator?.GetEvent<T>().Subscribe(action);
        }

        public void Execute()
        {
            Log.Setup();
        }
    }
}