using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Interfaces;

namespace Sdl.Community.XLIFF.Manager.Converters.XLIFF.Readers
{
	public class XliffReder: IXliffReader
	{
		private readonly IXliffReader _reader;
		
		public XliffReder(Enumerators.XLIFFSupport support)
		{
			switch (support)
			{
				case Enumerators.XLIFFSupport.xliff12polyglot:
					_reader = new Xliff12PolyglotReader();
					break;
				case Enumerators.XLIFFSupport.xliff12sdl:
					_reader = new Xliff12SDLReader();
					break;
			}
		}		
	}
}
