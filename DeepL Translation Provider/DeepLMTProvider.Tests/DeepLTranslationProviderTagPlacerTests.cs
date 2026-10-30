using System.Linq;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLTranslationProviderTagPlacerTests
    {
        [Theory]
        [InlineData(TagFormat.XML)]
        [InlineData(TagFormat.HTML)]
        public void PreparedSourceText_XmlOrHtml_EscapesSpecialCharactersInText(TagFormat tagHandling)
        {
            // Arrange: Studio's text holds plain characters; DeepL reads XML/HTML input as markup
            var tagPlacer = new DeepLTranslationProviderTagPlacer(
                TaggedSource("Geben Sie ", "<Kurzname> \"Grüße\"", " für A&B ein, Wert > 1."), tagHandling);

            // Act
            var prepared = tagPlacer.PreparedSourceText;

            // Assert: only &, < and > are escaped; tags, quotes and umlauts stay as they are
            Assert.Equal("Geben Sie <tg1>&lt;Kurzname&gt; \"Grüße\"</tg1> für A&amp;B ein, Wert &gt; 1.", prepared);
        }

        [Fact]
        public void PreparedSourceText_None_LeavesTextRaw()
        {
            // Arrange: without tag handling DeepL reads the whole string as plain text
            var tagPlacer = new DeepLTranslationProviderTagPlacer(
                TaggedSource("Geben Sie ", "<Kurzname>", " für A&B ein, Wert > 1."), TagFormat.None);

            // Act
            var prepared = tagPlacer.PreparedSourceText;

            // Assert
            Assert.Equal("Geben Sie <tg1><Kurzname></tg1> für A&B ein, Wert > 1.", prepared);
        }

        [Theory]
        [InlineData(TagFormat.XML, // v1 also escapes quotes and apostrophes
            "Inserire <tg1>&lt;nome&gt;</tg1> qui: A&amp;B &gt; 1, &quot;it&apos;s&quot; &#169; &#x41;.",
            "Inserire <Start1><nome><End1> qui: A&B > 1, \"it's\" © A.")]
        [InlineData(TagFormat.XML, // v2 leaves them as characters
            "Inserire <tg1>&lt;nome&gt;</tg1> qui: \"it's\" A&amp;B.",
            "Inserire <Start1><nome><End1> qui: \"it's\" A&B.")]
        [InlineData(TagFormat.HTML,
            "Caff&egrave; <tg1>&lt;nome&gt;</tg1> e t&egrave;.",
            "Caffè <Start1><nome><End1> e tè.")]
        public void GetTaggedSegment_XmlOrHtml_DecodesEntitiesInText(TagFormat tagHandling, string reply, string expected)
        {
            // Arrange: DeepL writes its reply as markup, so &, < and > come back escaped
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Geben Sie ", "<Kurzname>", " ein."), tagHandling);

            // Act
            var target = tagPlacer.GetTaggedSegment(reply);

            // Assert: the escaped <nome> is text, not a third tag
            Assert.Equal(expected, Render(target));
            Assert.Equal(2, target.Elements.OfType<Tag>().Count());
        }

        [Fact]
        public void GetTaggedSegment_None_LeavesReplyTextAsIs()
        {
            // Arrange: without tag handling DeepL replies in plain text, so &amp; is literal text
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Schreiben Sie ", "&amp;", " für &."), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment("Scrivere <tg1>&amp;</tg1> per &.");

            // Assert
            Assert.Equal("Scrivere <Start1>&amp;<End1> per &.", Render(target));
        }

        private static Segment TaggedSource(string before, string tagged, string after)
        {
            var source = new Segment(new CultureCode("de-DE"));
            source.Add(before);
            source.Add(new Tag(TagType.Start, "1", 1));
            source.Add(tagged);
            source.Add(new Tag(TagType.End, "1", 1));
            source.Add(after);
            return source;
        }

        private static string Render(Segment segment) =>
            string.Concat(segment.Elements.Select(e => e is Tag tag ? $"<{tag.Type}{tag.TagID}>" : e.ToString()));
    }
}
