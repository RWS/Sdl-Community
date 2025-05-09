using Newtonsoft.Json;

namespace Sdl.Community.GSVersionFetch.Model
{
    public class Credentials
    {
        [JsonIgnore]
        public CredentialType CredentialType { get; set; }

        [JsonIgnore]
        public string Password
        { get; set; }

        public string ServiceUrl { get; set; }

        [JsonIgnore]
        public SsoData SsoCredentials
        { get; set; }

        public string UserName { get; set; }

        [JsonIgnore]
        public WindowsSsoData WindowsSsoCredentials { get; set; }

        public void DetermineCredentialType(bool isWindowsUser)
        {
            var credentialType = CredentialType.Normal;

            if (isWindowsUser && string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(UserName))
            {
                credentialType = CredentialType.WindowsSSO;
            }

            if (!isWindowsUser && string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserName))
            {
                credentialType = CredentialType.SSO;
            }

            CredentialType = credentialType;
        }
    }
}