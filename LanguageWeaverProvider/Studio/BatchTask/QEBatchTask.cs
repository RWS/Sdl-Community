using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LanguageWeaverProvider.BatchTask.Model;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace LanguageWeaverProvider.BatchTask
{
	[AutomaticTask(Id = "Language Weaver - Apply QE",
	Name = "Language Weaver - Apply QE",
	Description = "Apply the QE status on segments if a QE model was selected.",
	GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	public class QEBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var filePath = projectFile.LocalFilePath;

			var languageCode = GetCurrentProjectLanguageCode(projectFile.Language.CultureInfo);
			var ratedSegments = RatedSegments.Segments.Where(x => x.TargetLanguageCode == languageCode);

			var translationOriginData = new TranslationOriginData()
			{
				Model = ratedSegments.First().Model,
				RatedSegments = new()
			};

			foreach (var ratedSegment in ratedSegments)
			{
				translationOriginData.RatedSegments[ratedSegment.SegmentId] = ratedSegment;
			}

			var metadataTransferObject = new MetadataTransferObject()
			{
				TargetLanguage = languageCode,
				FilePath = filePath,
				SegmentIds = ratedSegments.Select(x => x.SegmentId).ToList(),
				TranslationOriginData = translationOriginData
			};

			var mtoList = new List<MetadataTransferObject>()
			{
				metadataTransferObject
			};

			var converter = DefaultFileTypeManager.CreateInstance(true).GetConverterToDefaultBilingual(filePath, filePath, null);
			var contentProcessor = new MetaDataProcessor(mtoList);
			converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
			converter?.Parse();
			RatedSegments.Segments = new List<RatedSegment>();
		}

		private string GetCurrentProjectLanguageCode(CultureInfo cultureInfo)
		{
			var cultureCode = new CultureCode(cultureInfo);
			return DatabaseControl.GetLanguageCode(cultureCode);
		}
	}
}