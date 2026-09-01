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
        public void CurrentUsRegion_IsKept()
        {
            Assert.Equal(Constants.CloudUSUrl, CloudCredentialsViewModel.NormalizeRegion(Constants.CloudUSUrl));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("https://uat.api.languageweaver.com/")]
        [InlineData("not-a-region")]
        public void AnythingElse_FallsBackToEu(string persistedRegion)
        {
            // A region saved against another environment is treated as unset: it matches neither radio
            // button, so the selector would otherwise render blank.
            Assert.Equal(Constants.CloudEUUrl, CloudCredentialsViewModel.NormalizeRegion(persistedRegion));
        }
    }
}
