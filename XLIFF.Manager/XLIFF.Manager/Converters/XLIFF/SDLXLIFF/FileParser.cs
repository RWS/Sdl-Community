using System.Globalization;
using Sdl.Community.XLIFF.Manager.Converters.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Converters.XLIFF.SDLXLIFF
{
	internal class FileParser
	{
		public CultureInfo SourceLanguage { get; private set; }
		public CultureInfo TargetLanguage { get; private set; }

		internal Xliff ParseFile(string projectId, string filePath)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentProcessor = new ContentProcessor(projectId, filePath, false);

			converter.AddBilingualProcessor(new SourceToTargetCopier(ExistingContentHandling.Preserve));
			converter.AddBilingualProcessor(contentProcessor);

			SourceLanguage = contentProcessor.SourceLanguage;
			TargetLanguage = contentProcessor.TargetLanguage;

			converter.Progress += Converter_Progress;
			converter.Parse();

			return contentProcessor.Xliff;
		}

		private void Converter_Progress(object sender, Sdl.FileTypeSupport.Framework.IntegrationApi.BatchProgressEventArgs e)
		{
			
		}
	}
}
