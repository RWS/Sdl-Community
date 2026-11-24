using System.IO;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace XliffXsltPreviewSample
{
	[FileTypeComponentBuilderExtension(Id = "XliffXsltPreviewSample_Extension", Name = "Extension_Name", Description = "Extension_Description", OriginalFileType = "XLIFF 1.1-1.2 v 2.0.0.0")]
	public class XliffXsltPreviewAdapter : IFileTypeComponentBuilderAdapter
	{
		private const string XsltPreviewId = "XsltPreview";

		public IFileTypeComponentBuilder Original { get; set; }

		public IFileTypeManager FileTypeManager { get; set; }

		public IFileTypeDefinition FileTypeDefinition { get; set; }

		public IFileTypeInformation BuildFileTypeInformation(string name) => Original.BuildFileTypeInformation(name);

		public IQuickTagsFactory BuildQuickTagsFactory(string name) => Original.BuildQuickTagsFactory(name);

		public INativeFileSniffer BuildFileSniffer(string name) => Original.BuildFileSniffer(name);

		public IFileExtractor BuildFileExtractor(string name) => Original.BuildFileExtractor(name);

		public IFileGenerator BuildFileGenerator(string name) => Original.BuildFileGenerator(name);

		public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name) => Original.BuildAdditionalGeneratorsInfo(name);

		public IAbstractGenerator BuildAbstractGenerator(string name)
		{
			if (name != "Generator_" + XsltPreviewId)
			{
				return Original.BuildAbstractGenerator(name);
			}
			var generator = Original.BuildFileGenerator(name);
			generator.AddBilingualProcessor(new SegmentMarker());
			generator.AddFileTweaker(new XsltPreviewTweaker(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "preview.xslt")));
			return generator;
		}

		public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
		{
			var factory = Original.BuildPreviewSetsFactory(name);
			var target = (IControlPreviewType)factory.CreatePreviewType<IControlPreviewType>();
			target.TargetGeneratorId = new GeneratorId(XsltPreviewId);
			target.SingleFilePreviewControlId = new PreviewControlId("InternalPreview");
			var set = factory.CreatePreviewSet();
			set.Id = new PreviewSetId(XsltPreviewId);
			set.Name = new LocalizableString("XSLT");
			set.Target = target;
			factory.GetPreviewSets(null).Add(set);
			return factory;
		}

#pragma warning disable CS0618
		public IAbstractPreviewControl BuildPreviewControl(string name) => Original.BuildPreviewControl(name);

		public IAbstractPreviewApplication BuildPreviewApplication(string name) => Original.BuildPreviewApplication(name);
#pragma warning restore CS0618

		public IBilingualDocumentGenerator BuildBilingualGenerator(string name) => Original.BuildBilingualGenerator(name);

		public IVerifierCollection BuildVerifierCollection(string name) => Original.BuildVerifierCollection(name);
	}
}
