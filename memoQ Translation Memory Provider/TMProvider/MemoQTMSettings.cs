using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using TMProvider;

using System.ComponentModel;


namespace TMProvider
{
    public class MemoQTMSettings
    {
        private MemoQTMProviderTypes providerType;
        public MemoQTMProviderTypes ProviderType
        {
            get { return providerType; }
            set { this.providerType = value; }
        }

        private string url;
        public string URL
        {
            get { return url; }
            set { this.url = value; }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { this.userName = value; }
        }

        private string password;
        /// <summary>
        /// Encrypted password. Not serialized with the rest of the settings, but has to come from the credentialstore.
        /// </summary>
        [XmlIgnore]
        public string Password
        {
            get { return password; }
            set { this.password = value; }
        }

        private LoginTypes loginType = LoginTypes.Undefined;
        public LoginTypes LoginType
        {
            get { return loginType; }
            set { this.loginType = value; }
        }

        private List<Guid> usedTMs;
        /// <summary>
        /// The list of chosen TMs (guids).
        /// </summary>
        [XmlIgnore()]
        public IList<Guid> UsedTMs
        {
            get { return usedTMs.AsReadOnly(); }
            set { this.usedTMs = (List<Guid>)value; }
        }

        /// <summary>
        /// Only for seralization. DO NOT USE!
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Guid> XMLUsedTMs
        {
            get { return usedTMs; }
            set { this.usedTMs = value; }
        }

        private List<TMPurposes> tmSDLRights;
        /// <summary>
        /// Only for serialization! DO NOT USE!!
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<TMPurposes> XMLTMSDLRights
        {
            get { return tmSDLRights; }
            set { this.tmSDLRights = value; }
        }

        [XmlIgnore]
        public IList<TMPurposes> TMSDLRights
        {
            get { return tmSDLRights.AsReadOnly(); }
            set { this.tmSDLRights = (List<TMPurposes>)value; }
        }

        public void AddUsedTM(Guid guid, TMPurposes sdlPurpose)
        {
            usedTMs.Add(guid);
            tmSDLRights.Add(sdlPurpose);
        }

        public void RemoveUsedTM(Guid guid)
        {
            int ix = usedTMs.FindIndex(g => g == guid);
            if (ix != -1)
            {
                usedTMs.RemoveAt(ix);
                tmSDLRights.RemoveAt(ix);
            }
        }

        public void SetTMPurpose(Guid guid, TMPurposes sdlPurpose)
        {
            int ix = usedTMs.FindIndex(g => g == guid);
            if (ix != -1)
            {
                tmSDLRights[ix] = sdlPurpose;
            }
        }

        public void ClearUsedTMs()
        {
            usedTMs.Clear();
            tmSDLRights.Clear();
        }

        // all other settings will come here, and they have to be initialized, added to the constructor as parameter

        public MemoQTMSettings(MemoQTMProviderTypes type, string url, string username, string password, LoginTypes loginType, params TMInfo[] usedTMs)
        {
            this.providerType = type;
            this.url = url;
            this.userName = username;
            this.password = password;
            this.loginType = loginType;
            if (usedTMs == null || usedTMs.Length == 0)
            {
                this.usedTMs = new List<Guid>();
                this.tmSDLRights = new List<TMPurposes>();
            }
            else
            {
                this.usedTMs = usedTMs.Select(tm => tm.TMGuid).ToList();
                this.tmSDLRights = usedTMs.Select(tm => tm.Purpose).ToList();
            }
        }

        public MemoQTMSettings(MemoQTMProviderTypes type, string url, string username, string password, LoginTypes loginType, List<Guid> usedTMGuids, List<TMPurposes> usedTMPurposes)
        {
            this.providerType = type;
            this.url = url;
            this.userName = username;
            this.password = password;
            this.loginType = loginType;
            this.usedTMs = usedTMGuids;
            this.tmSDLRights = usedTMPurposes;
        }

        /// <summary>
        /// DO NOT USE: Public ctor is needed for XML serialization
        /// </summary>
        public MemoQTMSettings()
        {
            // Nothing to do here.
        }

        public static MemoQTMSettings CreateFromXML(string xmlClass)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MemoQTMSettings));
                StringReader stringReader = new StringReader(xmlClass);
                object obj = xmlSerializer.Deserialize(stringReader);
                if (obj is MemoQTMSettings)
                    return (MemoQTMSettings)obj;
                else return null;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The settings for the memoQ TM provider could not be loaded.");
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
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MemoQTMSettings));
                StringWriter stringWriter = new StringWriter();
                xmlSerializer.Serialize(stringWriter, this);
                string serializedXML = stringWriter.ToString();
                return serializedXML;
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("The settings for the memoQ TM provider could not be serialized. The settings will not be saved.");

            }
        }

        public MemoQTMSettings Clone()
        { 
            return new MemoQTMSettings(this.providerType, this.url, this.userName, this.password, this.loginType,
                new List<Guid>(this.usedTMs), new List<TMPurposes>(this.tmSDLRights));
        }

    }
}
