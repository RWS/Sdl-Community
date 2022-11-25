using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	[BsonIgnoreExtraElements]
    public class TmxText
    {
        public ulong TranslationUnitID;
        public string Language;
        public string LocaseText;
        public string FormattedText;
    }
}
