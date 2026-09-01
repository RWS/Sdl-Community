using LanguageWeaverProvider;
using LanguageWeaverProvider.ViewModel.Cloud;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
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
            Assert.Equal(Constants.CloudEUUrl, CloudCredentialsViewModel.NormalizeRegion(persistedRegion));
        }
    }
}
