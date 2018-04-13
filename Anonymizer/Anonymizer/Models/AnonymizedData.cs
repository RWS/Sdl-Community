using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.Anonymizer.Models
{
	public class AnonymizedData
	{
		public string OriginalText { get; set; }
		public string EncryptedText { get; set; }
		public bool IsEncrypted { get; set; }
	}
}
