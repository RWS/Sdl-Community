using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TMX_Lib.Db
{
	//[BsonIgnoreExtraElements]
    public class TmxTranslationUnit
    {
	    public ObjectId Id { get; set; }

		// IMPORTANT: at this time, I assume there's a single person updating the database
		// in this case, TranslationUnitID will always be unique
		//
		// when this won't be the case, we'll need to devise a different algorithm for generating unique IDs
		public ulong TranslationUnitID;

        public DateTime? CreationDate;
        public string CreationAuthor = "";
        public DateTime? ChangeDate;
        public string ChangeAuthor = "";
        public string XmlProperties = "";
        public string TuAttributes = "";

		// helper - so I know -> when an update of a Translation Unit happens,
		// when updating the texts -> are they adds or updates
        public List<string> NormalizedLanguages = new List<string>();
    }
}
