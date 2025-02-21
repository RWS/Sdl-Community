using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	[BsonIgnoreExtraElements]
    public class TmxMeta
    {
        // example: "Header"
        public string Type;
        // value related to the type
        public string Value;
    }
}
