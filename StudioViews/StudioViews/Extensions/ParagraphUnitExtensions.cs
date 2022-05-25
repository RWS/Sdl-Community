﻿using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StudioViews.Extensions
{
	public static class ParagraphUnitExtensions
	{
		public static IParagraph AddSegment(this IParagraph paragraph, ISegment segment, SegmentBuilder builder)
		{
			if (segment != null)
			{
				var newSegment = builder.CreateSegment(segment.Properties.Clone() as ISegmentPairProperties);
				foreach (var item in segment)
				{
					newSegment.Add(item?.Clone() as IAbstractMarkupData);
				}
				
				paragraph.Add(newSegment);
			}

			return paragraph;
		}

		public static IAbstractMarkupDataContainer AddSegment(this IAbstractMarkupDataContainer paragraph, ISegment segment, SegmentBuilder builder)
		{
			if (segment != null)
			{
				var newSegment = builder.CreateSegment(segment.Properties.Clone() as ISegmentPairProperties);
				foreach (var item in segment)
				{
					newSegment.Add(item?.Clone() as IAbstractMarkupData);
				}

				paragraph.Add(newSegment);
			}

			return paragraph;
		}
	}
}
