using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ExportOptions : BaseModel
	{
		public ExportOptions()
		{
			XliffSupport = Enumerators.XLIFFSupport.xliff12polyglot;
			IncludeTranslations = false;
			CopySourceToTarget = false;	
		}

		public Enumerators.XLIFFSupport XliffSupport { get; set; }

	    public bool IncludeTranslations { get; set; }

	    public bool CopySourceToTarget { get; set; }
	}
}
