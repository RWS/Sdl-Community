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
                sendSurroundingSegments, sendCustomContext, "Maria is a woman.", translationUnits);

            // Assert
            Assert.Equal(expectUnits, contextUnits != null);
            Assert.Equal(expectedCustomContext, customContext);
        }
    }
}
