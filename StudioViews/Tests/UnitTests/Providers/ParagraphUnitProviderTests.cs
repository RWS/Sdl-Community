using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StudioViews.Common;
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

			var analysisBands = new List<AnalysisBand>
			{
				new AnalysisBand {MaximumMatchValue = 74, MinimumMatchValue = 50},
				new AnalysisBand {MaximumMatchValue = 84, MinimumMatchValue = 75},
				new AnalysisBand {MaximumMatchValue = 94, MinimumMatchValue = 85},
				new AnalysisBand {MaximumMatchValue = 99, MinimumMatchValue = 95}
			};
			var filterItemService = new FilterItemService(analysisBands);
			var segmentVisitor = new SegmentVisitor();
			_paragraphUnitProvider = new ParagraphUnitProvider(segmentVisitor, filterItemService, segmentBuilder);
		}

		[Fact]
		public void GetUpdatedParagraphUnit_ReturnsEqual_WhenMergeAcrossParagraphs()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1", "Cats!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("2", " and", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("3", " Dogs!", string.Empty),
				_paragraphUnitHelper.GetSegmentPair("4", "Jack and Jill", string.Empty)
			};
			var paragraphUnit1 = _paragraphUnitHelper.CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				_paragraphUnitHelper.GetSegmentPair("1", "Cats! and Dogs!", "Gatti! e Cani!"),
				_paragraphUnitHelper.GetSegmentPair("2", "", ""),
				_paragraphUnitHelper.GetSegmentPair("3", "", ""),
				_paragraphUnitHelper.GetSegmentPair("4", "Jack and Jill", "Jack e Jill") 
			};
			_paragraphUnitHelper.SetMergedParagraphMetaData(segmentPairs2[0]);
			_paragraphUnitHelper.SetMergedParagraphMarker(segmentPairs2[1]);
			_paragraphUnitHelper.SetMergedParagraphMarker(segmentPairs2[2]);
			var paragraphUnit2 = _paragraphUnitHelper.CreateParagraph(segmentPairs2);

			// act
			var result = _paragraphUnitProvider.GetUpdatedParagraphUnit(paragraphUnit1, paragraphUnit2, new List<string>());

			// assert
			// confirm that the same amount of ISegments exist in source and target
			Assert.Equal(4, result.Paragraph.SegmentPairs.Count());

			// confirm tha the MergedParagraph markers exists and in the correct locations
			Assert.Equal(result.Paragraph.SegmentPairs.ToList()[0].Properties.TranslationOrigin.GetMetaData("MergeStatus"), Constants.MergedParagraph);
			Assert.Equal(result.Paragraph.SegmentPairs.ToList()[1].Properties.TranslationOrigin.OriginSystem, Constants.MergedParagraph);
			Assert.Equal(result.Paragraph.SegmentPairs.ToList()[2].Properties.TranslationOrigin.OriginSystem, Constants.MergedParagraph);

			// confirm that the first 3 segments from the input are exactly the same as the content returned in  the merged segment
			var sourceLeft = segmentPairs1[0].Source.FirstOrDefault()?.ToString() + 
			                 segmentPairs1[1].Source.FirstOrDefault() +
			                 segmentPairs1[2].Source.FirstOrDefault();
			var sourceRight = result.Paragraph.SegmentPairs.FirstOrDefault()?.Source.FirstOrDefault()?.ToString();
			Assert.Equal(sourceLeft, sourceRight);
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
			var result = _paragraphUnitProvider.GetUpdatedParagraphUnit(paragraphUnit1, paragraphUnit2, new List<string>());

			// assert
			// confirm that the same amount of ISegments exist in source and target
			Assert.Equal(5, result.Paragraph.SegmentPairs.Count());

			// confirm that last source (LeftOnly) content is built into the paragraph by comparing
			// sourceLeft (paragraph submitted) against sourceRight (paragraphUnit received).
			var sourceLeft = segmentPairs1.LastOrDefault()?.Source.FirstOrDefault()?.ToString();
			var sourceRight = result.Paragraph.SegmentPairs.LastOrDefault()?.Source.FirstOrDefault()?.ToString();

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

			// verify the expected alignment types
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

			// verify the expected alignment types
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

			// verify the expected alignment types
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

			// verify the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignments[0].Alignment);
		}
	}
}
