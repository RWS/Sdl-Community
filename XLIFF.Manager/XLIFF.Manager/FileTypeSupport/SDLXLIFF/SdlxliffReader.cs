using System.Globalization;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;

		public SdlxliffReader(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public Xliff ReadFile(string projectId, string filePath, bool copySourceToTarget)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentReader = new ContentReader(projectId, filePath, false, _segmentBuilder);

			if (copySourceToTarget)
			{
				converter.AddBilingualProcessor(new SourceToTargetCopier(ExistingContentHandling.Preserve));
			}

			converter.AddBilingualProcessor(contentReader);
			

			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;
			

			converter.Parse();
			return contentReader.Xliff;
		}
	}
}
