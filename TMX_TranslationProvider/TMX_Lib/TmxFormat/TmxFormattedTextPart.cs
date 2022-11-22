using System.Collections.Generic;

namespace TMX_Lib.TmxFormat
{
	public class TmxFormattedTextPart
	{
		public string Text;
		// in case it's an xml format node
        public string FormatType = "";
		public List<(string, string)> FormatAttributes = new List<(string, string)>();
    }
}
