using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SearchResultModel : SearchResult
	{
		public string ItemId { get; set; }
		public string Definition { get; set; }
		public string Domain { get; set; }
		public string Subdomain { get; set; }
		public string TermType { get; set; }
		public int DisplayOrder { get; set; }
		public int Evaluation { get; set; }
	}
}