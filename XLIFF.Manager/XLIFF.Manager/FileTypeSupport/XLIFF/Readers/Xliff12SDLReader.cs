using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Interfaces;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers
{
	public class Xliff12SdlReader : IXliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;

		public Xliff12SdlReader(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;
		}

		public Xliff ReadXliff(string filePath)
		{
			
			// TODO; fully implement the reader
			return new Xliff();
		}
	}
}
