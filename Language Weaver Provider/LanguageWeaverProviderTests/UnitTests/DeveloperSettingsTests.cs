using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Model;
using LanguageWeaverProvider.CohereSubscription.Settings.Services;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using System;
using System.Collections.Generic;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// The developer settings file exists to make the Cohere journey testable by hand, so its own failure
    /// modes have to be harmless: an installation that never touches the file must behave exactly as the
    /// shipped product, and a hand-edited file with a typo in it must not point a real user at UAT or
    /// silence the trial prompt.
    /// </summary>
    public class DeveloperSettingsTests
    {
        private readonly FakePathInfo _pathInfo = new FakePathInfo();
        private readonly FakeStorageService _storage = new FakeStorageService();

        private DeveloperSettingsService CreateService()
        {
            return new DeveloperSettingsService(_pathInfo, _storage);
        }

        [Fact]
        public void WithNoFile_TheDefaultsAreProductionAndTheFinalWeek()
        {
            var settings = CreateService().Load();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void WithNoFile_OneIsWrittenSoItCanBeFoundAndEdited()
        {
            CreateService().Load();

            Assert.True(_storage.Exists(_pathInfo.DeveloperSettingsPath));
        }

        [Fact]
        public void AskingForUat_SwitchesTheEnvironment()
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

            var settings = CreateService().Load();

            Assert.Equal(CloudEnvironment.Uat.Name, settings.Environment);
        }

        [Theory]
        [InlineData("uat")]
        [InlineData("Uat")]
        public void TheEnvironmentName_IsNotCaseSensitive(string name)
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = name });

            var settings = CreateService().Load();

            Assert.Equal(CloudEnvironment.Uat.Name, settings.Environment);
        }

        [Theory]
        [InlineData("Preprod")]
        [InlineData("")]
        [InlineData(null)]
        public void AnUnrecognisedEnvironment_FallsBackToProduction(string name)
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = name });

            var settings = CreateService().Load();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
        }

        [Fact]
        public void AnUnreadableFile_DoesNotStopStudioLoading()
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });
            _storage.ThrowOnLoad = true;

            var settings = CreateService().Load();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void AThresholdOfZero_IsHonouredSoTheFinalDayAloneCanBeTested()
        {
            // Zero is a legitimate setting - it silences everything but the last day - so it must not be
            // mistaken for "unset" and replaced by the default.
            _storage.Save(
                _pathInfo.DeveloperSettingsPath,
                new DeveloperSettings { TrialPromptFromDaysRemaining = 0 });

            var settings = CreateService().Load();

            Assert.Equal(0, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void AnOmittedThreshold_ResolvesToTheShippedDefault()
        {
            // Load always returns a usable value, so no caller has to repeat the fallback.
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

            var settings = CreateService().Load();

            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void Apply_SetsTheEnvironmentTheRestOfThePluginReads()
        {
            var original = CloudEnvironment.Current;
            try
            {
                _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

                CreateService().Apply();

                Assert.Same(CloudEnvironment.Uat, CloudEnvironment.Current);
            }
            finally
            {
                CloudEnvironment.Current = original;
            }
        }

        private class FakePathInfo : IPathInfo
        {
            public string SettingsPath => "settings.json";

            public string DeveloperSettingsPath => "developer.json";
        }

        private class FakeStorageService : IStorageService
        {
            private readonly Dictionary<string, object> _files = new Dictionary<string, object>();

            public bool ThrowOnLoad { get; set; }

            public bool Exists(string filePath) => _files.ContainsKey(filePath);

            public T Load<T>(string filePath) where T : class
            {
                if (ThrowOnLoad)
                    throw new InvalidOperationException("The file could not be read.");

                return _files.TryGetValue(filePath, out var value) ? value as T : null;
            }

            public void Save<T>(string filePath, T data)
            {
                _files[filePath] = data;
            }
        }
    }
}
