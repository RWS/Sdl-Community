using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Interfaces;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Readers
{
	public class XliffReder : IXliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly XliffSniffer _xliffSupportSniffer;
		private IXliffReader _reader;

		public XliffReder(XliffSniffer xliffSupportSniffer, SegmentBuilder segmentBuilder)
		{
			_xliffSupportSniffer = xliffSupportSniffer;
			_segmentBuilder = segmentBuilder;
		}

		public Xliff ReadXliff(string filePath)
		{	
			var support = _xliffSupportSniffer.GetXliffSupport(filePath);
			if (support == Enumerators.XLIFFSupport.none)
			{
				return null;
			}
			
			_reader = GetXliffReader(support);

			return _reader?.ReadXliff(filePath);
		}

		private IXliffReader GetXliffReader(Enumerators.XLIFFSupport support)
		{
			switch (support)
			{
				case Enumerators.XLIFFSupport.xliff12polyglot:
					return new Xliff12PolyglotReader(_segmentBuilder);
				case Enumerators.XLIFFSupport.xliff12sdl:
					return new Xliff12SdlReader(_segmentBuilder);				
				default:
					return null;
			}
		}
	}
}
