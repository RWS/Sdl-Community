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
            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void WithNoFile_OneIsWrittenSoItCanBeFoundAndEdited()
        {
            CreateService().LoadCreatingDefaultsIfMissing();

            Assert.True(_storage.Exists(_pathInfo.DeveloperSettingsPath));
        }

        [Fact]
        public void AskingForUat_SwitchesTheEnvironment()
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(CloudEnvironment.Uat.Name, settings.Environment);
        }

        [Theory]
        [InlineData("uat")]
        [InlineData("Uat")]
        public void TheEnvironmentName_IsNotCaseSensitive(string name)
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = name });

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(CloudEnvironment.Uat.Name, settings.Environment);
        }

        [Theory]
        [InlineData("Preprod")]
        [InlineData("")]
        [InlineData(null)]
        public void AnUnrecognisedEnvironment_FallsBackToProduction(string name)
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = name });

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
        }

        [Fact]
        public void AnUnreadableFile_DoesNotStopStudioLoading()
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });
            _storage.ThrowOnLoad = true;

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(CloudEnvironment.Production.Name, settings.Environment);
            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void AThresholdOfZero_IsHonouredSoTheFinalDayAloneCanBeTested()
        {
            _storage.Save(
                _pathInfo.DeveloperSettingsPath,
                new DeveloperSettings { TrialPromptFromDaysRemaining = 0 });

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(0, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void AnOmittedThreshold_ResolvesToTheShippedDefault()
        {
            _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

            var settings = CreateService().LoadCreatingDefaultsIfMissing();

            Assert.Equal(7, settings.TrialPromptFromDaysRemaining);
        }

        [Fact]
        public void Apply_SetsTheEnvironmentTheRestOfThePluginReads()
        {
            var original = CloudEnvironment.Current;
            try
            {
                _storage.Save(_pathInfo.DeveloperSettingsPath, new DeveloperSettings { Environment = "UAT" });

                CreateService().ApplyToTheCurrentEnvironment();

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
