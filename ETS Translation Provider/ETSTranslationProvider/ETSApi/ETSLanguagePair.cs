namespace ETSTranslationProvider.ETSApi
{
    public class ETSLanguagePair
    {
        public string SourceLanguageId { get; set; }
        public string TargetLanguageId { get; set; }
        public string LanguagePairId { get; set; }
        public string Domain { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public string Technology { get; set; }

		public string DictionaryId { get; set; }
    }
}