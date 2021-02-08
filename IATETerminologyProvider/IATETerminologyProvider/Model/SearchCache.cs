using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SearchCache
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public string SourceText { get; set; }
		public string TargetLanguageName { get; set; }
		public string QueryString { get; set; }
		public string SearchResultsString { get; set; }
	}
}
