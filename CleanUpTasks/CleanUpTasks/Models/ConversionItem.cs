using System.Xml.Serialization;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks.Models
{
	public class ConversionItem : BindableBase
    {
        private string description = string.Empty;
        private SearchText searchText = new SearchText();
        private ReplacementText replacementText = new ReplacementText();

        [XmlElement]
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        [XmlElement]
        public SearchText Search
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value); }
        }

        [XmlElement]
        public ReplacementText Replacement
        {
            get { return replacementText; }
            set { SetProperty(ref replacementText, value); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ConversionItem c = (ConversionItem)obj;

            return searchText.Equals(c.searchText) &&
                   replacementText.Equals(c.replacementText);
        }

        public override int GetHashCode()
        {
            return 31 * searchText.GetHashCode() * replacementText.GetHashCode();
        }
    }
}