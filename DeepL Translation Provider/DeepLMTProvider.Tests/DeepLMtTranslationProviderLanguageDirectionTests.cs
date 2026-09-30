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
    }
}
