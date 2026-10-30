using Sdl.Community.DeepLMTProvider.ViewModel;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLWindowViewModelTests
    {
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TagHandlingEnabled_PlainTextDisablesTagHandling(bool sendPlainText, bool expected)
        {
            // Act: plain text is sent without tag handling, so the setting does not apply
            var enabled = DeepLWindowViewModel.TagHandlingEnabled(sendPlainText);

            // Assert
            Assert.Equal(expected, enabled);
        }
    }
}
