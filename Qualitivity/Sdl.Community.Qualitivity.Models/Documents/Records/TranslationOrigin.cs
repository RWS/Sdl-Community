using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{
   
    [Serializable]
    public class TranslationOrigin : ICloneable
    {

        public enum LanguageType
        {
            Original,
            Updated,
            UpdatedPrevious,
            None
        }

        public string Id { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }
        [XmlIgnore]
        public int RecordId { get; set; }

        public LanguageType LangType { get; set; }

        /// <summary>
        /// ApprovedSignOff
        /// ApprovedTranslation
        /// Draft
        /// RejectedSignOff
        /// RejectedTranslation
        /// Translated
        /// Unspecified
        /// </summary>
        [XmlAttribute(AttributeName = "confirmationLevel")]
        public string ConfirmationLevel { get; set; } //confirmation level {Draft, Translated etc...} 

        /// <summary>
        ///PM
        ///CM
        ///AT
        ///100
        ///1-99
        ///New
        /// </summary>
        [XmlAttribute(AttributeName = "translationStatus")]
        public string TranslationStatus { get; set; } //translation status 
       
        /// <summary>
        /// interactive
        /// source
        /// document-match
        /// mt
        /// tm
        /// auto-propagated
        /// auto-suggest
        /// </summary>
        [XmlAttribute(AttributeName = "originType")]
        public string OriginType { get; set; } //origin type 

        /// <summary>
        /// name of tm provider system
        /// </summary>
        [XmlAttribute(AttributeName = "originSystem")]
        public string OriginSystem { get; set; } //origin system


        public TranslationOrigin()
        {
            Id = Guid.NewGuid().ToString();
            DocumentActivityId = -1;
            RecordId = -1;
            LangType = LanguageType.None;

            ConfirmationLevel = string.Empty;
            TranslationStatus = string.Empty;
            OriginType = string.Empty;
            OriginSystem = string.Empty;
        }

        public TranslationOrigin(LanguageType langType)
        {
            Id = Guid.NewGuid().ToString();
            DocumentActivityId = -1;
            RecordId = -1;
            LangType = langType;

            ConfirmationLevel = string.Empty;
            TranslationStatus = string.Empty;
            OriginType = string.Empty;
            OriginSystem = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
