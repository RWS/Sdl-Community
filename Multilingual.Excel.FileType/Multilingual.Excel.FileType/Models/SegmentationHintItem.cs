using System;
using System.Collections.Generic;
using Multilingual.Excel.FileType.Extensions;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Models
{
	public class SegmentationHintItem
	{
		public string Key { get; set; }

		public string Name { get; set; }

		public SegmentationHint Item { get; set; }

		public static List<SegmentationHintItem> GetSegmentationHintItems()
		{
			var keys = Enum.GetNames(typeof(SegmentationHint));
			var items = new List<SegmentationHintItem>();
			foreach (var key in keys)
			{
				var segmentationHint = (SegmentationHint)Enum.Parse(typeof(SegmentationHint), key, true);
				if (segmentationHint != SegmentationHint.Undefined && segmentationHint != SegmentationHint.Exclude)
				{
					var item = new SegmentationHintItem
					{
						Item = segmentationHint,
						Key = key,
						Name = key.SplitCapitalizedWords()
					};
					items.Add(item);
				}
			}

			return items;
		}
	}
}
