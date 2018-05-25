using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class ServerTmBackUp
	{
		public ScheduledServerTranslationMemoryExport ScheduledExport { get; set; }
		public string FilePath { get; set; }
	}
}
