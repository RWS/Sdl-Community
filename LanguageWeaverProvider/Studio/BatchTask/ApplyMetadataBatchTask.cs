using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LanguageWeaverProvider.BatchTask.Model;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace LanguageWeaverProvider.BatchTask
{
	[AutomaticTask(Id = "Apply Language Weaver Metadata",
		Name = "Apply Language Weaver Metadata",
		Description = "Apply metadata for files pre-translated with the Language Weaver plugin.",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
		AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	public class ApplyMetadataBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		IEnumerable<RatedSegment> _ratedSegments;

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var filePath = projectFile.LocalFilePath;
			var fileName = System.IO.Path.GetFileName(filePath);

			if (GetCurrentProjectLanguageCode(projectFile.Language.CultureInfo) is not string languageCode)
			{
				return;
			}

			_ratedSegments = ApplicationInitializer.RatedSegments.Where(seg => seg.TargetLanguageCode == languageCode && seg.FileName.Equals(fileName));
			if (!_ratedSegments.Any())
			{
				return;
			}

			var translationOriginData = new TranslationOriginData()
			{
				Model = _ratedSegments.First().Model,
				RatedSegments = _ratedSegments.ToDictionary(ratedSegment => ratedSegment.SegmentId)
			};

			var metadataTransferObject = new MetadataTransferObject()
			{
				TargetLanguage = languageCode,
				FilePath = filePath,
				SegmentIds = _ratedSegments.Select(x => x.SegmentId).ToList(),
				TranslationOriginData = translationOriginData
			};

			var metadataTransferObjectList = new List<MetadataTransferObject>() { metadataTransferObject };
			var processor = new ApplyMetadataProcessor(metadataTransferObjectList);
			var processorHandler = new BilingualContentHandlerAdapter(processor);

			multiFileConverter?.AddBilingualProcessor(processorHandler);
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			ApplicationInitializer.RatedSegments = ApplicationInitializer.RatedSegments.Except(_ratedSegments).ToList();
			return true;
		}

		private string GetCurrentProjectLanguageCode(CultureInfo cultureInfo)
		{
			var cultureCode = new CultureCode(cultureInfo);
			return DatabaseControl.GetLanguageCode(cultureCode);
		}
	}
}