using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLWindowViewModelTests
    {
        [Theory]
        [InlineData(true, TagFormat.XML, false, false)]
        [InlineData(true, TagFormat.None, false, false)]
        [InlineData(false, TagFormat.XML, true, true)]
        [InlineData(false, TagFormat.HTML, true, true)]
        [InlineData(false, TagFormat.None, true, false)]
        public void TagHandlingControlsEnabled_PlainTextDisablesTagHandlingAndVersion(
            bool sendPlainText, TagFormat tagType, bool expectTagHandling, bool expectVersion)
        {
            // Act: plain text is sent without tag handling, so neither setting applies
            var (tagHandling, version) = DeepLWindowViewModel.TagHandlingControlsEnabled(sendPlainText, tagType);

            // Assert
            Assert.Equal(expectTagHandling, tagHandling);
            Assert.Equal(expectVersion, version);
        }
    }
}
