using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
    public class XLIFFSupportModel
    {
		public Enumerators.XLIFFSupport SupportType { get; set; }

		public string Name { get; set; }

	    public override string ToString()
	    {
		    return Name;
	    }
    }
}
