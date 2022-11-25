using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	[BsonIgnoreExtraElements]
	public class TmxLanguage
    {
        // example: en-GB
        public string Language;
        // any other extra info related to the language
        public string Meta;
    }
}
