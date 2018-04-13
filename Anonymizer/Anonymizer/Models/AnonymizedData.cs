using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.Anonymizer.Models
{
	public class AnonymizedData
	{
		public int PositionInOriginalText { get; set; }
		public string MatchText { get; set; }
		public string EncryptedText { get; set; }
	}
}
