using System;

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
