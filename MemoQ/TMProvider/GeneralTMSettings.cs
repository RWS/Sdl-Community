using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.IO;

namespace TMProvider
{
    public class GeneralTMSettings
    {
        #region Settings for all TMs

        private bool canReverseLangDir = true;
        public bool CanReverseLangDir
        {
            get
            {
                return canReverseLangDir;
            }
        }

        private bool strictSublanguageMatching = false;
        public bool StrictSublanguageMatching
        {
            get { return this.strictSublanguageMatching; }
        }

        private bool concordanceCaseSensitive = false;
        public bool ConcordanceCaseSensitive
        {
            get { return this.concordanceCaseSensitive; }
        }

        private bool concordancNumericEquivalence = false;
        public bool ConcordancNumericEquivalence
        {
            get { return this.concordancNumericEquivalence; }
        }

        #endregion

        public GeneralTMSettings(bool canRevLangDir, bool strictSublang, bool concCaseSens, bool concNumEquiv)
        {
            this.canReverseLangDir = canRevLangDir;
            this.strictSublanguageMatching = strictSublang;
            this.concordanceCaseSensitive = concCaseSens;
            this.concordancNumericEquivalence = concNumEquiv;
        }


        /// <summary>
        /// DO NOT USE: Public ctor is needed for XML serialization
        /// </summary>
        public GeneralTMSettings()
        {
            // Nothing to do here.
        }

        public static GeneralTMSettings CreateFromXML(string xmlClass)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MemoQTMSettings));
                StringReader stringReader = new StringReader(xmlClass);
                object obj = xmlSerializer.Deserialize(stringReader);
                if (obj is MemoQTMSettings)
                    return (GeneralTMSettings)obj;
                else return null;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The general settings for the memoQ TM providers could not be loaded.");
            }
        }

        /// <summary>
        /// Serializes the class into xml string.
        /// </summary>
        /// <returns>The xml string from the class.</returns>
        public string ToXmlString()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GeneralTMSettings));
                StringWriter stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, this);
                string serializedXML = stringWriter.ToString();
                return serializedXML;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The general settings for the memoQ TM providers could not be serialized. The settings will not be saved.");

            }
        }

    }
}
