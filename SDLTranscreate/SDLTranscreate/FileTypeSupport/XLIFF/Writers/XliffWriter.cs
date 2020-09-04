using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.Community.Transcreate.Interfaces;

namespace Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers
{
	public class XliffWriter: IXliffWriter
	{
		private readonly IXliffWriter _writer;
		
		public XliffWriter(Enumerators.XLIFFSupport support)
		{
			switch (support)
			{
				case Enumerators.XLIFFSupport.xliff12polyglot:
					_writer = new Xliff12PolyglotWriter();
					break;
				case Enumerators.XLIFFSupport.xliff12sdl:
					_writer = new Xliff12SDLWriter();
					break;
			}
		}

		public bool WriteFile(Xliff xliff, string outputFilePath, bool includeTranslations)
		{
			if (_writer == null)
			{
				return false;
			}

			return _writer.WriteFile(xliff, outputFilePath, includeTranslations);			
		}
	}
}
