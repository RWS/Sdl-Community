namespace TMX_TranslationProvider.Search.SearchSegment
{
	// represents the text we're searching for, optimized for comparison
	public class TextSegment
	{
		private string _originalText;
		public string OriginalText => _originalText;

		// FIXME later -> formatting

		public TextSegment(string originalText)
		{
			_originalText = originalText;
		}


		// returns a number 0-100 , 100=exact, 0= none
		public static int CompareScore(TextSegment a, TextSegment b)
		{
			// care about full word misses - create 2 dictionaries for the sourceText and unit.sourcelanguage
			// + care about numbers
			// + care about punctuation

			return 0;
		}
	}
}
