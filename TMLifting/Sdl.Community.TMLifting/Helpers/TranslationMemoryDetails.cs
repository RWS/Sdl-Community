using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMLifting.Helpers
{
	public class TranslationMemoryDetails
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedOn { get; set; }
		public string Location { get; set; }
		public bool ShouldRecomputeStatistics { get; set; }
		public DateTime? LastReIndexDate { get; set; }
		public int? LastReIndexSize { get; set; }
		public string Status { get; set; }
	}
}
