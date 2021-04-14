using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Xunit;

namespace UnitTests.Services
{
	public class ParagraphUnitProviderTests
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly SegmentVisitor _segmentVisitor;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;

		public ParagraphUnitProviderTests()
		{
			_segmentBuilder = new SegmentBuilder();
			_segmentVisitor = new SegmentVisitor();
			_paragraphUnitProvider = new ParagraphUnitProvider(_segmentVisitor);
		}

		[Fact]
		public void GetUpdatedParagraphUnit_ReturnsEqual_WhenMultipleSegmentsAreSplitAndMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "Cats!", string.Empty),
				GetSegmentPair("1 b a", " and", string.Empty),
				GetSegmentPair("1 b b", " Dogs!", string.Empty),
				GetSegmentPair("2", "First. Second!", string.Empty),
				GetSegmentPair("3", "Jack and Jill", string.Empty),
				GetSegmentPair("4", "Last section", string.Empty) // Alignment = LeftOnly
			};
			var paragraphUnit1 = CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "Cats! and Dogs!", "Gatti! e Cani!"), // merged {1 a, 1 b a, 1 b b}
				GetSegmentPair("2 a", "First.", "Primo."), // split from segment id 2
				GetSegmentPair("2 b", " Second!", " Secondo!"), // split from segment id 2
				GetSegmentPair("3", "Jack and Jill", "Jack e Jill") // matched
			};
			var paragraphUnit2 = CreateParagraph(segmentPairs2);


			// act
			var paragraphUnit = _paragraphUnitProvider.GetUpdatedParagraphUnit(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(5, paragraphUnit.Source.Count);
			Assert.Equal(5, paragraphUnit.Target.Count);

			// confirm that last source (LeftOnly) content is built into the paragraph by comparing
			// sourceLeft (paragraph submitted) against sourceRight (paragraphUnit received).
			_segmentVisitor.VisitSegment(segmentPairs1[segmentPairs1.Count - 1].Source);
			var sourceLeft = _segmentVisitor.Text;
			var sourceRight = ((IText) paragraphUnit.Source[paragraphUnit.Source.Count - 1]).Properties.Text;

			Assert.Equal(sourceLeft, sourceRight);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenSegmentIsSplit()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair> { GetSegmentPair("1", "First. Second!", string.Empty) };
			var paragraphUnit1 = CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "First.", "Primo."),
				GetSegmentPair("1 b", "Second!", "Secondo!")
			};
			var paragraphUnit2 = CreateParagraph(segmentPairs2);

			// act
			var alignment = _paragraphUnitProvider.GetSegmentPairAlignment(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(3, alignment.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1", alignment[0].SegmentId);
			Assert.Equal("1 a", alignment[1].SegmentId);
			Assert.Equal("1 b", alignment[2].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignment[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignment[1].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignment[2].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenSegmentIsMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "First.", string.Empty),
				GetSegmentPair("1 b", "Second!", string.Empty)
			};
			var paragraphUnit1 = CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair> { GetSegmentPair("1 a", "First. Second!", "Primo. Secondo!") };
			var paragraphUnit2 = CreateParagraph(segmentPairs2);

			// act
			var alignment = _paragraphUnitProvider.GetSegmentPairAlignment(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(2, alignment.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1 a", alignment[0].SegmentId);
			Assert.Equal("1 b", alignment[1].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignment[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignment[1].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenMultipleSegmentsAreSplitAndMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "Cats!", string.Empty),
				GetSegmentPair("1 b a", " and", string.Empty),
				GetSegmentPair("1 b b", " Dogs!", string.Empty),
				GetSegmentPair("2", "First. Second!", string.Empty),
				GetSegmentPair("3", "Jack and Jill", string.Empty)
			};
			var paragraphUnit1 = CreateParagraph(segmentPairs1);

			var segmentPairs2 = new List<ISegmentPair>
			{
				GetSegmentPair("1 a", "Cats! and Dogs!", "Gatti! e Cani!"), // merged {1 a, 1 b a, 1 b b}
				GetSegmentPair("2 a", "First.", "Primo."), // split from segment id 2
				GetSegmentPair("2 b", "Second!", "Secondo!"), // split from segment id 2
				GetSegmentPair("3", "Jack and Jill", "Jack e Jill") // matched
			};
			var paragraphUnit2 = CreateParagraph(segmentPairs2);

			// act
			var alignment = _paragraphUnitProvider.GetSegmentPairAlignment(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Equal(7, alignment.Count);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1 a", alignment[0].SegmentId);
			Assert.Equal("1 b a", alignment[1].SegmentId);
			Assert.Equal("1 b b", alignment[2].SegmentId);
			Assert.Equal("2", alignment[3].SegmentId);
			Assert.Equal("2 a", alignment[4].SegmentId);
			Assert.Equal("2 b", alignment[5].SegmentId);
			Assert.Equal("3", alignment[6].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignment[0].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignment[1].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignment[2].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Removed, alignment[3].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignment[4].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Added, alignment[5].Alignment);
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignment[6].Alignment);
		}

		[Fact]
		public void GetSegmentPairAlignment_ReturnsEqual_WhenNotSplitOrMerged()
		{
			// arrange
			var segmentPairs1 = new List<ISegmentPair> { GetSegmentPair("1", "First. Second!", string.Empty) };
			var segmentPairs2 = new List<ISegmentPair> { GetSegmentPair("1", "First. Second!", "Primo. Secondo!") };

			var paragraphUnit1 = CreateParagraph(segmentPairs1);
			var paragraphUnit2 = CreateParagraph(segmentPairs2);

			// act
			var alignment = _paragraphUnitProvider.GetSegmentPairAlignment(paragraphUnit1, paragraphUnit2);

			// assert
			Assert.Single(alignment);

			// verify that the expected segment ids exist and in the correct order
			Assert.Equal("1", alignment[0].SegmentId);

			// verify that the expected alignment types
			Assert.Equal(AlignmentInfo.AlignmentType.Matched, alignment[0].Alignment);
		}

		private ISegmentPair GetSegmentPair(string segmentId, string sourceText, string targetText)
		{
			var segmentPairProperties = _segmentBuilder.CreateSegmentPairProperties();
			segmentPairProperties.Id = new SegmentId(segmentId);

			var sourceSegment = _segmentBuilder.CreateSegment(segmentPairProperties);
			sourceSegment.Properties = segmentPairProperties;
			sourceSegment.Add(_segmentBuilder.Text(sourceText));

			var targetSegment = _segmentBuilder.CreateSegment(segmentPairProperties);
			targetSegment.Properties = segmentPairProperties;
			targetSegment.Add(_segmentBuilder.Text(targetText));

			var segmentPair = _segmentBuilder.CreateSegmentPair(sourceSegment, targetSegment);
			return segmentPair;
		}

		private IParagraphUnit CreateParagraph(IEnumerable<ISegmentPair> segmentPairs)
		{
			var paragraphUnit = _segmentBuilder.CreateParagraphUnit(LockTypeFlags.Unlocked);
			paragraphUnit.Properties.ParagraphUnitId = new ParagraphUnitId(Guid.NewGuid().ToString());
			foreach (var segmentPair in segmentPairs)
			{
				paragraphUnit.Source.Add(segmentPair.Source);
				paragraphUnit.Target.Add(segmentPair.Target);
			}

			return paragraphUnit;
		}

		private ITagPair GetTagPair(string tagId, string tag, string content, List<string> tagIds)
		{
			var tagPair = _segmentBuilder.CreateTagPair(tagId, tag, ref tagIds);
			tagPair.Add(_segmentBuilder.Text(content));
			return tagPair;
		}

		private string GetUniqueTagPairId(List<string> existingTagIds)
		{
			var id = 1;
			while (existingTagIds.Contains(id.ToString()))
			{
				id++;
			}

			return id.ToString();
		}
	}
}
