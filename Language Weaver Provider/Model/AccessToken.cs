using LanguageWeaverProvider.Services.Model;
using Newtonsoft.Json;
using System;

namespace LanguageWeaverProvider.Model;

public class AccessToken
{
    [JsonProperty("accessToken")]
    public string Token { get; set; }

    public string TokenType { get; set; }

    public string RefreshToken { get; set; }

    public long ValidityInSeconds { get; set; }

    public long ExpiresAt { get; set; }

    public string AccountId { get; set; }

    public string AccountNickname { get; set; }

    public Uri BaseUri { get; set; }

    public EdgeUserPermissions EdgeUserPermissions { get; set; }
}