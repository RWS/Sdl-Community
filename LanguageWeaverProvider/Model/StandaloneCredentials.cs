using Newtonsoft.Json;
using System;

namespace LanguageWeaverProvider.Model
{
    public class StandaloneCredentials
    {
        public StandaloneCredentials(string serializedCredentials)
        {
            try
            {
                CloudCredentials = JsonConvert.DeserializeObject<CloudCredentials>(serializedCredentials);
                CloudCredentials.AccountRegion
                    = CloudCredentials.AccountRegion.ToLower().Equals("eu")
                        ? Constants.CloudEUUrl
                        : Constants.CloudUSUrl;
            }
            catch { }

            try
            {
                EdgeCredentials = JsonConvert.DeserializeObject<EdgeCredentials>(serializedCredentials);
                EdgeCredentials.Uri = new Uri(EdgeCredentials.Host);
            }
            catch { }
        }

        public AuthenticationType AuthenticationType
        {
            get
            {
                var authenticationType = AuthenticationType.None;
                if (IsCloudCredential)
                {
                    if (CloudCredentials.ClientID is not null) authenticationType = AuthenticationType.CloudAPI;
                    if (CloudCredentials.UserPassword is not null) authenticationType = AuthenticationType.CloudCredentials;
                    if (CloudCredentials.ConnectionCode is not null) authenticationType = AuthenticationType.CloudSSO;

                    if (authenticationType != AuthenticationType.None) EdgeCredentials = null;
                }

                CloudCredentials = null;

                if (EdgeCredentials?.Host is not null) return AuthenticationType.EdgeApiKey;
                if (EdgeCredentials?.Password is not null) return AuthenticationType.EdgeCredentials;
                //if (EdgeCredentials is not null) return AuthenticationType.EdgeSSO;
                return AuthenticationType.None;
            }
        }

        public CloudCredentials CloudCredentials { get; set; }

        public EdgeCredentials EdgeCredentials { get; set; }

        public bool IsCloudCredential => CloudCredentials != null;
    }
}