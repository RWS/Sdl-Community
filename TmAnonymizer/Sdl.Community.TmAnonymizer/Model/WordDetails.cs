using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class WordDetails
	{
		public int Position { get; set; }
		public string Text { get; set; }
		public int Length { get; set; }
		public string NextWord { get; set; }
		public string PreviousWord { get; set; }
	}
}
