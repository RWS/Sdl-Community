using System;
using System.Linq;
using Multilingual.XML.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;

namespace Multilingual.XML.FileType.FileType.Processors
{
	public class NormalizeEmbeddedContentContextProcessor : AbstractBilingualContentProcessor
	{
		private bool _isCDATA;

		private int _paragraphIndex;

		private readonly SegmentBuilder _segmentBuilder;

		public NormalizeEmbeddedContentContextProcessor(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var multilingualParagraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a => a.ContextType == Constants.MultilingualParagraphUnit);
			if (multilingualParagraphUnitContext != null)
			{
				_paragraphIndex = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(Constants.MultilingualParagraphUnitIndex));
				_isCDATA = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(Constants.IsCDATA));
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

			var paragraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a => a.ContextType == StandardContextTypes.Paragraph);
			if (paragraphUnitContext == null)
			{
				var paragraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
				paragraphUnit.Properties.Contexts.Contexts.Add(paragraphContextInfo);
			}

			if (multilingualParagraphUnitContext == null)
			{
				var multilingualParagraphContextInfo = _segmentBuilder.CreateMultilingualParagraphContextInfo();
				paragraphUnit.Properties.Contexts.Contexts.Add(multilingualParagraphContextInfo);

				multilingualParagraphContextInfo.SetMetaData(Constants.MultilingualParagraphUnitIndex, _paragraphIndex.ToString());
				multilingualParagraphContextInfo.SetMetaData(Constants.IsCDATA, _isCDATA.ToString());
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
