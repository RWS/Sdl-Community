using System;
using System.Collections.Generic;
using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace UnitTests.Utils
{
	public class ParagraphUnitHelper
	{
		private readonly SegmentBuilder _segmentBuilder;
		
		public ParagraphUnitHelper(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}
		
		public ISegmentPair GetSegmentPair(string segmentId, string sourceText, string targetText)
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

		public IParagraphUnit CreateParagraph(IEnumerable<ISegmentPair> segmentPairs)
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

		public ITagPair GetTagPair(string tagId, string tag, string content, List<string> tagIds)
		{
			var tagPair = _segmentBuilder.CreateTagPair(tagId, tag, ref tagIds);
			tagPair.Add(_segmentBuilder.Text(content));
			return tagPair;
		}

		public string GetUniqueTagPairId(List<string> existingTagIds)
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
