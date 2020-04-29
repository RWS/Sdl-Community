using System;

namespace Sdl.LanguagePlatform.MTConnectors.Google.DataContracts
{
    internal class GoogleCredentialsDataStore
    {
        private const string GoogleSettingsProviderFilename = "Google.SettingsProvider.xml";

        private string CredentialProviderFilePath
        {
            get
            {
                string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string projectApiFolder = System.IO.Path.Combine(localAppDataFolder, "SDL");
                return System.IO.Path.Combine(projectApiFolder, GoogleSettingsProviderFilename);
            }
        }

        internal void Save(GoogleCredentials credentials)
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(credentials.GetType());
            using (System.IO.StreamWriter xml = new System.IO.StreamWriter(CredentialProviderFilePath))
            {
                xmlSerializer.Serialize(xml, credentials);
            }
        }

        internal GoogleCredentials Load()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer
                    = new System.Xml.Serialization.XmlSerializer(typeof(GoogleCredentials));

                using (System.IO.StreamReader rdr = new System.IO.StreamReader(CredentialProviderFilePath))
                {
                    var googleCredentials = xmlSerializer.Deserialize(rdr) as GoogleCredentials;
                    return googleCredentials;
                }
            }
            catch (Exception)
            {
                // ignore all exceptions
            }

            return null;
        }
    }
}
