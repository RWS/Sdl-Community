using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffWriter
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly IFileTypeManager _fileTypeManager;

		public SdlxliffWriter(IFileTypeManager fileTypeManager, SegmentBuilder segmentBuilder)
		{
			_fileTypeManager = fileTypeManager;
			_segmentBuilder = segmentBuilder;
		}

		public bool UpdateFile(Xliff xliff, string filePath)
		{			
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePath, filePath + ".out.sdlxliff", null);

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
