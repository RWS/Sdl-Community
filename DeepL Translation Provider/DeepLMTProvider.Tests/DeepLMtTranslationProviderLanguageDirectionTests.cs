using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Studio;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLMtTranslationProviderLanguageDirectionTests
    {
        [Fact]
        public void BuildContextUnits_ReturnsPlainSourceOfEveryUnitInOrder()
        {
            // Arrange
            var tagged = new Segment(new CultureCode("en-US"));
            tagged.Add("Hello ");
            tagged.Add(new Tag(TagType.Start, "1", 1));
            tagged.Add("world");
            tagged.Add(new Tag(TagType.End, "1", 1));

            var plain = new Segment(new CultureCode("en-US"));
            plain.Add("Bye");

            var translationUnits = new[]
            {
                new TranslationUnit { SourceSegment = tagged },
                new TranslationUnit(),
                new TranslationUnit { SourceSegment = plain }
            };

            // Act
            var contextUnits = DeepLMtTranslationProviderLanguageDirection.BuildContextUnits(translationUnits);

            // Assert
            Assert.Equal(new[] { "Hello world", "", "Bye" }, contextUnits);
        }

        [Theory]
        [InlineData(false, false, false, null)]
        [InlineData(true, false, true, null)]
        [InlineData(false, true, false, "Maria is a woman.")]
        [InlineData(true, true, true, "Maria is a woman.")]
        public void SelectContext_SendsTheChosenSources(bool sendSurroundingSegments, bool sendCustomContext, bool expectUnits, string expectedCustomContext)
        {
            // Arrange: a custom context is stored whether or not it is sent
            var plain = new Segment(new CultureCode("en-US"));
            plain.Add("Bye");
            var translationUnits = new[] { new TranslationUnit { SourceSegment = plain } };

            // Act
            var (contextUnits, customContext) = DeepLMtTranslationProviderLanguageDirection.SelectContext(
                sendSurroundingSegments, sendCustomContext, "Maria is a woman.", translationUnits, TagFormat.None);

            // Assert
            Assert.Equal(expectUnits, contextUnits != null);
            Assert.Equal(expectedCustomContext, customContext);
        }

        [Theory]
        [InlineData(TagFormat.XML, "Datei &gt; Speichern &amp; Schließen", "Wert &lt; 1")]
        [InlineData(TagFormat.HTML, "Datei &gt; Speichern &amp; Schließen", "Wert &lt; 1")]
        [InlineData(TagFormat.None, "Datei > Speichern & Schließen", "Wert < 1")]
        public void SelectContext_EscapesContextForXmlAndHtml(TagFormat tagHandling, string expectedUnit, string expectedCustomContext)
        {
            // Arrange: with tag handling DeepL parses the context as markup too
            var plain = new Segment(new CultureCode("de-DE"));
            plain.Add("Datei > Speichern & Schließen");
            var translationUnits = new[] { new TranslationUnit { SourceSegment = plain } };

            // Act
            var (contextUnits, customContext) = DeepLMtTranslationProviderLanguageDirection.SelectContext(
                true, true, "Wert < 1", translationUnits, tagHandling);

            // Assert
            Assert.Equal(new[] { expectedUnit }, contextUnits);
            Assert.Equal(expectedCustomContext, customContext);
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
    }
}
