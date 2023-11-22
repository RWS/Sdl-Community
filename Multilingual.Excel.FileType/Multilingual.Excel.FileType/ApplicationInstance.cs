using System.Windows.Forms;
using Multilingual.Excel.FileType.FileType.Processors;
using Multilingual.Excel.FileType.Services.Entities;
using Multilingual.Excel.FileType.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Multilingual.Excel.FileType
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		private DocumentProcessor _documentProcessor;

		public void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();

			var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			var entityContext = new EntityContext();
			var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
			var entityMarkerConversionService = new EntityMarkerConversionService();
			var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

			var segmentBuilder = new SegmentBuilder(entityService);

			_documentProcessor = new DocumentProcessor(editorController, segmentBuilder);

			// Setup Logger
		}

		public static Form GetActiveForm()
		{
			var allForms = Application.OpenForms;
			var activeForm = allForms[allForms.Count - 1];
			foreach (Form form in allForms)
			{
				if (form.GetType().Name == "StudioWindowForm")
				{
					activeForm = form;
					break;
				}
			}

			return activeForm;
		}
	}
}
