using System.Collections.Generic;
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
		private IEnumerable<IGrouping<string, MetadataTransferObject>> _dataDictionary;

		private List<TranslationData> Data { get; } = new();

		private IEnumerable<IGrouping<string, MetadataTransferObject>> GroupedData
			=> _dataDictionary ??= Data.Select(ConvertToSdlMtData).GroupBy(mtData => mtData.FilePath);

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
				var manager = DefaultFileTypeManager.CreateInstance(true);
				var converter = manager.GetConverterToDefaultBilingual(kvp.Key, kvp.Key, null);

				var contentProcessor = new MetaDataProcessor(kvp.ToList());
				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));

				converter.Parse();
			}

			ResetData();
		}

		private void ResetData()
		{
			Data.Clear();
			_dataDictionary = null;
		}

		private MetadataTransferObject ConvertToSdlMtData(TranslationData translationData) => new()
		{
			FilePath = translationData.FilePath,
			SegmentIds = translationData.SegmentIds,
			TranslationOriginInformation = translationData.TranslationOriginInformation
		};
	}
}