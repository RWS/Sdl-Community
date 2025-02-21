using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks.Models
{
	public class SegmentLockItem : BindableBase
    {
        private string searchText = string.Empty;
        private bool isRegex = false;
        private bool isCaseSensitive = false;
        private bool wholeWord = false;

        public string SearchText
        {
            get { return searchText; }
            set { SetProperty<string>(ref searchText, value); }
        }

        public bool IsRegex
        {
            get { return isRegex; }
            set { SetProperty<bool>(ref isRegex, value); }
        }

        public bool IsCaseSensitive
        {
            get { return isCaseSensitive; }
            set { SetProperty<bool>(ref isCaseSensitive, value); }
        }

        public bool WholeWord
        {
            get { return wholeWord; }
            set { SetProperty<bool>(ref wholeWord, value); }
        }
    }
}