using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service.RateIt
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

		public void AddToCurrentSegmentContextData(IStudioDocument activeDocument, TranslationOriginDatum translationOriginDatum)
		{
			var currentSegmentPair = activeDocument.ActiveSegmentPair;
			var translationOrigin = currentSegmentPair.Properties.TranslationOrigin;

			if (translationOrigin is null)
				return;

			translationOrigin.SetMetaData("quality_estimation", translationOriginDatum.QualityEstimation);
			translationOrigin.SetMetaData("model", translationOriginDatum.Model);

			activeDocument.UpdateSegmentPairProperties(currentSegmentPair, currentSegmentPair.Properties);
		}

		public void AddToSegmentContextData()
		{
			foreach (var kvp in GroupedData)
			{
				var currentFilePath = kvp.Key;
				if (currentFilePath == null) continue;

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
			SegmentIds = translationData.Segments.Keys.ToList(),
			TranslationOriginData = translationData.TranslationOriginData
		};

		private string GetFilePath(TranslationData translationData)
		{
			var projectPath = Path.GetDirectoryName(MtCloudApplicationInitializer.ProjectInCreationFilePath) ??
			                  Path.GetDirectoryName(MtCloudApplicationInitializer.GetProjectInProcessing().FilePath);

			var filepath = $@"{projectPath}\{translationData.TargetLanguage}\{translationData.FilePath}.sdlxliff";

			if (File.Exists(filepath)) return filepath;
			filepath =
				Directory.GetFiles($@"{projectPath}\{translationData.TargetLanguage}").FirstOrDefault(
					f => Path.GetFileName(f).Contains(Path.GetFileNameWithoutExtension(translationData.FilePath)));

			return File.Exists(filepath) ? filepath : null;
		}

		private void ResetData()
		{
			Data.Clear();
			MtCloudApplicationInitializer.ProjectInCreationFilePath = null;
		}
	}
}