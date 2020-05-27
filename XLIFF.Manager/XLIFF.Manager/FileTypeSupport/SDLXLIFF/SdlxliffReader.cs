using System.Globalization;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		public CultureInfo SourceLanguage { get; private set; }
		public CultureInfo TargetLanguage { get; private set; }

		public Xliff ReadFile(string projectId, string filePath, bool copySourceToTarget)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentProcessor = new ContentProcessor(projectId, filePath, false);

			if (copySourceToTarget)
			{
				converter.AddBilingualProcessor(new SourceToTargetCopier(ExistingContentHandling.Preserve));
			}

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
