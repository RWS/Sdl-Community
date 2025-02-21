using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Versions.Structures
{
	public class VersionDetails
	{
		public string OriginalFileLocation { get; set; }
		public string ModifiedFileLocation { get; set; }
		public LanguageProperty SourceLanguage { get; set; }
		public List<LanguageProperty> TargetLanguages { get; set; }
	}
}
