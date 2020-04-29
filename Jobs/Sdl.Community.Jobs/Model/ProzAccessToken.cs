using System;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class ProzAccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public Int64 ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        public bool HasExpired
        {
            get
            {
                var currentDate = DateTimeOffset.UtcNow;
                var tokenDuration = _tokenDate.AddMilliseconds(ExpiresIn);
                if (currentDate <= tokenDuration) return false;
                return true;
            }
        }

        private DateTimeOffset _tokenDate;

        public ProzAccessToken()
        {
            this._tokenDate = DateTimeOffset.UtcNow;
        }
    }
}
