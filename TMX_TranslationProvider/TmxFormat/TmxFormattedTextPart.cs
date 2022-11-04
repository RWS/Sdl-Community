using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_TranslationProvider.TmxFormat
{
	public class TmxFormattedTextPart
	{
		public string Text;
		// in case it's an xml format node
        public string FormatType = "";
		public List<(string, string)> FormatAttributes = new List<(string, string)>();
    }
}
