using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
    public class XLIFFSupportItem
    {
		public Enumerators.XLIFFSupport SupportType { get; set; }

		public string Name { get; set; }

	    public override string ToString()
	    {
		    return Name;
	    }
    }
}
