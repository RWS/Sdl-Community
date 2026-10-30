using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Studio;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLMtTranslationProviderLanguageDirectionTests
    {
        [Theory]
        [InlineData(true, TagFormat.XML, TagFormat.None)]
        [InlineData(true, TagFormat.HTML, TagFormat.None)]
        [InlineData(true, TagFormat.None, TagFormat.None)]
        [InlineData(false, TagFormat.XML, TagFormat.XML)]
        [InlineData(false, TagFormat.HTML, TagFormat.HTML)]
        [InlineData(false, TagFormat.None, TagFormat.None)]
        public void EffectiveTagHandling_PlainTextSendsNoTagHandling(bool sendPlainText, TagFormat setting, TagFormat expected)
        {
            // Act: plain text carries no tags, so it must not be parsed as markup
            var tagHandling = DeepLMtTranslationProviderLanguageDirection.EffectiveTagHandling(sendPlainText, setting);

            // Assert
            Assert.Equal(expected, tagHandling);
        }

        [Theory]
        [InlineData(TagFormat.XML)]
        [InlineData(TagFormat.HTML)]
        public void BuildTarget_UntaggedSourceXmlOrHtml_DecodesReply(TagFormat tagHandling)
        {
            // Arrange: a segment without tags; DeepL still writes its reply as markup
            var source = new Segment(new CultureCode("de-DE"));
            source.Add("Firma A&B, Wert > 1.");

            // Act
            var target = DeepLMtTranslationProviderLanguageDirection.BuildTarget(
                source, "Azienda A&amp;B, valore &gt; 1.", tagHandling, false, new CultureCode("it-IT"));

            // Assert
            Assert.Equal("Azienda A&B, valore > 1.", target.ToPlain());
        }

        [Theory]
        [InlineData(false)] // tag handling None
        [InlineData(true)]  // Send plain text: the effective tag handling is None
        public void BuildTarget_WithoutTagHandling_LeavesReplyAsIs(bool sendPlainText)
        {
            // Arrange: without tag handling DeepL replies in plain text, so &amp; is literal text
            var source = new Segment(new CultureCode("de-DE"));
            source.Add("Schreiben Sie &amp; für &.");

            // Act
            var target = DeepLMtTranslationProviderLanguageDirection.BuildTarget(
                source, "Scrivere &amp; per &.", TagFormat.None, sendPlainText, new CultureCode("it-IT"));

            // Assert
            Assert.Equal("Scrivere &amp; per &.", target.ToPlain());
        }
    }
}
