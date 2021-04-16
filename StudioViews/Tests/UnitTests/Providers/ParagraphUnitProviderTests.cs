using System.Collections.Generic;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using UnitTests.Utils;
using Xunit;

namespace UnitTests.Providers
{
	public class ParagraphUnitProviderTests
	{
		private readonly ParagraphUnitHelper _paragraphUnitHelper;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;

		public ParagraphUnitProviderTests()
		{
			var segmentBuilder = new SegmentBuilder();
			_paragraphUnitHelper = new ParagraphUnitHelper(segmentBuilder);

			var segmentVisitor = new SegmentVisitor();
			_paragraphUnitProvider = new ParagraphUnitProvider(segmentVisitor);
		}

		[Fact]
		public void GetUpdatedParagraphUnit_ReturnsEqual_WhenMultipleSegmentsAreSplitAndMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "Cats!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("1 b a", " and", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("1 b b", " Dogs!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("2", "First. Second!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("3", "Jack and Jill", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("4", "Last section", string.Empty) // Alignment = LeftOnly
			};
			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "Cats! and Dogs!", "Gatti! e Cani!"), // merged {1 a, 1 b a, 1 b b}
				_paragraphUnitHelper.GetSegmentPair("2 a", "First.", "Primo."), // split from segment id 2
				_paragraphUnitHelper.GetSegmentPair("2 b", " Second!", " Secondo!"), // split from segment id 2
				_paragraphUnitHelper.GetSegmentPair("3", "Jack and Jill", "Jack e Jill") // matched
			};
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);


			// act
			var paragraphUnit = _paragraphUnitProvider.GetUpdatedParagraphUnit(paragraphUnit1, paragraphUnit2);

			// assert
			// confirm that the same amount of ISegments exist in source and target
			Assert.Equal(5, paragraphUnit.Source.Count);
			Assert.Equal(5, paragraphUnit.Target.Count);

			// confirm that last source (LeftOnly) content is built into the paragraph by comparing
			// sourceLeft (paragraph submitted) against sourceRight (paragraphUnit received).
			var sourceLeft = "Last section";
			var sourceRight = ((IText)paragraphUnit.Source[paragraphUnit.Source.Count - 1]).Properties.Text;

			Assert.Equal(sourceLeft, sourceRight);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenSegmentIsSplit()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair> { _paragraphUnitHelper.GetSegmentPair("1", "First. Second!", string.Empty) };
			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "First.", "Primo."),
				_paragraphUnitHelper.GetSegmentPair("1 b", "Second!", "Secondo!")
			};
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);

			// act
			var alignments = _paragraphUnitProvider.GetSegmentPairAlignments(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(3, alignments.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1", alignments[0].SegmentId);
			Assert.Equal("1 a", alignments[1].SegmentId);
			Assert.Equal("1 b", alignments[2].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignments[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignments[1].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignments[2].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenSegmentIsMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "First.", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("1 b", "Second!", string.Empty)
			};
			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair> { _paragraphUnitHelper.GetSegmentPair("1 a", "First. Second!", "Primo. Secondo!") };
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);

			// act
			var alignments = _paragraphUnitProvider.GetSegmentPairAlignments(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(2, alignments.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1 a", alignments[0].SegmentId);
			Assert.Equal("1 b", alignments[1].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignments[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignments[1].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenMultipleSegmentsAreSplitAndMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "Cats!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("1 b a", " and", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("1 b b", " Dogs!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("2", "First. Second!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("3", "Jack and Jill", string.Empty)
			};
			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1 a", "Cats! and Dogs!", "Gatti! e Cani!"), // merged {1 a, 1 b a, 1 b b}
				_paragraphUnitHelper.GetSegmentPair("2 a", "First.", "Primo."), // split from segment id 2
				_paragraphUnitHelper.GetSegmentPair("2 b", "Second!", "Secondo!"), // split from segment id 2
				_paragraphUnitHelper.GetSegmentPair("3", "Jack and Jill", "Jack e Jill") // matched
			};
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);

			// act
			var alignments = _paragraphUnitProvider.GetSegmentPairAlignments(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(7, alignments.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1 a", alignments[0].SegmentId);
			Assert.Equal("1 b a", alignments[1].SegmentId);
			Assert.Equal("1 b b", alignments[2].SegmentId);
			Assert.Equal("2", alignments[3].SegmentId);
			Assert.Equal("2 a", alignments[4].SegmentId);
			Assert.Equal("2 b", alignments[5].SegmentId);
			Assert.Equal("3", alignments[6].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignments[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignments[1].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignments[2].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignments[3].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignments[4].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignments[5].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignments[6].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenNotSplitOrMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair> { _paragraphUnitHelper.GetSegmentPair("1", "First. Second!", string.Empty) };
			var segmentPairs2 = new List<ISegmentPair> { _paragraphUnitHelper.GetSegmentPair("1", "First. Second!", "Primo. Secondo!") };

			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);

			// act
			var alignments = _paragraphUnitProvider.GetSegmentPairAlignments(paragraphUnit1, paragraphUnit2);

			// assert

			// verify only a single alignment result is returned
			Assert.Single(alignments);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1", alignments[0].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignments[0].Alignment);
		}
	}
}
