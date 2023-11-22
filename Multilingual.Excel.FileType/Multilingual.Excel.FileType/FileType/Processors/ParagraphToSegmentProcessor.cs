using System.Linq;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class ParagraphToSegmentProcessor : AbstractBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;

		public ParagraphToSegmentProcessor(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			if (!paragraphUnit.SegmentPairs.Any())
			{
				var source = _segmentBuilder.CreateSegment(_segmentBuilder.CreateSegmentPairProperties());
				foreach (var item in paragraphUnit.Source)
				{
					source.Add(item.Clone() as IAbstractMarkupData);
				}
				
				var target = _segmentBuilder.CreateSegment(source.Properties.Clone() as ISegmentPairProperties);
				foreach (var item in paragraphUnit.Target)
				{
					target.Add(item.Clone() as IAbstractMarkupData);
				}

				paragraphUnit.Source.Clear();
				paragraphUnit.Target.Clear();
				
				paragraphUnit.Source.Add(source);
				paragraphUnit.Target.Add(target);
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
