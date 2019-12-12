using System;
using NSubstitute;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class LockHandlerTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void ConstructorThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LockHandler(null, null, null, null));
        }

        [Fact]
        public void VisitSegmentDoesLockSegmentWhenWholeSegmentMatches()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              IsRegex = true,
                                              SearchText = @"^[0-9]+$"
                                          });

            var segment = utility.CreateSegment();
            var txt1 = utility.CreateText(text: "12345", segment: segment);
            var txt2 = utility.CreateText(text: "6789", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt1);
            handler.VisitText(txt2);
            handler.VisitSegment(segment);

            Assert.True(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentDoesNotLockSegmentWhenWholeSegmentDoesNotMatch()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              IsRegex = true,
                                              SearchText = @"^[0-9]+$"
                                          });

            var segment = utility.CreateSegment();
            var txt1 = utility.CreateText(text: "12345", segment: segment);
            var txt2 = utility.CreateText(text: "not a number", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt1);
            handler.VisitText(txt2);
            handler.VisitSegment(segment);

            Assert.False(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentIgnoresInvalidRegexExpressions()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              IsRegex = true,
                                              SearchText = @"^[ ("
                                          });

            var segment = utility.CreateSegment(isLocked: false);
            var txt = utility.CreateText(text: "whatever", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.False(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentIgnoresSegmentsOnCaseSensitiveRegex()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              IsRegex = true,
                                              IsCaseSensitive = true,
                                              SearchText = @"[A-Z]+"
                                          });

            var segment = utility.CreateSegment();
            var txt = utility.CreateText(text: "abcdefg", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.False(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentLocksSegment()
        {
            // Arrange
            var text = "matchtome";
            var settings = utility.CreateSettings(useSegmentLocker: false,
                                          useStructureLocker: true,
                                          contextDef: new ContextDef()
                                          {
                                              Type = text,
                                              IsChecked = true
                                          });

            var segment = utility.CreateSegment();
            var paragraphUnit = utility.CreateParagraphUnit(text);
            segment.ParentParagraphUnit.Returns(paragraphUnit);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitSegment(segment);

            Assert.True(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentLocksSegmentOnWholeWordMatch()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              WholeWord = true,
                                              SearchText = @"d"
                                          });

            var segment = utility.CreateSegment();
            var txt = utility.CreateText(text: "hidden dad d dear red", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.True(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentLocksSegmentsOnRegexMatch()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              IsRegex = true,
                                              SearchText = @"^[ -~]+$"
                                          });

            var segment = utility.CreateSegment();
            var txt = utility.CreateText(text: "ABCabc123%- ", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.True(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentSkipsSegmentsWhenUseStructureLockerFalse()
        {
            // Arrange
            var text = "matchtome";
            var settings = utility.CreateSettings(useSegmentLocker: false,
                                          useStructureLocker: false, // false instead of true
                                          contextDef: new ContextDef()
                                          {
                                              Type = text
                                          });

            var segment = utility.CreateSegment();
            var paragraphUnit = utility.CreateParagraphUnit(text);
            segment.ParentParagraphUnit.Returns(paragraphUnit);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitSegment(segment);

            Assert.False(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentsLocksSegmentsOnNormalMatch()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                              segmentLockItem: new SegmentLockItem()
                              {
                                  IsRegex = false,
                                  SearchText = "123"
                              });

            var segment = utility.CreateSegment();
            var txt = utility.CreateText(text: "ABCabc123%- ", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.True(segment.Properties.IsLocked);
        }

        [Fact]
        public void VisitSegmentThrowsOnNull()
        {
            var settings = Substitute.For<ICleanUpSourceSettings>();
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            Assert.Throws<ArgumentNullException>(() => handler.VisitSegment(null));
        }

        [Fact]
        public void VisitTextDoesNotLockSegmentOnWholeWordNoMatch()
        {
            // Arrange
            var settings = utility.CreateSettings(useContentLocker: true,
                                          segmentLockItem: new SegmentLockItem()
                                          {
                                              WholeWord = true,
                                              SearchText = @"d"
                                          });

            var segment = utility.CreateSegment();
            var txt = utility.CreateText(text: "hidden dad dear red", segment: segment);
            var itemFactory = Substitute.For<IDocumentItemFactory>();
            var reporter = Substitute.For<ICleanUpMessageReporter>();
            var reportGenerator = Substitute.For<IXmlReportGenerator>();
            var handler = new LockHandler(settings, itemFactory, reporter, reportGenerator);

            // Act
            handler.VisitText(txt);
            handler.VisitSegment(segment);

            Assert.False(segment.Properties.IsLocked);
        }

        #region Fixture

        private readonly TestUtilities utility = null;

        public LockHandlerTests(TestUtilities utility)
        {
            this.utility = utility;
        }

        #endregion Fixture
    }
}