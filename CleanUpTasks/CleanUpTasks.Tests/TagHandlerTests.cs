using System;
using System.Collections.Generic;
using NSubstitute;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TagHandlerTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void ConstructorThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TagHandler(null, null, null, null, null));
        }

        [Fact]
        public void VisitSegmentSkipsLockedSegments()
        {
            // Arrange
            var settings = utility.CreateSettings(bold: true);
            settings.FormatTagList = new Dictionary<string, bool>();
            settings.PlaceholderTagList = new Dictionary<string, bool>();
            var fmtVisitor = utility.CreateFormattingVisitor();
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var tagHandler = new TagHandler(settings, fmtVisitor, itemFactory, reporter, reportGenerator);
            var segment = utility.CreateSegment(isLocked: true);
            var tagPair = utility.CreateTag(new Bold(true));
            segment.GetEnumerator().Returns(utility.AbstractMarkupDataContainer(tagPair));

            // Act
            tagHandler.VisitSegment(segment);

            // Assert
            tagPair.DidNotReceive().AcceptVisitor(tagHandler);
        }

        [Fact]
        public void VisitSegmentDoesNotSkipUnlockedSegments()
        {
            // Arrange
            var settings = utility.CreateSettings(bold: true);
            settings.FormatTagList = new Dictionary<string, bool>();
            settings.PlaceholderTagList = new Dictionary<string, bool>();
            var fmtVisitor = utility.CreateFormattingVisitor();
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var tagHandler = new TagHandler(settings, fmtVisitor, itemFactory, reporter, reportGenerator);
            var segment = utility.CreateSegment(isLocked: false);
            var tagPair = utility.CreateTag(new Bold(true));
            segment.GetEnumerator().Returns(utility.AbstractMarkupDataContainer(tagPair));

            // Act
            tagHandler.VisitSegment(segment);

            // Assert
            tagPair.Received().AcceptVisitor(tagHandler);
        }

        [Fact]
        public void VisitTagPairCallsAcceptVisitor()
        {
            // Arrange
            var settings = utility.CreateSettings(bold: true);
            settings.FormatTagList = new Dictionary<string, bool>() { { "anything", true } };
            settings.PlaceholderTagList = new Dictionary<string, bool>();
            var fmtVisitor = utility.CreateFormattingVisitor();
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var tagHandler = new TagHandler(settings, fmtVisitor, itemFactory, reporter, reportGenerator);
            var formattingItem = Substitute.For<IFormattingItem>();
            var tagPair = utility.CreateTag(formattingItem);

            // Act
            tagHandler.VisitTagPair(tagPair);

            // Assert
            formattingItem.Received().AcceptVisitor(fmtVisitor);
        }

        #region Fixture

        private readonly TestUtilities utility = null;

        public TagHandlerTests(TestUtilities utility)
        {
            this.utility = utility;
        }

        #endregion Fixture
    }
}