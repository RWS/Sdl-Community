using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentMetadataCreator : ISegmentMetadataCreator
	{
		private IEnumerable<IGrouping<string, MetadataTransferObject>> _groupedData;
		private List<TranslationData> Data { get; } = new();

		private IEnumerable<IGrouping<string, MetadataTransferObject>> GroupedData
			=> _groupedData ??= Data.Select(ConvertToSdlMtData).GroupBy(mtData => mtData.FilePath);

		public void AddTargetSegmentMetaData(TranslationData translationData)
		{
			Data.Add(translationData);
		}

		public void AddToCurrentSegmentContextData(Document activeDocument, TranslationOriginInformation translationOriginInformation)
		{
			var currentSegmentPair = activeDocument.ActiveSegmentPair;
			var translationOrigin = currentSegmentPair.Properties.TranslationOrigin;

			if (translationOrigin is null)
				return;

			translationOrigin.SetMetaData("quality_estimation", translationOriginInformation.QualityEstimation);
			translationOrigin.SetMetaData("model", translationOriginInformation.Model);

			activeDocument.UpdateSegmentPairProperties(currentSegmentPair, currentSegmentPair.Properties);
		}

		public void AddToSegmentContextData()
		{
			foreach (var kvp in GroupedData)
			{
				var currentFilePath = kvp.Key;
				if (!File.Exists(currentFilePath)) return;

				var manager = DefaultFileTypeManager.CreateInstance(true);
				var converter = manager.GetConverterToDefaultBilingual(currentFilePath, currentFilePath, null);
				var contentProcessor = new MetaDataProcessor(kvp.ToList());
				converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter?.Parse();

			}
			ResetData();
		}

		private MetadataTransferObject ConvertToSdlMtData(TranslationData translationData) => new()
		{
			FilePath = translationData.FilePath,
			SegmentIds = translationData.SegmentIds,
			TranslationOriginInformation = translationData.TranslationOriginInformation
		};

		private void ResetData()
		{
			Data.Clear();
		}
	}
}