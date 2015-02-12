using System.Linq;
using Sdl.Community.EmbeddedContentProcessor.Processor;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.EmbeddedContentProcessor
{
    [FileTypeComponentBuilderExtension(Id = "XliffEmbeddedContentExtension_Id",
        Name = "XliffEmbeddedContentExtension_Name",
        Description = "XliffEmbeddedContentExtension_Description",
        OriginalFileType = "XLIFF 1.1-1.2 v 2.0.0.0")]
    public class XliffEmbeddedContentExentsionBuilder : IFileTypeComponentBuilderAdapter
    {
        public IFileTypeComponentBuilder Original { get; set; }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {

            return Original.BuildAbstractGenerator(name);
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            return Original.BuildAdditionalGeneratorsInfo(name);
        }

        public IBilingualDocumentGenerator BuildBilingualGenerator(string name)
        {
            return Original.BuildBilingualGenerator(name);
        }

        public IFileExtractor BuildFileExtractor(string name)
        {
            var originalFileExtractor = Original.BuildFileExtractor(name);
            originalFileExtractor.AddBilingualProcessor(new RegexEmbeddedContentBilingualProcessor(new RegexEvaluator()));

            return originalFileExtractor;
        }

        public IFileGenerator BuildFileGenerator(string name)
        {

            var originalFileGenerator = Original.BuildFileGenerator(name);
            originalFileGenerator.AddBilingualProcessor(new RegexEmbeddedContentBilingualGenerator(new RegexEvaluator()));
            //originalFileGenerator.NativeGenerator.AddProcessor();
            return originalFileGenerator;
        }

        public FileTypeSupport.Framework.NativeApi.INativeFileSniffer BuildFileSniffer(string name)
        {
            return Original.BuildFileSniffer(name);
        }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            var fileTypeInformation = Original.BuildFileTypeInformation(name);

            var settingsPages = fileTypeInformation.WinFormSettingsPageIds.ToList();
            settingsPages.Add("CommunityEmbedddedContentProcessor_Settings");
            fileTypeInformation.WinFormSettingsPageIds = settingsPages.ToArray();
            return fileTypeInformation;
        }

        public IAbstractPreviewApplication BuildPreviewApplication(string name)
        {
            return Original.BuildPreviewApplication(name);
        }

        public IAbstractPreviewControl BuildPreviewControl(string name)
        {
            return Original.BuildPreviewControl(name);
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            return Original.BuildPreviewSetsFactory(name);
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            return Original.BuildQuickTagsFactory(name);
        }

        public IVerifierCollection BuildVerifierCollection(string name)
        {
            return Original.BuildVerifierCollection(name);
        }

        public IFileTypeManager FileTypeManager { get; set; }

        public IFileTypeDefinition FileTypeDefinition { get; set; }
    }
}
