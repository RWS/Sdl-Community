using System.Linq;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class EmptyTargetProcessor : AbstractBilingualContentProcessor
	{
		private readonly bool _protect;
		private readonly SegmentBuilder _segmentBuilder;

		public EmptyTargetProcessor(SegmentBuilder segmentBuilder, bool protect)
		{
			_segmentBuilder = segmentBuilder;
			_protect = protect;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				if (_protect)
				{
					if (!segmentPair.Target.Any())
					{
						var text = _segmentBuilder.Text(EntityConstants.EmptyTargetRefEscape);
						segmentPair.Target.Add(text);
					}
				}
				else
				{
					if (segmentPair.Target.Any())
					{
						if (segmentPair.Target[0] is IText text)
						{
							if (text.Properties.Text == EntityConstants.EmptyTargetRefEscape)
							{
								segmentPair.Target.Clear();
							}
						}
					}
				}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}