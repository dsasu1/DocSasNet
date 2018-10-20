using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Gmail.Model
{
    [Serializable]
    public class GoogleTokenRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

    }

    [Serializable]
    public class GoogleRefreshTokenRequest
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }

    [Serializable]
    public class GoogleTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public string ErrorMessage { get; set; }
    }

}
