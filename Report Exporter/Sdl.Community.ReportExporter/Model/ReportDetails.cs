using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReportExporter.Model
{
	public class ReportDetails
	{
		public string ProjectName { get; set; }
		public Dictionary<LanguageDirection,bool> LanguagesForPoject { get; set; }
	}
}
