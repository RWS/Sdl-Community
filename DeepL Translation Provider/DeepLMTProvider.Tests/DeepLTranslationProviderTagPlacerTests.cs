using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Xunit;

namespace DeepLMTProvider.Tests
{
    public class DeepLTranslationProviderTagPlacerTests
    {
        [Fact]
        public void GetTaggedSegment_AddsNoSourceSpaceAfterElidedArticle()
        {
            // Arrange: the source has a space before the tag, Italian elides "l'" onto the tagged word
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Wählen Sie die ", "Option", " aus."), TagFormat.XML);

            // Act
            var target = tagPlacer.GetTaggedSegment("Selezionare l'<tg1>opzione</tg1>.");

            // Assert
            Assert.Equal("Selezionare l'<Start1>opzione<End1>.", Render(target));
        }

        [Fact]
        public void GetTaggedSegment_KeepsSpaceDeepLAddsBesideTag()
        {
            // Arrange: the source glues the tag to the previous word, DeepL separates them
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Klicken Sie auf", "OK", "."), TagFormat.XML);

            // Act
            var target = tagPlacer.GetTaggedSegment("Cliquez sur <tg1>OK</tg1>.");

            // Assert
            Assert.Equal("Cliquez sur <Start1>OK<End1>.", Render(target));
        }

        [Fact]
        public void GetTaggedSegment_KeepsSpaceBetweenTags()
        {
            // Arrange: DeepL returns one source pair as two pairs separated by a space
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("an der ", "Dienstart", " die"), TagFormat.XML);

            // Act
            var target = tagPlacer.GetTaggedSegment("nella <tg1>categoria</tg1> <tg1>di</tg1> guida");

            // Assert
            Assert.Equal("nella <Start1>categoria<End1> <Start1>di<End1> guida", Render(target));
        }

        [Theory]
        [InlineData("Con <TG1>Riassunto</TG1> si avvia")]
        [InlineData("Con <TG1>Riassunto</tg1> si avvia")]
        [InlineData("Con <tg1>Riassunto</TG1> si avvia")]
        public void GetTaggedSegment_MapsTagsRegardlessOfCase(string returnedText)
        {
            // Arrange: with tag handling None and a glossary, DeepL can return the tags in upper or mixed case
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Mit ", "Zusammenfassung", " startet"), TagFormat.XML);

            // Act
            var target = tagPlacer.GetTaggedSegment(returnedText);

            // Assert
            Assert.Equal("Con <Start1>Riassunto<End1> si avvia", Render(target));
        }

