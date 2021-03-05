using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SegmentMetadataCreator : ISegmentMetadataCreator
	{
		private readonly IFileTypeManager _manager;
		private IEnumerable<IGrouping<string, MetadataTransferObject>> _groupedData;

		public SegmentMetadataCreator()
		{
			_manager = DefaultFileTypeManager.CreateInstance(true);
		}

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

				var converter = _manager.GetConverterToDefaultBilingual(currentFilePath, currentFilePath, null);
				var contentProcessor = new MetaDataProcessor(kvp.ToList());
				converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter?.Parse();
			}
			ResetData();
		}

		private MetadataTransferObject ConvertToSdlMtData(TranslationData translationData) => new()
		{
			FilePath = GetFilePath(translationData),
			SegmentIds = translationData.SegmentIds,
			TranslationOriginInformation = translationData.TranslationOriginInformation
		};

		private string GetFilePath(TranslationData translationData)
		{
			if (File.Exists(translationData.FilePath)) return translationData.FilePath;

			var projectPath = Path.GetDirectoryName(MtCloudApplicationInitializer.ProjectsController.CurrentProject.FilePath);
			return $@"{projectPath}\{translationData.TargetLanguage}\{translationData.FilePath}.sdlxliff";
		}

		private void ResetData()
		{
			Data.Clear();
		}
	}
}