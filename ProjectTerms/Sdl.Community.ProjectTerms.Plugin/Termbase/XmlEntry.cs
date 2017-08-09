namespace Sdl.Community.ProjectTerms.Plugin.Termbase
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ConceptGrp
    {
        private sbyte conceptField;
        private ConceptGrpLanguageGrp[] languageGrpField;

        public sbyte Concept { get { return conceptField; } set { conceptField = value; } }
        [System.Xml.Serialization.XmlElementAttribute("languageGrp")]
        public ConceptGrpLanguageGrp[] LanguageGrp { get { return languageGrpField; } set { languageGrpField = value; } }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConceptGrpLanguageGrp
    {
        private ConceptGrpLanguageGrpLanguage languageField;
        private ConceptGrpLanguageGrpTermGrp termGrpField;

        public ConceptGrpLanguageGrpLanguage Language { get { return languageField; } set { languageField = value; } }
        public ConceptGrpLanguageGrpTermGrp TermGrp { get { return termGrpField; } set { termGrpField = value; } }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConceptGrpLanguageGrpLanguage
    {
        private string langField;
        private string typeField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Lang { get { return langField; } set { langField = value; } }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type { get { return typeField; } set { typeField = value; } }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConceptGrpLanguageGrpTermGrp
    {
        private string termField;

        public string Term { get { return termField; } set { termField = value; } }
    }
}
