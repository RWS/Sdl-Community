using System.Linq;
using Sdl.Community.DeepLMTProvider.Helpers;
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
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Wählen Sie die ", "Option", " aus."));

            // Act
            var target = tagPlacer.GetTaggedSegment("Selezionare l'<tg1>opzione</tg1>.");

            // Assert
            Assert.Equal("Selezionare l'<Start1>opzione<End1>.", Render(target));
        }

        [Fact]
        public void GetTaggedSegment_KeepsSpaceDeepLAddsBesideTag()
        {
            // Arrange: the source glues the tag to the previous word, DeepL separates them
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("Klicken Sie auf", "OK", "."));

            // Act
            var target = tagPlacer.GetTaggedSegment("Cliquez sur <tg1>OK</tg1>.");

            // Assert
            Assert.Equal("Cliquez sur <Start1>OK<End1>.", Render(target));
        }

        [Fact]
        public void GetTaggedSegment_KeepsSpaceBetweenTags()
        {
            // Arrange: DeepL returns one source pair as two pairs separated by a space
            var tagPlacer = new DeepLTranslationProviderTagPlacer(TaggedSource("an der ", "Dienstart", " die"));

            // Act
            var target = tagPlacer.GetTaggedSegment("nella <tg1>categoria</tg1> <tg1>di</tg1> guida");

            // Assert
            Assert.Equal("nella <Start1>categoria<End1> <Start1>di<End1> guida", Render(target));
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
