using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMLifting.Helpers
{
	public class ReindexOperationStatus
	{
		public int RowIndex { get; set; }
		public ScheduledReindexOperation ReindexOperation { get; set; }
	}
}
