namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SearchCache
	{
		public int Id { get; set; }
		
		public string SourceText { get; set; }
		
		public string TargetLanguage { get; set; }
		
		public string QueryString { get; set; }
		
		public string SearchResultsString { get; set; }
	}
}
