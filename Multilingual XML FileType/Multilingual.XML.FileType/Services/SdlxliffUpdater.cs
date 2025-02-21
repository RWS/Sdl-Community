using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Multilingual.XML.FileType.Services
{
	public class SdlxliffUpdater
	{
		private readonly AbstractBilingualContentProcessor _abstractBilingualContentProcessor;

		public SdlxliffUpdater(AbstractBilingualContentProcessor abstractBilingualContentProcessor)
		{
			_abstractBilingualContentProcessor = abstractBilingualContentProcessor;
		}

		public void UpdateFile(string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			converter.AddBilingualProcessor(_abstractBilingualContentProcessor);
			converter.SynchronizeDocumentProperties();

			converter.Parse();
		}
	}
}
