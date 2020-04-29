using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.MTConnectors.Google.DataContracts
{
    /// <summary>
    /// Defines the content of the Google credential file for storing private API Keys
    /// </summary>
    [DataContract]
    public class GoogleCredentials
    {
        private List<ConnectionEntry> _credentialsList;

        public GoogleCredentials()
        {
            _credentialsList = new List<ConnectionEntry>();
        }

        /// <summary>
        /// An array of ConnectionEntry objects for storing in the LW credentials file
        /// </summary>
        [DataMember]
        public ConnectionEntry[] Credentials
        {
            get
            {
                return _credentialsList.ToArray();
            }

            set
            {
                _credentialsList.Clear();
                _credentialsList.AddRange(value);
            }
        }

        public bool DoesDifferentNamedSettingAlreadyExist(Settings singleSettings, string name)
        {
            if (string.IsNullOrEmpty(name) || singleSettings == null)
            {
                return false;
            }

            return _credentialsList.Exists(item => item.DoesDifferentConnectionWithSameNameExist(singleSettings, name));
        }

        public void UpdateKeyToUserStorage(string name, Settings singleSettings)
        {
            if (!string.IsNullOrEmpty(name))
            {
                DeleteNamedEntry(name); // delete any with the same name
            }

            AddOrUpdate(new ConnectionEntry(name, singleSettings.UserKey));
        }

        public void DeleteNamedEntry(string name)
        {
            _credentialsList.RemoveAll(item => item.HasSameName(name));
        }

        public void DeleteAllEntries()
        {
            _credentialsList.Clear();
        }

        private void AddOrUpdate(ConnectionEntry connectionEntry)
        {
            ConnectionEntry found = _credentialsList.Find(item => item.IsSameConnectionDetails(connectionEntry));
            if (found != null)
            {
                if (!string.IsNullOrEmpty(connectionEntry.Name))
                {
                    // and update the name if there is one
                    found.Name = connectionEntry.Name;
                }
            }
            else
            {
                _credentialsList.Add(connectionEntry);      // remember connection details so Delete All can clear the cache of them
            }
        }
    }
}
