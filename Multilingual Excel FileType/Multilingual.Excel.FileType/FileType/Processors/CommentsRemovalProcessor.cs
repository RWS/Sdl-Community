using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class CommentsRemovalProcessor : AbstractBilingualContentProcessor
	{

		public override void Initialize(IDocumentProperties documentInfo)
		{
			Output.Initialize(documentInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				Output.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			RemoveCommentMarkers(paragraphUnit.Source);
			RemoveCommentMarkers(paragraphUnit.Target);

			Output.ProcessParagraphUnit(paragraphUnit);
		}

		private static void RemoveCommentMarkers(IAbstractMarkupDataContainer paragraph)
		{
			var sourceComments = paragraph.AllSubItems.OfType<ICommentMarker>();

			while (sourceComments.Any())
			{
				foreach (var item in sourceComments)
				{
					item.MoveAllItemsTo(item.Parent);
					item.RemoveFromParent();
					break;
				}

				sourceComments = paragraph.AllSubItems.OfType<ICommentMarker>();
			}
		}
	}
}
