using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class CustomField
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public List<Details> Details { get; set; }
	}
}
