using LanguageWeaverProvider.BatchTask.Model;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LanguageWeaverProvider.BatchTask
{
    [AutomaticTask(Id = "Apply Language Weaver Metadata",
        Name = "Apply Language Weaver Metadata",
        Description = "Use this batch task when automating your workflows to extract information from Language Weaver, when relying on Quality Estimation or offering Feedback.",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget,
        AllowMultiple = true)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    public class ApplyMetadataBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        private IEnumerable<RatedSegment> _ratedSegments;

        public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            return true;
        }

        public override void TaskComplete()
        {
            base.TaskComplete();
            ApplicationInitializer.RatedSegments = ApplicationInitializer.RatedSegments.Except(_ratedSegments).ToList();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            var filePath = projectFile.LocalFilePath;
            var fileName = System.IO.Path.GetFileName(filePath);

            if (GetCurrentProjectLanguageCode(projectFile.Language.CultureInfo) is not string languageCode)
            {
                return;
            }

            //TODO: Developer named variable FileName, but it stores the FilePath; needs to be revised to avoid confusion
            _ratedSegments = ApplicationInitializer.RatedSegments.Where(seg =>
                seg.TargetLanguageCode == languageCode &&
                System.IO.Path.GetFileName(seg.FileName).Equals(fileName)).ToList();
            if (!_ratedSegments.Any())
            {
                return;
            }

            var translationOriginData = new TranslationOriginData
            {
                Model = _ratedSegments.First().Model,
                RatedSegments = _ratedSegments.ToDictionary(ratedSegment => ratedSegment.SegmentId)
            };

            var metadataTransferObject = new MetadataTransferObject
            {
                TargetLanguage = languageCode,
                FilePath = filePath,
                SegmentIds = _ratedSegments.Select(x => x.SegmentId).ToList(),
                TranslationOriginData = translationOriginData
            };

            var metadataTransferObjectList = new List<MetadataTransferObject>
            {
                metadataTransferObject
            };

            var processor = new ApplyMetadataProcessor(metadataTransferObjectList);
            var processorHandler = new BilingualContentHandlerAdapter(processor);

            multiFileConverter?.AddBilingualProcessor(processorHandler);
        }

        private string GetCurrentProjectLanguageCode(CultureInfo cultureInfo)
        {
            var cultureCode = new CultureCode(cultureInfo);
            return DatabaseControl.GetLanguageCode(cultureCode);
        }
    }
}