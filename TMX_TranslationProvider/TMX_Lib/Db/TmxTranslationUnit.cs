using System;
using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	[BsonIgnoreExtraElements]
    public class TmxTranslationUnit
    {
        public ulong ID;
        public DateTime? CreationDate;
        public string CreationAuthor;
        public DateTime? ChangeDate;
        public string ChangeAuthor;
        public string XmlProperties;
        public string TuAttributes;
    }
}