        [Fact]
        public void GetTaggedSegment_MapsUppercaseStandaloneTag()
        {
            // Arrange
            var source = new Segment(new CultureCode("de-DE"));
            source.Add("Seite ");
            source.Add(new Tag(TagType.Standalone, "1", 1));
            source.Add(" öffnen");
            var tagPlacer = new DeepLTranslationProviderTagPlacer(source, TagFormat.XML);

            // Act
            var target = tagPlacer.GetTaggedSegment("Aprire la pagina <TG1/>");

            // Assert
            Assert.Equal("Aprire la pagina <Standalone1>", Render(target));
        }

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
            "Inserire <tg1>&lt;nome abbreviato&gt;</tg1>: Azienda A&amp;B, valore &gt; 1, &quot;it&apos;s&quot; &#169; &#x41;.",
            "Inserire <Start1><nome abbreviato><End1>: Azienda A&B, valore > 1, \"it's\" © A.")]
        [InlineData(TagFormat.XML, // v2 leaves them as characters
            "Inserire <tg1>&lt;nome&gt;</tg1>: \"it's\" A&amp;B.",
            "Inserire <Start1><nome><End1>: \"it's\" A&B.")]
        [InlineData(TagFormat.HTML,
            "Caff&egrave; <tg1>&lt;nome&gt;</tg1> &amp; t&egrave;.",
            "Caffè <Start1><nome><End1> & tè.")]
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

        [Fact]
        public void GetTaggedSegment_None_DropsOpeningTagWithoutClosingTag()
        {
            // Arrange: with tag handling None DeepL can return an opening tag without its closing tag
            var tagPlacer = new DeepLTranslationProviderTagPlacer(
                Source("Jeder <488>Betriebstag</488> wird durch seine <489>ID</489> und den <490>Kurznamen</490> identifiziert."), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment(
                "Ogni <TG488>giorno d'esercizio</TG488> è identificato dal proprio <TG489>ID</TG489> e dall'abbreviazione <TG490>.");

            // Assert: the translation is kept with its valid pairs, so Studio does not drop it
            Assert.Equal("Ogni <Start488>giorno d'esercizio<End488> è identificato dal proprio <Start489>ID<End489> e dall'abbreviazione .", Render(target));
            Assert.True(IsValidIn(target));
        }

        [Fact]
        public void GetTaggedSegment_None_LeavesGarbledTagAsTextAndDropsItsUnpairedTag()
        {
            // Arrange: DeepL can garble the plugin's own tag, here </tg59> into </TGTG59>
            var tagPlacer = new DeepLTranslationProviderTagPlacer(
                Source("<59>Kurzname</59>: Eingabefeld"), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment("<TG59>Abbreviazione</TGTG59>: campo");

            // Assert: DeepL's output is not repaired; only the opening tag left without a partner goes
            Assert.Equal("Abbreviazione</TGTG59>: campo", Render(target));
            Assert.True(IsValidIn(target));
        }

        [Fact]
        public void GetTaggedSegment_None_KeepsOnePairWhenOpeningTagIsRepeated()
        {
            // Arrange: DeepL can repeat an opening tag; one of the two has no closing tag
            var tagPlacer = new DeepLTranslationProviderTagPlacer(
                Source("für alle <1327>Kann</1327>-Fahrten"), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment("per tutte le corse possibili <TG1327><TG1327>possibili</TG1327>.");

            // Assert: the closing tag pairs with the inner opening tag; the outer one goes
            Assert.Equal("per tutte le corse possibili <Start1327>possibili<End1327>.", Render(target));
            Assert.True(IsValidIn(target));
        }

        [Fact]
        public void GetTaggedSegment_None_DropsClosingTagWithoutOpeningTag()
        {
            // Arrange: DeepL can put a closing tag before its opening tag
            var tagPlacer = new DeepLTranslationProviderTagPlacer(Source("Der <1>Text</1>."), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment("Il </TG1>testo<TG1>.");

            // Assert: neither tag has a partner, so both go and the text stays
            Assert.Equal("Il testo.", Render(target));
            Assert.True(IsValidIn(target));
        }

        [Fact]
        public void GetTaggedSegment_None_KeepsOnlyCorrectlyNestedPairWhenPairsOverlap()
        {
            // Arrange: DeepL can return two pairs that cross each other
            var tagPlacer = new DeepLTranslationProviderTagPlacer(Source("<1>Ein <2>roter</2> Text</1>"), TagFormat.None);

            // Act
            var target = tagPlacer.GetTaggedSegment("<TG1>Un <TG2>testo</TG1> rosso</TG2>");

            // Assert: </TG1> does not close the innermost tag, so pair 1 goes and pair 2 stays
            Assert.Equal("Un <Start2>testo rosso<End2>", Render(target));
            Assert.True(IsValidIn(target));
        }

        // "<5>" start, "</5>" end, "<5/>" standalone; tag ID and anchor are the number
        private static Segment Source(string markup)
        {
            var source = new Segment(new CultureCode("de-DE"));
            foreach (var piece in Regex.Split(markup, @"(</?\d+/?>)"))
            {
                var match = Regex.Match(piece, @"^<(/?)(\d+)(/?)>$");
                if (!match.Success)
                {
                    if (piece.Length > 0) source.Add(piece);
                    continue;
                }

                var type = match.Groups[1].Value == "/" ? TagType.End : match.Groups[3].Value == "/" ? TagType.Standalone : TagType.Start;
                source.Add(new Tag(type, match.Groups[2].Value, int.Parse(match.Groups[2].Value)));
            }
            return source;
        }

        // Studio judges a target segment with its target culture
        private static bool IsValidIn(Segment target)
        {
            var withCulture = new Segment(new CultureCode("it-IT"));
            foreach (var element in target.Elements) withCulture.Add(element);
            return withCulture.IsValid();
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
