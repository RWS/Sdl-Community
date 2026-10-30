using System;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class OptionsProviderTests
    {
        [Fact]
        public void GetTranslationOptions_StateSavedBeforeTagHandlingVersion_LoadsV2()
        {
            // Arrange: provider state as saved by 8.1.1.0, without TagHandlingVersion
            const string state = "{\"TagHandling\":1,\"ResendDraft\":true}";

            // Act
            var options = new OptionsProvider().GetTranslationOptions(new Uri("deepltranslationprovider:///"), state);

            // Assert
            Assert.Equal(TagFormat.XML, options.TagHandling);
            Assert.Equal(TagHandlingVersion.V2, options.TagHandlingVersion);
        }
    }
}
