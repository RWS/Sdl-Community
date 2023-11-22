using Newtonsoft.Json;

namespace Sdl.Community.MTEdge.Provider.Model
{
    public class MTEdgeLanguagePair
    {
        public string Domain { get; set; }

		public string Version { get; set; }

		public string Platform { get; set; }

        public string Technology { get; set; }

		public string DictionaryId { get; set; }

        public string LanguagePairId { get; set; }

        public string SourceLanguageId { get; set; }

        public string TargetLanguageId { get; set; }

		public string DisplayName => $"{Domain} {Version}";
	}
}