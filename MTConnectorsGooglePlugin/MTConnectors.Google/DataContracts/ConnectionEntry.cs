using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.MTConnectors.Google.DataContracts
{
    /// <summary>
    /// Defines a connection entry containing a private API Key for a LW REST connection
    /// </summary>
    [DataContract]
    public class ConnectionEntry
    {
        /// <summary>
        /// Parameterless contructor required for serialization
        /// </summary>
        public ConnectionEntry()
        {
        }

        public ConnectionEntry(string name, string userKey)
        {
            UserKey = userKey;
            Name = name;
        }

        [DataMember]
        public string UserKey { get; set; }

        /// <summary>
        /// The Name of the saved conneciton
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        public bool IsSameConnectionDetails(ConnectionEntry other)
        {
            if (other == null)
            {
                return false;
            }

            if (string.Compare(this.UserKey, other.UserKey, true) != 0)
            {
                return false;
            }

            return true;
        }

        public bool DoesDifferentConnectionWithSameNameExist(Settings other, string name)
        {
            if (other == null)
            {
                return false;
            }

            if (string.Compare(this.Name, name, true) == 0)
            {
                if (string.Compare(this.UserKey, other.UserKey, true) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasSameName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (string.Compare(this.Name, name, true) == 0)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
