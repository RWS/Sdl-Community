using System;
using System.Linq;
using Autofac;
using Multilingual.XML.FileType.FileType.Preview;
using Multilingual.XML.FileType.FileType.Processors;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.Services;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Core.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.FileTypeSupport.Framework.PreviewControls;

namespace Multilingual.XML.FileType.FileType
{
	[FileTypeComponentBuilder(Id = Constants.ComponentBuilderId,
		Name = "MultilingualXMLFileType_ComponentBuilder_Name",
		Description = "MultilingualXMLFileType_ComponentBuilder_Description")]
	public class ComponentBuilder : IFileTypeComponentBuilder,
		IDefaultFileTypeSettingsProvider,
		IFileTypeSettingsConverterComponentBuilder,
		IFileTypeDefinitionAware
	{
		public IFileTypeDefinition FileTypeDefinition { get; set; }

		internal Func<IContainer> ContainerFactory { get; set; }

		public IFileTypeManager FileTypeManager { get; set; }

		public IFileTypeInformation BuildFileTypeInformation(string name)
		{
			var info = FileTypeManager.BuildFileTypeInformation();
			info.Enabled = true;
			info.Hidden = false;
			info.FileTypeDefinitionId = new FileTypeDefinitionId(Constants.FileTypeDefinitionId);
			info.FileTypeName = new LocalizableString(Constants.FileTypeName);
			info.FileTypeDocumentName = new LocalizableString(Constants.FileTypeDocumentName);
			info.FileTypeDocumentsName = new LocalizableString(Constants.FileTypeDocumentsName);
			info.Description = new LocalizableString(Constants.Description);
			info.FileTypeFrameworkVersion = new Version(Constants.FileTypeFrameworkVersion);
			info.FileDialogWildcardExpression = Constants.FileDialogWildcardExpression;
			info.DefaultFileExtension = Constants.DefaultFileExtension;
			info.Icon = new IconDescriptor(PluginResources.mlXML);

			info.WinFormSettingsPageIds = new[]
			{
				Constants.LanguageMappingId,
				//Constants.CommentMappingId,
				Constants.EmbeddedContentId,
				Constants.PlaceholdersId,
				//Constants.WriterId,
				"Xml_2_EntitySettings",
				//Constants.EntitiesId,
				//Constants.WriterSettingsId,
				//"TemplateXml_EntitySettings",
				//"Xml_2_WhitespaceNormalization",
				//"Xml_PreviewSettings",
				//"XmlTemplate_VerifierSettings",
				"Xml_QuickTagsSettings"
			};

			return info;
		}

		public IQuickTagsFactory BuildQuickTagsFactory(string name)
		{
			var tagMaps = QuickTagBuilder.BuildStandardQuickTags();
			var quickTags = tagMaps.Select(tagMap => tagMap.QuickTag).ToList();

			var quickTagsFactory = FileTypeManager.BuildQuickTagsFactory();
			quickTagsFactory.GetQuickTags(null).SetStandardQuickTags(quickTags);

			return quickTagsFactory;
		}

		public INativeFileSniffer BuildFileSniffer(string name)
		{
			var xmlReaderFactory = new XmlReaderFactory();
			var namespaceUri = new DefaultNamespaceHelper(xmlReaderFactory);
			var sniffer = new XmlFileSniffer(namespaceUri, xmlReaderFactory);

			return sniffer;
		}

		public IFileExtractor BuildFileExtractor(string name)
		{
			var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			var entityContext = new EntityContext();
			var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
			var entityMarkerConversionService = new EntityMarkerConversionService();
			var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

			var segmentBuilder = new SegmentBuilder(entityService);
			var xmlReaderFactory = new XmlReaderFactory();
			var sourceFileDefaultNamespaceHelper = new DefaultNamespaceHelper(xmlReaderFactory);
			var fileSystemService = new FileSystemService();
			var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

			var parser = new BilingualParser(segmentBuilder,
				sourceFileDefaultNamespaceHelper,
				xmlReaderFactory,
				entityContext,
				entityService,
				alternativeInputFileGenerator,
				fileSystemService);
			var extractor = FileTypeManager.BuildFileExtractor(parser, this);
			extractor.AddBilingualProcessor(new SegmentRenumberingTrigger());
			//extractor.AddBilingualProcessor(new ParagraphToSegmentProcessor(segmentBuilder));
			extractor.AddBilingualProcessor(new NormalizeEmbeddedContentContextProcessor(segmentBuilder));
			extractor.AddBilingualProcessor(new PlaceholdersProcessor(segmentBuilder, parser));
			//extractor.AddBilingualProcessor(new CommentsProcessor(segmentBuilder, parser));

			return extractor;
		}

		public IFileGenerator BuildFileGenerator(string name)
		{
			var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			var entityContext = new EntityContext();
			var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
			var entityMarkerConversionService = new EntityMarkerConversionService();
			var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

			var segmentBuilder = new SegmentBuilder(entityService);
			var xmlReaderFactory = new XmlReaderFactory();
			var fileSystemService = new FileSystemService();
			var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

			var namespaceUri = new DefaultNamespaceHelper(xmlReaderFactory);

			var writer = new BilingualWriter(segmentBuilder, namespaceUri, xmlReaderFactory, entityContext,
				entityService,
				alternativeInputFileGenerator,
				fileSystemService);
			var generator = FileTypeManager.BuildFileGenerator(writer);
			//generator.AddBilingualProcessor(new CopySourceToEmptyTargetProcessor(segmentBuilder));
			//generator.NativeGenerator.AddProcessor(new FeedbackRemovalProcessor());

			return generator;
		}

		public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
		{
			return null;
		}

		public IAbstractGenerator BuildAbstractGenerator(string name)
		{
			if (name == "Generator_SourceStaticPreview")
			{
				var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
				var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

				var entityContext = new EntityContext();
				var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
				var entityMarkerConversionService = new EntityMarkerConversionService();
				var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

				var segmentBuilder = new SegmentBuilder(entityService);
				var xmlReaderFactory = new XmlReaderFactory();
				var fileSystemService = new FileSystemService();
				var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

				var namespaceUri = new DefaultNamespaceHelper(xmlReaderFactory);

				var writer = new BilingualWriter(segmentBuilder, namespaceUri, xmlReaderFactory, entityContext,
					entityService,
					alternativeInputFileGenerator,
					fileSystemService, true, true);
				var abstractGenerator = FileTypeManager.BuildFileGenerator(writer);

				var internalPostTweker = new InternalPreviewFileTweaker(xmlReaderFactory, alternativeInputFileGenerator, entityMarkerConversionService);
				abstractGenerator.AddFileTweaker(internalPostTweker);

				return abstractGenerator;
			}

			if (name == "Generator_TargetStaticPreview")
			{
				var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
				var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

				var entityContext = new EntityContext();
				var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
				var entityMarkerConversionService = new EntityMarkerConversionService();
				var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

				var segmentBuilder = new SegmentBuilder(entityService);
				var xmlReaderFactory = new XmlReaderFactory();
				var fileSystemService = new FileSystemService();
				var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

				var namespaceUri = new DefaultNamespaceHelper(xmlReaderFactory);

				var writer = new BilingualWriter(segmentBuilder, namespaceUri, xmlReaderFactory, entityContext,
					entityService,
					alternativeInputFileGenerator,
					fileSystemService, true, false);
				var abstractGenerator = FileTypeManager.BuildFileGenerator(writer);
				var internalPostTweker = new InternalPreviewFileTweaker(xmlReaderFactory, alternativeInputFileGenerator, entityMarkerConversionService);
				abstractGenerator.AddFileTweaker(internalPostTweker);

				return abstractGenerator;
			}

			return null;
		}

		public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
		{
			var previewFactory = FileTypeManager.BuildPreviewSetsFactory();

			BuildExternalSets(previewFactory);
			BuildInternalSets(previewFactory);

			return previewFactory;
		}

		private static void BuildExternalSets(IPreviewSetsFactory previewFactory)
		{
			var externalStaticPreviewSet = previewFactory.CreatePreviewSet();
			externalStaticPreviewSet.Id = new PreviewSetId("ExternalStaticPreview");
			externalStaticPreviewSet.Name = new LocalizableString("External XML Preview");

			if (previewFactory.CreatePreviewType<IApplicationPreviewType>() is IApplicationPreviewType sourceExternalPreviewType
			)
			{
				sourceExternalPreviewType.TargetGeneratorId = new GeneratorId("SourceStaticPreview");
				sourceExternalPreviewType.SingleFilePreviewApplicationId = new PreviewApplicationId("ExternalNavigablePreview");
				externalStaticPreviewSet.Source = sourceExternalPreviewType;
			}

			if (previewFactory.CreatePreviewType<IApplicationPreviewType>() is IApplicationPreviewType targetExternalPreviewType
			)
			{
				targetExternalPreviewType.TargetGeneratorId = new GeneratorId("TargetStaticPreview");
				targetExternalPreviewType.SingleFilePreviewApplicationId = new PreviewApplicationId("ExternalNavigablePreview");
				externalStaticPreviewSet.Target = targetExternalPreviewType;
			}

			previewFactory.GetPreviewSets(null).Add(externalStaticPreviewSet);
		}

		private static void BuildInternalSets(IPreviewSetsFactory previewFactory)
		{
			var internalStaticPreviewSet = previewFactory.CreatePreviewSet();
			internalStaticPreviewSet.Id = new PreviewSetId("InternalStaticPreview");
			internalStaticPreviewSet.Name = new LocalizableString("XML Preview");

			if (previewFactory.CreatePreviewType<IControlPreviewType>() is IControlPreviewType sourceControlPreviewType1)
			{
				sourceControlPreviewType1.TargetGeneratorId = new GeneratorId("SourceStaticPreview");
				sourceControlPreviewType1.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
				internalStaticPreviewSet.Source = sourceControlPreviewType1;
			}

			if (previewFactory.CreatePreviewType<IControlPreviewType>() is IControlPreviewType targetControlPreviewType1)
			{
				targetControlPreviewType1.TargetGeneratorId = new GeneratorId("TargetStaticPreview");
				targetControlPreviewType1.SingleFilePreviewControlId = new PreviewControlId("InternalNavigablePreview");
				internalStaticPreviewSet.Target = targetControlPreviewType1;
			}

			previewFactory.GetPreviewSets(null).Add(internalStaticPreviewSet);
		}


		public IAbstractPreviewControl BuildPreviewControl(string name)
		{
			if (name == "PreviewControl_InternalNavigablePreview")
			{
				return new InternalPreviewController();
			}

			return null;
		}

		public IAbstractPreviewApplication BuildPreviewApplication(string name)
		{
			if (name == "PreviewApplication_ExternalNavigablePreview")
			{
				var previewApplication = new GenericExteralPreviewApplication { ApplicationPath = "" };

				return previewApplication;
			}

			return null;
		}

		public IBilingualDocumentGenerator BuildBilingualGenerator(string name)
		{
			return null;
		}

		public IVerifierCollection BuildVerifierCollection(string name)
		{
			return FileTypeManager.BuildVerifierCollection();
		}

		public IFileTypeSettingsConverter BuildFileTypeSettingsConverter(string name)
		{
			return new GenericFileTypeSettingsConverter(
				//SettingsFormatConverter.ConvertSettings<EmbeddedContentProcessingSettings>,
				SettingsFormatConverter.ConvertSettings<PlaceholderPatternSettings>,
				SettingsFormatConverter.ConvertSettings<QuickInsertsSettings>);
		}

		public void PopulateDefaultSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
			var settings = new FileTypeSettingsBase[]
			{
				//new EmbeddedContentProcessingSettings(),
				new PlaceholderPatternSettings(),
				new QuickInsertsSettings()
			};

			foreach (var fileTypeSettingsBase in settings)
			{
				fileTypeSettingsBase.SaveDefaultsToSettingsBundle(settingsBundle, fileTypeConfigurationId);
			}
		}
	}
}
