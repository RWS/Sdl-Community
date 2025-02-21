using Trados.Transcreate.Common;

namespace Trados.Transcreate.Model
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
