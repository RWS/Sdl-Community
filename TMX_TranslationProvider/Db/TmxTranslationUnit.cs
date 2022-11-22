using System;

namespace TMX_TranslationProvider.Db
{
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
