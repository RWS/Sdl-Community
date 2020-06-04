using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffWriter
	{
		private readonly SegmentBuilder _segmentBuilder;

		public SdlxliffWriter(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public bool UpdateFile(Xliff xliff, string filePath)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, filePath + ".out.sdlxliff", null);

			var contentWriter = new ContentWriter(xliff, _segmentBuilder);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			//var writer = new XliffFileWriter(filePath);
			//converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(writer));
			converter.Parse();

			return true;
		}
	}
}
