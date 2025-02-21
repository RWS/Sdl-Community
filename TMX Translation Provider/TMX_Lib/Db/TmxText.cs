using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	[BsonIgnoreExtraElements]
    public class TmxText
    {
        public ulong TranslationUnitID;
		// such as "en" (from en-US)
        public string NormalizedLanguage = "";
		// such as "us" (from en-US)
		public string NormalizedLocale = "";

		public string LanguageAndLocale() {
			return NormalizedLocale != "" ? $"{NormalizedLanguage}-{NormalizedLocale}" : NormalizedLanguage;
		}

		// strips punctuation + transforms it to lower case, for exact-search 
		// 
		// the idea for lower case: mongodb does have lower-case search feature, but it requires a locale
		// therefore, since we have texts in several languages, we can either:
		// a) write them as lower-case in db directly
		// b) have a specific table for each language
		//
		// at this time (11/25/2022) I opted for a)
		public string LocaseText;

        public string FormattedText;

        [BsonIgnoreIfNull]
        public double? Score { get; set; }
    }

}
