using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.MTConnectors.Google.GoogleService;
using System;
using Xunit;

namespace MTConnectors.Google.UnitTests.GoogleService
{
    public class SegmentConverterTests
    {
        public class ConvertSourceSegmentToText
        {
            [Fact]
            public void ThrowsArgumentNullException_WhenSegmentIsNull()
            {
                //Arrange

                //Act

                //Assert
                Assert.Throws<ArgumentNullException>(() => new SegmentConverter(null));
            }

            [Fact]
            public void ReturnStringEmpty_WhenSegmentHasNoElements()
            {
                //Arrange
                var segment = new Segment();

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.True(string.IsNullOrEmpty(converter.ConvertSourceSegmentToText()));
            }

            [Fact]
            public void ReturnStringWithNoTags_WhenSegmentHasNoTags()
            {
                //Arrange
                var segment = new Segment();
                var textWithNoTags = "I contain no tags";
                segment.Add(textWithNoTags);

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal(textWithNoTags, converter.ConvertSourceSegmentToText());
            }


            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnStringWithSingleTag_WhenSegmentHasOneTagAtTheEnd(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain one tag");
                segment.Add(new Tag(tagType, "1", 1));

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("I contain one tag<tg1-1/>", converter.ConvertSourceSegmentToText());
            }

            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnStringWithSingleTag_WhenSegmentHasOneTagInTheBeginning(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("I contain one tag");


                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("<tg1-1/>I contain one tag", converter.ConvertSourceSegmentToText());
            }


            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnStringWithSingleTag_WhenSegmentHasOneTagInTheMiddle(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("one tag");


                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("I contain<tg1-1/>one tag", converter.ConvertSourceSegmentToText());
            }


            [Fact]
            public void ReturnStringWithStartAndEndTag_WhenSegmentHasStartAndEndTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add("one");
                segment.Add(new Tag(TagType.End, "1", 1));
                segment.Add("tag");

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("I contain<tg1-1>one</tg1-1>tag", converter.ConvertSourceSegmentToText());
            }

            [Fact]
            public void ReturnStringWithStartTag_WhenSegmentHasOnlyStartTag()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add("one ");
                segment.Add("tag");

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("I contain<tg1-1>one tag", converter.ConvertSourceSegmentToText());
            }

