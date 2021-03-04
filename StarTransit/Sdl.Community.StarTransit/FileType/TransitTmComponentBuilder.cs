using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.FileType
{
	[FileTypeComponentBuilder(Id = "TransitTm_FilterComponentBuilderExtension_Id",
		Name = "TransitTm_FilterComponentBuilderExtension_Name",
		Description = "TransitTm_FilterComponentBuilderExtension_Description")]
	public class TransitTmComponentBuilder : IFileTypeComponentBuilder
	{
		public IFileTypeDefinition FileTypeDefinition { get; set; }
		public IFileTypeInformation BuildFileTypeInformation(string name)
		{
			var fileTypeDetails = new FiletypeDetails();
			var info = FileTypeManager.BuildFileTypeInformation();

			info.FileTypeDefinitionId = new FileTypeDefinitionId("Transit TM File Type 1.0.0.0");
			info.FileTypeName = new LocalizableString("Transit TMs bilingual documents");
			info.FileTypeDocumentName = new LocalizableString("Transit TMs");
			info.FileTypeDocumentsName = new LocalizableString("Transit TMs");
			info.Description = new LocalizableString("Documents, generated from Transit TMs. Documents may be untranslated, partially or fully translated.");
			info.FileDialogWildcardExpression = fileTypeDetails.FileExtensions;
			info.DefaultFileExtension = fileTypeDetails.FileExtensions;
			info.Icon = new IconDescriptor(PluginResources.bil);
			info.Enabled = true;

			return info;
		}

		public IQuickTagsFactory BuildQuickTagsFactory(string name)
		{
			return FileTypeManager.BuildQuickTagsFactory();
		}

		public INativeFileSniffer BuildFileSniffer(string name)
		{
			return new TmSniffer();
		}

		public IFileExtractor BuildFileExtractor(string name)
		{
			return FileTypeManager.BuildFileExtractor(new TmFileParser(), this);
		}

		public IFileGenerator BuildFileGenerator(string name)
		{
			//TODO: Implement specific writer for tm if necessary
			return FileTypeManager.BuildFileGenerator(new TransitWriter());
		}

		public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
		{
			return null;
		}

		public IAbstractGenerator BuildAbstractGenerator(string name)
		{
			return null;
		}

		public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
		{
			return FileTypeManager.BuildPreviewSetsFactory();
		}

		public IAbstractPreviewControl BuildPreviewControl(string name)
		{
			return  null;
		}

		public IAbstractPreviewApplication BuildPreviewApplication(string name)
		{
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

		public IFileTypeManager FileTypeManager { get; set; }
	}
}
