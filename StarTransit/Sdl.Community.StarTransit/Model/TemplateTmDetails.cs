using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.Model
{
	public class TemplateTmDetails
	{
		public int Penalty { get; set; }
		public bool IsCreatedFromPlugin { get; set; }
		public CultureInfo SourceLanguage { get; set; }
		public CultureInfo TargetLanguage { get; set; }
		public string LocalPath { get; set; }
		public string Name { get; set; }
	}
}