            [Fact]
            public void ReturnStringWithImbricatedStartAndEndTag_WhenSegmentHasImbricatedStartAndEndTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add(new Tag(TagType.Start, "2", 2));
                segment.Add("one");
                segment.Add(new Tag(TagType.End, "2", 2));
                segment.Add(new Tag(TagType.End, "1", 1));
                segment.Add("tag");

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("I contain<tg1-1><tg2-2>one</tg2-2></tg1-1>tag", converter.ConvertSourceSegmentToText());
            }

            [Fact]
            public void ReturnsString_WhenSegmentHasTagsWithTheSameId()
            {
                //Arrange
                var segment = new Segment();
                segment.Add(new Tag(TagType.Start, "109", 1));
                segment.Add("Bold");
                segment.Add(new Tag(TagType.End, "109", 1));
                segment.Add(new Tag(TagType.Standalone, "110", 2));
                segment.Add(new Tag(TagType.Start, "111", 3));
                segment.Add("Italics");
                segment.Add(new Tag(TagType.End, "111", 3));
                segment.Add(new Tag(TagType.Standalone, "110", 4));
                segment.Add(new Tag(TagType.Start, "113", 5));
                segment.Add("Single Underline");
                segment.Add(new Tag(TagType.End, "113", 5));

                //Act
                var converter = new SegmentConverter(segment);

                //Assert
                Assert.Equal("<tg109-1>Bold</tg109-1><tg110-2/><tg111-3>Italics</tg111-3><tg110-4/><tg113-5>Single Underline</tg113-5>", converter.ConvertSourceSegmentToText());
            }
        }

        public class ConvertToSegment
        {
            [Fact]
            public void ReturnSegmentWithNoElement_WhenTextIsEmpty()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("This is a test");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment(string.Empty);

                //Assert
                Assert.Empty(result.Elements);
            }

            [Fact]
            public void ReturnSegmentWithNoElement_WhenTextIsNull()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("This is a test");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment(null);

                //Assert
                Assert.Empty(result.Elements);
            }

            [Fact]
            public void ReturnsSingleElementSegment_WhenTextHasNoTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("This is a test");

                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var translatedText = "Das ist ein Test";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.Single(result.Elements);
                Assert.Equal(translatedText, result.Elements[0].ToString());
            }

            [Fact]
            public void ReturnsDecodedSingleElementSegment_WhenTextHasNoTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("Hello G%C3%BCnter");

                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var translatedText = "Hallo G&#252;nter";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.Single(result.Elements);
                Assert.Equal("Hallo Günter", result.Elements[0].ToString());
            }

            [Fact]
            public void RemovesBomCharacters_WhenTextHasBomCharactersTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("Hello");

                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var translatedText = "Hallo" + (char)8203;
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.Single(result.Elements);
                Assert.Equal("Hallo", result.Elements[0].ToString());
            }

            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnSegmentWithSingleTag_WhenTagIsAtTheEnd(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain one tag");
                segment.Add(new Tag(tagType, "1", 1));
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var translatedText = "Ich habe einen tag<tg1-1/>";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(2, result.Elements.Count);
                Assert.Equal(tagType, (result.Elements[1] as Tag).Type);
            }

            [Theory]
            [InlineData(TagType.Standalone, 1)]
            [InlineData(TagType.TextPlaceholder, 1)]
            [InlineData(TagType.LockedContent, 1)]
            [InlineData(TagType.Standalone, 2)]
            [InlineData(TagType.TextPlaceholder, 2)]
            [InlineData(TagType.LockedContent, 2)]
            public void ReturnSegmentWithTheSameNumberOfSpaces_WhenTagIsAtTheEndAndIsPrecedeedBySpace(TagType tagType, int noSpaces)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain one tag".PadRight(noSpaces));
                segment.Add(new Tag(tagType, "1", 1));
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var startText = "Ich habe einen tag";
                var translatedText = startText + "<tg1-1/>";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(2, result.Elements.Count);
                Assert.Equal(startText.PadRight(noSpaces), result.Elements[0].ToString());
                Assert.Equal(tagType, (result.Elements[1] as Tag).Type);
            }

            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnSegmentWithSingleTag_WhenTranslatedTextHasOneTagInTheBeginning(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("I contain one tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();


                //Act
                var translatedText = "<tg1-1/>Ich habe einen tag";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(2, result.Elements.Count);
                Assert.Equal(tagType, (result.Elements[0] as Tag).Type);
            }

            [Theory]
            [InlineData(TagType.Standalone, 1)]
            [InlineData(TagType.TextPlaceholder, 1)]
            [InlineData(TagType.LockedContent, 1)]
            [InlineData(TagType.Standalone, 2)]
            [InlineData(TagType.TextPlaceholder, 2)]
            [InlineData(TagType.LockedContent, 2)]
            public void ReturnSegmentWithTheSameNumberOfSpaces_WhenTagIsAtTheBeginningAndIsFollowedBySpace(TagType tagType, int noSpaces)
            {
                //Arrange
                var segment = new Segment();
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("I contain one tag".PadLeft(noSpaces));
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var startText = "Ich habe einen tag";
                var translatedText = "<tg1-1/>" + startText;
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(2, result.Elements.Count);
                Assert.Equal(startText.PadLeft(noSpaces), result.Elements[1].ToString());
                Assert.Equal(tagType, (result.Elements[0] as Tag).Type);
            }

            [Theory]
            [InlineData(TagType.Standalone)]
            [InlineData(TagType.TextPlaceholder)]
            [InlineData(TagType.LockedContent)]
            public void ReturnSegmentWithSingleTag_WhenTranslatedTextHasOneTagInTheMiddle(TagType tagType)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("one tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();


                //Act
                var translatedText = "Ich habe<tg1-1/>einen tag";
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(3, result.Elements.Count);
                Assert.Equal(tagType, (result.Elements[1] as Tag).Type);
            }

            [Theory]
            [InlineData(TagType.Standalone, 1, 1)]
            [InlineData(TagType.TextPlaceholder, 1, 1)]
            [InlineData(TagType.LockedContent, 1, 1)]
            [InlineData(TagType.Standalone, 2, 3)]
            [InlineData(TagType.TextPlaceholder, 3, 1)]
            [InlineData(TagType.LockedContent, 3, 5)]
            public void ReturnSegmentWithTheSameNumberOfSpaces_WhenTagIsAtTheMiddleAndIsSurroundedBySpace(TagType tagType, int noSpacesLeft, int noSpacesRight)
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain".PadRight(noSpacesLeft));
                segment.Add(new Tag(tagType, "1", 1));
                segment.Add("one tag".PadLeft(noSpacesRight));
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var beforeTag = "Ich habe";
                var afterTag = "einen tag";
                var translatedText = beforeTag + "<tg1-1/>" + afterTag;
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(3, result.Elements.Count);
                Assert.Equal(beforeTag.PadRight(noSpacesLeft), result.Elements[0].ToString());
                Assert.Equal(afterTag.PadLeft(noSpacesRight), result.Elements[2].ToString());
            }

            [Fact]
            public void ReturnSegmentWithStartAndEndTag_WhenTranslatedTextHasStartAndEndTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain ");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add("one");
                segment.Add(new Tag(TagType.End, "1", 1));
                segment.Add(" tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act

                var result = converter.ConvertTargetTextToSegment("Ich habe<tg1-1>einen</tg1-1>tag");


                //Assert
                Assert.Equal(5, result.Elements.Count);
                Assert.Equal("Ich habe <1 id=1>einen</1> tag", result.ToString());
            }

            [Fact]
            public void ReturnsSegmentWithStartTag_WhenTranslatedTextHasOnlyStartTag()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add("one tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment("Ich habe<tg1-1>einen tag");

                //Assert
                Assert.Equal(3, result.Elements.Count);
                Assert.Equal("Ich habe<1 id=1>einen tag", result.ToString());
            }

            [Fact]
            public void ReturnSegmentithImbricatedStartAndEndTag_WhenTranslatedTextHasImbricatedStartAndEndTags()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain ");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add(new Tag(TagType.Start, "2", 2));
                segment.Add("one ");
                segment.Add(new Tag(TagType.End, "2", 2));
                segment.Add(new Tag(TagType.End, "1", 1));
                segment.Add("tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment("Ich habe<tg1-1><tg2-2>einen</tg2-2></tg1-1>tag");

                //Assert
                Assert.Equal(7, result.Elements.Count);
                Assert.Equal("Ich habe <1 id=1><2 id=2>einen </2></1>tag", result.ToString());
            }

            [Fact]
            public void ReturnSegmentWithImbricatedStartAndEndTagAndInBetweenText_WhenSegmentHasImbricatedStartAndEndTagsAndInBetweenText()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I ");
                segment.Add(new Tag(TagType.Start, "1", 1));
                segment.Add("have ");
                segment.Add(new Tag(TagType.Start, "2", 2));
                segment.Add("one ");
                segment.Add(new Tag(TagType.End, "2", 2));
                segment.Add("new");
                segment.Add(new Tag(TagType.End, "1", 1));
                segment.Add("tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment("Ich<tg1-1>habe<tg2-2>einen</tg2-2>neues</tg1-1>tag");

                //Assert
                Assert.Equal(9, result.Elements.Count);
                Assert.Equal("Ich <1 id=1>habe <2 id=2>einen </2>neues</1>tag", result.ToString());
            }

            [Fact]
            public void ReturnSegmentWithCorrectTag_WhenTagIdContainsLetters()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("I contain");
                segment.Add(new Tag(TagType.Standalone, "ph14", 1));
                segment.Add("one tag");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var beforeTag = "Ich habe";
                var afterTag = "einen tag";
                var translatedText = beforeTag + "<tgph14-1/>" + afterTag;
                var result = converter.ConvertTargetTextToSegment(translatedText);

                //Assert
                Assert.True(segment.HasTags);
                Assert.Equal(3, result.Elements.Count);
                Assert.Equal("ph14", (result.Elements[1] as Tag).TagID);
            }

            [Fact]
            public void ReturnsCorrectSpacing_WhenSegmentHasTagsWithTheSameId()
            {
                //Arrange
                var segment = new Segment();
                segment.Add("Bold ");
                segment.Add(new Tag(TagType.Standalone, "110", 1));
                segment.Add("  Italics   ");
                segment.Add(new Tag(TagType.Standalone, "110", 2));
                segment.Add("    Single Underline");
                var converter = new SegmentConverter(segment);
                converter.ConvertSourceSegmentToText();

                //Act
                var result = converter.ConvertTargetTextToSegment("Bold <tg110-1/>  Italics   <tg110-2/>    Single Underline");

                //Assert
                Assert.Equal(5, result.Elements.Count);
                Assert.Equal("Bold <1 id=110/>  Italics   <2 id=110/>    Single Underline", result.ToString());
            }

        }
    }
}
