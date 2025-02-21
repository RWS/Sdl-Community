using System.Linq;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class NormalizeEmbeddedContentContextProcessor : AbstractBilingualContentProcessor
	{
		private IContextProperties _contextProperties;
		private ICommentProperties _commentProperties;

		private readonly SegmentBuilder _segmentBuilder;

		public NormalizeEmbeddedContentContextProcessor(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var multilingualParagraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a => a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
			if (multilingualParagraphUnitContext != null)
			{
				_contextProperties = paragraphUnit.Properties.Contexts;
				_commentProperties = paragraphUnit.Properties.Comments;
			}

			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			if (paragraphUnit.Properties.Contexts == null)
			{
				var contextProperties = ItemFactory.PropertiesFactory.CreateContextProperties();
				paragraphUnit.Properties.Contexts = contextProperties;
			}

			if (multilingualParagraphUnitContext != null)
			{
				var paragraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a => a.ContextType == StandardContextTypes.Paragraph);
				if (paragraphUnitContext == null)
				{
					var paragraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
					paragraphUnit.Properties.Contexts?.Contexts.Add(paragraphContextInfo);
				}
			}
			else
			{
				foreach (var contextProperty in _contextProperties.Contexts)
				{
					paragraphUnit.Properties.Contexts.Contexts.Add(contextProperty);
				}

				if (_commentProperties?.Count > 0 && paragraphUnit.Source.Any())
				{
					foreach (var commentPropertiesComment in _commentProperties.Comments)
					{
						var newComment = _segmentBuilder.CreateCommentContainer(commentPropertiesComment.Text, commentPropertiesComment.Author,
							commentPropertiesComment.Severity, commentPropertiesComment.Date, commentPropertiesComment.Version) as IAbstractMarkupDataContainer;

						paragraphUnit.Source.MoveAllItemsTo(newComment);
						paragraphUnit.Source.Add(newComment as IAbstractMarkupData);
					}

					_commentProperties = _segmentBuilder.PropertiesFactory.CreateCommentProperties();
				}
			}

			PutBackSoftReturns(paragraphUnit);

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private static void PutBackSoftReturns(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit?.Source == null)
			{
				return;
			}

			foreach (var item in paragraphUnit.Source.AllSubItems)
			{
				if (item is IText textItem)
				{
					if (textItem.Properties?.Text != null)
					{
						textItem.Properties.Text =
							textItem.Properties.Text?.Replace(EntityConstants.SoftReturnEntityRefEscape, "\n");
					}
				}
			}
		}
	}
}
