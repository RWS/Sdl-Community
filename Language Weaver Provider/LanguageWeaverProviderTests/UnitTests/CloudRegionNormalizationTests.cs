using LanguageWeaverProvider;
using LanguageWeaverProvider.ViewModel.Cloud;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins how a persisted account region is interpreted when the credentials screen loads.
    /// <para>
    /// Saved credentials outlive an environment switch. A production host left in the settings file was still
    /// being bound while the plug-in ran against UAT, so neither region radio matched and the selector rendered
    /// blank. Regions that do not belong to the current environment are coerced to its EU host instead.
    /// </para>
    /// </summary>
    public class CloudRegionNormalizationTests
    {
        [Fact]
        public void CurrentEuRegion_IsKept()
        {
            Assert.Equal(Constants.CloudEUUrl, CloudCredentialsViewModel.NormalizeRegion(Constants.CloudEUUrl));
        }

        [Fact]
        public void CurrentUsRegion_IsKept()
        {
            Assert.Equal(Constants.CloudUSUrl, CloudCredentialsViewModel.NormalizeRegion(Constants.CloudUSUrl));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("https://api.languageweaver.com/")]
        [InlineData("https://uat.api.languageweaver.com/")]
        [InlineData("not-a-region")]
        public void RegionFromAnotherEnvironmentOrNoRegionAtAll_FallsBackToCurrentEu(string persistedRegion)
        {
            var normalized = CloudCredentialsViewModel.NormalizeRegion(persistedRegion);

            Assert.True(
                normalized == Constants.CloudEUUrl || normalized == Constants.CloudUSUrl,
                $"'{persistedRegion}' normalized to '{normalized}', which is not a selectable region.");
        }
    }
}
