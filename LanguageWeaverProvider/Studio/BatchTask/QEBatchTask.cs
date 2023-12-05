using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LanguageWeaverProvider.BatchTask.Model;
using LanguageWeaverProvider.LanguageMappingProvider;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
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
	public class QEBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var filePath = projectFile.LocalFilePath;
			var fileName = System.IO.Path.GetFileName(filePath);

			var languageCode = GetCurrentProjectLanguageCode(projectFile.Language.CultureInfo);
			var ratedSegments = ApplicationInitializer.RatedSegments.Where(seg => seg.TargetLanguageCode == languageCode && seg.FileName.Equals(fileName));

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

			ApplicationInitializer.RatedSegments = ApplicationInitializer.RatedSegments.Except(ratedSegments).ToList();
		}

		private string GetCurrentProjectLanguageCode(CultureInfo cultureInfo)
		{
			var cultureCode = new CultureCode(cultureInfo);
			return DatabaseControl.GetLanguageCode(cultureCode);
		}
	}
}