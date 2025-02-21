using System.Linq;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class CommentsProcessor : AbstractBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly BilingualParser _parser;

		public CommentsProcessor(SegmentBuilder segmentBuilder, BilingualParser parser)
		{
			_segmentBuilder = segmentBuilder;
			_parser = parser;
		}

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

			//if (_parser.ParagraphComments.Count > 0)
			//{
			//	foreach (var comment in _parser.ParagraphComments)
			//	{
			//		if (paragraphUnit.Properties.Comments == null)
			//		{
			//			paragraphUnit.Properties.Comments = PropertiesFactory.CreateCommentProperties();
			//		}

			//		paragraphUnit.Properties.Comments.Add(comment);
			//	}

			//	//foreach (var segmentComment in _parser.SegmentComments)
			//	//{
			//	//	var s = paragraphUnit.Source.FirstOrDefault() as ISegment;
			//	//}
			//}

			Output.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
