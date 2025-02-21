using System;
using System.Collections.Generic;
using NSubstitute;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class ConversionCleanUpHandlerTests : IClassFixture<TestUtilities>
	{
		[Fact]
		public void ConstructorThrowsException()
		{
			var list = new List<ConversionItemList>();

			Assert.Throws<ArgumentNullException>(() => new ConversionCleanupHandler(null, list, null, null, null, BatchTaskMode.Source));
		}

		[Fact]
		public void VisitSegmentCreatesSinglePlaceholderWithEmbeddedTags()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<.*?>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("<html>", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			Assert.Equal(1, sourceHandler.PlaceholderList.Count);
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPairWithSoftReturns()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<.*?>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("<head>\n<title>\nWhen non - translatable styles were not used\n</title>\n</head>", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			Assert.Equal(4, sourceHandler.PlaceholderList.Count);
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPair()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<strong>text is bold</strong>",
														 embeddedTags: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This <strong>text is bold</strong> and the tags should stay", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			Assert.Equal(2, sourceHandler.PlaceholderList.Count);
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPairRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<strong>(.+?)</strong>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This <strong>text is bold</strong> and the tags should stay", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPairNested()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<strong>(.+?)</strong>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This <strong>text <nested/>is bold</strong> and the tags should stay", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			Assert.Equal(3, sourceHandler.PlaceholderList.Count);
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPairNestedTwice()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<strong>(.+?)</strong>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This <strong>text with <more> nested <nested/> </more> is bold</strong> and the tags should stay", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			Assert.Equal(5, sourceHandler.PlaceholderList.Count);
		}

		[Fact]
		public void VisitSegmentCreatesOnePlaceholderWithEmbeddedTag()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "text",
														 placeHolder: true,
														 replacementText: "<text />");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This text <will/> be replaced", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
		}

		[Fact]
		public void VisitSegmentReportsWarningWhenReplacementEmpty()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "text",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This text <will/> be replaced", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.DidNotReceive().RemoveFromParent();
			reporter.Received().Report(sourceHandler, ErrorLevel.Warning, Arg.Any<string>(), Arg.Any<string>());
		}

		[Fact]
		public void VisitSegmentCreatesEmbeddedTagPairRegexBackReferences()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"<([a-z]*)>.*?<\/\1>",
														 embeddedTags: true,
														 useRegex: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("This <strong>text is bold</strong> and the tags should stay", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
		}

		[Fact]
		public void VisitSegmentCreatesNewFormattingTagPair()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "some text",
														 placeHolder: true,
														 replacementText: "<cf bold=\"True\">some text</cf>");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some text", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			text.Received().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
		}

		[Fact]
		public void VisitSegmentCreatesPlaceholderElement()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"name",
														 replacementText: "<name />",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("hello my name is bob", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentCreatesPlaceholderElementAndUpdatesSurroundingTextToUpper()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"([0-9]+)",
														 useRegex: true,
														 replacementText: @"<numbers value=""$1""/>",
														 placeHolder: true);

			list[0].Items.Add(new ConversionItem()
			{
				Search = new SearchText() { Text = "here is" },
				Replacement = new ReplacementText() { Text = "I REPLACED THIS" }
			});

			list[0].Items.Add(new ConversionItem()
			{
				Search = new SearchText() { Text = "other" },
				Replacement = new ReplacementText() { Text = "", ToUpper = true }
			});

			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("here is some numbers 123456 and some other stuff", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.DidNotReceive().RemoveFromParent();
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentCreatesPlaceholderElementAndUpdatesText()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"([0-9]+)",
														 useRegex: true,
														 replacementText: @"<numbers value=""$1""/>",
														 placeHolder: true);

			list[0].Items.Add(new ConversionItem()
			{
				Search = new SearchText() { Text = "here is" },
				Replacement = new ReplacementText() { Text = "I REPLACED THIS" }
			});

			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("here is some numbers 123456", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.DidNotReceive().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentCreatesPlaceholderElementWithAttributeRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"([0-9]+)",
														 useRegex: true,
														 replacementText: @"<numbers value=""$1""/>",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("here is some numbers 123456", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.DidNotReceive().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentCreatesSinglePlaceholderElement()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"Company",
														 useRegex: true,
														 replacementText: @"<Company />",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("Company", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentDoesNotUpdateITextInvalidRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @".)",
														 useRegex: true,
														 caseSensitive: false,
														 replacementText: "whatever");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("I remain the same", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("I remain the same", result);
		}

		[Fact]
		public void VisitSegmentDoesNotUpdateITextNoMatchNormalCaseSensitive()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"no match",
														 caseSensitive: true,
														 replacementText: "whatever");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("NO MATCH", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("NO MATCH", result);
		}

		[Fact]
		public void VisitSegmentDoesNotUpdateITextNoMatchRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"[0-9]",
														 caseSensitive: false,
														 replacementText: "whatever");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("I contain no numbers", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("I contain no numbers", result);
		}

		[Fact]
		public void VisitSegmentMatchesTagPairAndCreatesPlaceholder()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<tag>(.+?)</tag>",
														 tagPair: true,
														 replacementText: @"<tag trans=""$1""/>",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some text", segment);
			var tagPair = utility.CreateTag("<tag>", "</tag>", text);
			tagPair.Parent = utility.CreateSegment();
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitTagPair(tagPair);
			sourceHandler.VisitSegment(segment);

			// Assert
			tagPair.Parent.Received().Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			tagPair.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentMatchesTagPairAndUpdatesContent()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<cf bold=\"True\">(.+?)</cf>",
														 tagPair: true,
														 replacementText: "<cf bold=\"True\">$1</cf>",
														 toUpper: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some text", segment);
			var tagPair = utility.CreateTag("<cf bold=\"True\">", "</cf>", text);
			tagPair.Parent = utility.CreateSegment();

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitTagPair(tagPair);
			sourceHandler.VisitSegment(segment);

			// Assert
			tagPair.Received().Add(Arg.Any<IAbstractMarkupData>());
			tagPair.DidNotReceive().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentMatchesTagPairAndUpdatesFormatting()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<cf bold=\"True\">(.+?)</cf>",
														 tagPair: true,
														 replacementText: "<cf bold=\"False\">$1</cf>");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some text", segment);
			var tagPair = utility.CreateTag("<cf bold=\"True\">", "</cf>", text);
			tagPair.Parent = utility.CreateSegment();

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitTagPair(tagPair);
			sourceHandler.VisitSegment(segment);

			// Assert
			tagPair.StartTagProperties.Formatting.Received().Add(Arg.Any<IFormattingItem>());
			tagPair.DidNotReceive().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentMatchesWholeWord()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "the",
														 wholeWord: true,
														 replacementText: "REPLACED");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("theater the them their", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			Assert.Equal("theater REPLACED them their", text.Properties.Text);
		}

		[Fact]
		public void VisitSegmentMatchesWholeWordPlaceholder()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "d",
														 wholeWord: true,
														 replacementText: @"<locked name=""d""/>",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("hidden d in does dead", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.Received(3).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentMatchesWholeWordWithMultipleReplacements()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "d",
														 wholeWord: true,
														 replacementText: @"<locked name=""d""/>",
														 placeHolder: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("hidden d in does dead and there is another d", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			segment.Received(4).Insert(Arg.Any<int>(), Arg.Any<IAbstractMarkupData>());
			text.Received().RemoveFromParent();
		}

		[Fact]
		public void VisitSegmentsSkipsLockedSegment()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = new List<ConversionItemList>();
			var segment = utility.CreateSegment(isLocked: true);
			var tagPair = utility.CreateTag();
			segment.GetEnumerator().Returns(utility.AbstractMarkupDataContainer(tagPair));
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitSegment(segment);

			// Assert
			tagPair.DidNotReceive().AcceptVisitor(sourceHandler);
		}

		[Fact]
		public void VisitSegmentUpdatesITextCaseInSensitive()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"hello",
														 caseSensitive: false,
														 replacementText: "hello");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("Hello HELLO heLLo", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("hello hello hello", result);
		}

		[Fact]
		public void VisitSegmentUpdatesITextCaseSensitive()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "TEXT RIGHT HERE",
														 caseSensitive: true,
														 replacementText: "I REPLACED IT");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("Text right here TEXT RIGHT HERE", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("Text right here I REPLACED IT", result);
		}

		[Fact]
		public void VisitSegmentUpdatesITextComplexSegmentTagSensitive()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "Delete me",
														 replacementText: "");
			var segment = utility.CreateSegment(isLocked: false);
			var text1 = utility.CreateText("Delete me", segment);
			var text2 = utility.CreateText("I will remain", segment);
			var tag = utility.CreateTag(new Bold(true));

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text1);
			sourceHandler.VisitTagPair(tag);
			sourceHandler.VisitText(text2);
			sourceHandler.VisitSegment(segment);

			// Assert
			text1.Received().RemoveFromParent();
			Assert.Equal("I will remain", text2.Properties.Text);
		}

		[Fact]
		public void VisitSegmentUpdatesITextJapanese()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "ｶﾀｶﾅ",
														 replacementText: "かたかな");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("ｶﾀｶﾅかたかなｶﾀｶﾅ", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("かたかなかたかなかたかな", result);
		}

		[Fact]
		public void VisitSegmentUpdatesITextRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"\b(\w+)(\s)(\w+)\b",
														 caseSensitive: true,
														 useRegex: true,
														 replacementText: "$3$2$1");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("one two", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("two one", result);
		}

		[Fact]
		public void VisitSegmentUpdatesITextStrConv()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "ｶﾀｶﾅ",
														 strConv: true,
														 vbstrConv: utility.CreateVbStrConvList(Microsoft.VisualBasic.VbStrConv.Wide),
														 replacementText: "");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("ｶﾀｶﾅHelloｶﾀｶﾅ", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			Assert.Equal("カタカナHelloカタカナ", text.Properties.Text);
		}

		[Fact]
		public void VisitSegmentUpdatesITextStrConvRegex()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "[ｦ-ﾟ]+",
														 strConv: true,
														 useRegex: true,
														 vbstrConv: utility.CreateVbStrConvList(Microsoft.VisualBasic.VbStrConv.Wide),
														 replacementText: "");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("ｶﾀｶﾅHelloｶﾀｶﾅ", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			Assert.Equal("カタカナHelloカタカナ", text.Properties.Text);
		}

		[Fact]
		public void VisitSegmentUpdatesITextStrConvUppercase()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: ".+",
														 strConv: true,
														 useRegex: true,
														 vbstrConv: utility.CreateVbStrConvList(Microsoft.VisualBasic.VbStrConv.Uppercase),
														 replacementText: "");
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some lowercase text", segment);

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			Assert.Equal("SOME LOWERCASE TEXT", text.Properties.Text);
		}

		[Fact]
		public void VisitSegmentUpdatesITextToLower()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: @"[A-Z]\b",
														 useRegex: true,
														 caseSensitive: true,
														 replacementText: "",
														 toLower: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("thE lasT letteR shoulD bE madE lowercasE", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("the last letter should be made lowercase", result);
		}

		[Fact]
		public void VisitSegmentUpdatesITextToUpper()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "[a-z]",
														 useRegex: true,
														 caseSensitive: true,
														 replacementText: "",
														 toUpper: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("SOME TEXT make me uppercase", segment);
			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitText(text);
			sourceHandler.VisitSegment(segment);

			// Assert
			var result = text.Properties.Text;
			Assert.Equal("SOME TEXT MAKE ME UPPERCASE", result);
		}

		[Fact]
		public void VisitSegmentUpdatesMetaDataOfFormattingTag()
		{
			// Arrange
			var settings = utility.CreateSettings();
			var list = utility.CreateConversionItemLists(searchText: "<cf bold=\"True\">(.+?)</cf>",
														 tagPair: true,
														 replacementText: "<cf bold=\"False\">$1</cf>",
														 toUpper: true);
			var segment = utility.CreateSegment(isLocked: false);
			var text = utility.CreateText("some text", segment);
			var tagPair = utility.CreateTag("<cf bold=\"True\">", "</cf>", text);
			tagPair.Parent = utility.CreateSegment();
			tagPair.StartTagProperties.TagContent = "<cf bold=\"True\">";
			tagPair.TagProperties.Returns(Substitute.For<IAbstractTagProperties>());
			var pair = new KeyValuePair<string, string>("bold", $@"<w:rPr> { Environment.NewLine }< w:bold w:val = ""True"" /></ w:rPr > ");
			tagPair.TagProperties.MetaData.Returns(new List<KeyValuePair<string, string>>()
			{
				pair
			});

			var itemFactory = Substitute.For<IDocumentItemFactory>();
			var reporter = Substitute.For<ICleanUpMessageReporter>();
			var reportGenerator = Substitute.For<IXmlReportGenerator>();
			var sourceHandler = new ConversionCleanupHandler(settings, list, itemFactory, reporter, reportGenerator, BatchTaskMode.Source);

			// Act
			sourceHandler.VisitTagPair(tagPair);
			sourceHandler.VisitSegment(segment);

			// Assert
			tagPair.StartTagProperties.Formatting.Received().Add(Arg.Any<IFormattingItem>());
			tagPair.TagProperties.Received().SetMetaData(Arg.Any<string>(), Arg.Any<string>());
		}

		#region Fixture

		private readonly TestUtilities utility = null;

		public ConversionCleanUpHandlerTests(TestUtilities utility)
		{
			this.utility = utility;
		}

		#endregion Fixture
	}
}