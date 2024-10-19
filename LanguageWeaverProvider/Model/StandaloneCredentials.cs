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
                    else if (CloudCredentials.UserPassword is not null) authenticationType = AuthenticationType.CloudCredentials;
                    else if (CloudCredentials.ConnectionCode is not null) authenticationType = AuthenticationType.CloudSSO;

                    if (authenticationType != AuthenticationType.None)
                    {
                        EdgeCredentials = null;
                        return authenticationType;
                    }
                }
                
                CloudCredentials = null;

                if (EdgeCredentials?.Host is not null) authenticationType = AuthenticationType.EdgeApiKey;
                else if (EdgeCredentials?.Password is not null) authenticationType = AuthenticationType.EdgeCredentials;
                //if (EdgeCredentials is not null) return AuthenticationType.EdgeSSO;

                if (authenticationType == AuthenticationType.None) EdgeCredentials = null;
                return authenticationType;
            }
        }

        public CloudCredentials CloudCredentials { get; set; }

        public EdgeCredentials EdgeCredentials { get; set; }

        public bool IsCloudCredential => CloudCredentials != null;
    }
}