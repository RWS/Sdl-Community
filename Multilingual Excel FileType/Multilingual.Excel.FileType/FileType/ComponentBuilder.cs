using System;
using System.Linq;
using Autofac;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Preview;
using Multilingual.Excel.FileType.FileType.Processors;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Services;
using Multilingual.Excel.FileType.Services.Entities;
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

namespace Multilingual.Excel.FileType.FileType
{
	[FileTypeComponentBuilder(Id = FiletypeConstants.ComponentBuilderId,
		Name = "MultilingualExcelFileType_ComponentBuilder_Name",
		Description = "MultilingualExcelFileType_ComponentBuilder_Description")]
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
			info.FileTypeDefinitionId = new FileTypeDefinitionId(FiletypeConstants.FileTypeDefinitionId);
			info.FileTypeName = new LocalizableString(FiletypeConstants.FileTypeName);
			info.FileTypeDocumentName = new LocalizableString(FiletypeConstants.FileTypeDocumentName);
			info.FileTypeDocumentsName = new LocalizableString(FiletypeConstants.FileTypeDocumentsName);
			info.Description = new LocalizableString(FiletypeConstants.Description);
			info.FileTypeFrameworkVersion = new Version(FiletypeConstants.FileTypeFrameworkVersion);
			info.FileDialogWildcardExpression = FiletypeConstants.FileDialogWildcardExpression;
			info.DefaultFileExtension = FiletypeConstants.DefaultFileExtension;
			info.Icon = new IconDescriptor(PluginResources.MLExcel);

			info.WinFormSettingsPageIds = new[]
			{
				FiletypeConstants.LanguageMappingId,
				//Constants.CommentMappingId,
				FiletypeConstants.EmbeddedContentId,
				FiletypeConstants.PlaceholdersId,
				"Xml_2_EntitySettings",
				//Constants.EntitiesId,
				//Constants.WriterSettingsId,
				//"TemplateXml_EntitySettings",
				//"Xml_2_WhitespaceNormalization",
				//"Xml_PreviewSettings",
				//"XmlTemplate_VerifierSettings",
				"Xml_QuickTagsSettings"
				//"QuickInserts_Settings"
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
			return new ExcelFileSniffer();
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
			var colorService = new ColorService();
			var excelReader = new ExcelReader(colorService);

			var parser = new BilingualParser(segmentBuilder,
				entityContext,
				entityService,
				excelReader);
			var extractor = FileTypeManager.BuildFileExtractor(parser, this);
			extractor.AddBilingualProcessor(new SegmentRenumberingTrigger());
			extractor.AddBilingualProcessor(new ParagraphToSegmentProcessor(segmentBuilder));
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
			var colorService = new ColorService();
			var excelReader = new ExcelReader(colorService);
			var excelWriter = new ExcelWriter();
			
			var writer = new BilingualWriter(segmentBuilder, entityContext,
				entityService, excelReader, excelWriter);
			var generator = FileTypeManager.BuildFileGenerator(writer);
			//generator.AddBilingualProcessor(new CopySourceToEmptyTargetProcessor(segmentBuilder));
			//generator.AddBilingualProcessor(new CommentsRemovalProcessor());
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

				var xmlReaderFactory = new XmlReaderFactory();
				var fileSystemService = new FileSystemService();
				var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

				var segmentBuilder = new SegmentBuilder(entityService);
				var colorService = new ColorService();
				var excelReader = new ExcelReader(colorService);
				var excelWriter = new ExcelWriter();

				var writer = new BilingualWriter(segmentBuilder, entityContext,
					entityService, excelReader, excelWriter, true, true);
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

				var xmlReaderFactory = new XmlReaderFactory();
				var fileSystemService = new FileSystemService();
				var alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);


				var segmentBuilder = new SegmentBuilder(entityService);
				var colorService = new ColorService();
				var excelReader = new ExcelReader(colorService);
				var excelWriter = new ExcelWriter();

				var writer = new BilingualWriter(segmentBuilder, entityContext,
					entityService, excelReader, excelWriter, true, false);
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

			//BuildExternalSets(previewFactory);
			//BuildInternalSets(previewFactory);

			return previewFactory;
		}

		private static void BuildExternalSets(IPreviewSetsFactory previewFactory)
		{
			var externalStaticPreviewSet = previewFactory.CreatePreviewSet();
			externalStaticPreviewSet.Id = new PreviewSetId("ExternalStaticPreview");
			externalStaticPreviewSet.Name = new LocalizableString("External Excel Preview");

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
			internalStaticPreviewSet.Name = new LocalizableString("Excel Preview");

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
			//if (name == "PreviewControl_InternalNavigablePreview")
			//{
			//	return new InternalPreviewController();
			//}

			return null;
		}

		public IAbstractPreviewApplication BuildPreviewApplication(string name)
		{
			//if (name == "PreviewApplication_ExternalNavigablePreview")
			//{
			//	var previewApplication = new GenericExteralPreviewApplication { ApplicationPath = "" };

			//	return previewApplication;
			//}

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
