using dtSearch.Engine;

namespace Sdl.Community.DtSearch4Studio.Provider.Model
{
	public class WordItem
	{
		#region Public Properties
		public string Word { get; set; }
		public string Detail { get; set; }

		//HitCount represents how many times the word was used
		public int HitCount { get; set; }
		#endregion

		#region public methods
		public void MakeFromWordListBuilder(WordListBuilder wordListBuilder, int iItem)
		{
			Word = wordListBuilder.GetNthWord(iItem);
			HitCount = wordListBuilder.GetNthWordCount(iItem);
			Detail = " " + wordListBuilder.GetNthWordCount(iItem) + " hits in " + wordListBuilder.GetNthWordDocCount(iItem) + " documents";
		}
		#endregion
	}
}