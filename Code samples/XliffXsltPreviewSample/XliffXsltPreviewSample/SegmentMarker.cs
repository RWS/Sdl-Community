using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace XliffXsltPreviewSample
{
	public class SegmentMarker : AbstractBilingualContentProcessor
	{
		internal const string Open = "{{SDLSEG:";
		internal const string Close = "}}";

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var paragraphUnitId = paragraphUnit.Properties.ParagraphUnitId.Id;
			foreach (var segment in paragraphUnit.Source.AllSubItems.Concat(paragraphUnit.Target.AllSubItems).OfType<ISegment>())
			{
				var marker = Open + paragraphUnitId + "DELIMITER" + segment.Properties.Id.Id + Close;
				segment.Insert(0, ItemFactory.CreateText(PropertiesFactory.CreateTextProperties(marker)));
			}
			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
