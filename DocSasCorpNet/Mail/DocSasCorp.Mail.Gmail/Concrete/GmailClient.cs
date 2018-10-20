using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DocSasCorp.Mail.Concrete;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Gmail.Contract;
using DocSasCorp.Mail.MailException;
using Google.Apis.Gmail.v1.Data;
using System.Net.Mail;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Auth.OAuth2.Responses;
using DocSasCorp.Mail.Gmail.Model;
using DocSasCorp.Core.Convertor;
using DocSasCorp.Mail.Helper;

namespace DocSasCorp.Mail.Gmail.Concrete
{
    /// <summary>
    /// GmailClient
    /// </summary>
    public class GmailClient : IGmailClient
    {
        private readonly IOAuth2MailClientConfig _clientConfig;
        private GmailService _client = null;
        private string _token = string.Empty;

        /// SmtpMailClient
        /// </summary>
        /// <param name="clientConfig"></param>
        public GmailClient(IOAuth2MailClientConfig clientConfig)
        {
            _clientConfig = clientConfig ?? throw new DocSasCorpMailException("clientConfig is required");

            if (clientConfig.OAuth2TokenizeRequestInfo == null)
                throw new DocSasCorpMailException("OAuth2TokenizeRequestInfo is required");
            else if (clientConfig.MailDataStore == null)
                throw new DocSasCorpMailException("MailDataStore is required");

 
        }

        /// <summary>
        /// AuthenticateAsync
        /// </summary>
        /// <returns></returns>
        public async Task<OAuth2AccessResponse> AuthenticateAsync(string code)
        {
            var response = new OAuth2AccessResponse();

            var tokenRequest = new GoogleTokenRequest()
            {
                ClientId = _clientConfig.OAuth2TokenizeRequestInfo.ClientId,
                ClientSecret = _clientConfig.OAuth2TokenizeRequestInfo.ClientSecret,
                RedirectUri = _clientConfig.OAuth2TokenizeRequestInfo.RedirectUrl,
                Code = code
            };

            _clientConfig.OAuth2TokenizeRequestInfo.AuthorizedCode = code;

            var result = await GetTokenByCode(_clientConfig.OAuth2TokenizeRequestInfo.TokenEndPointUrl, tokenRequest);

            if (!string.IsNullOrWhiteSpace(result.AccessToken))
            {
                var refreshDataExpireDate = DateTimeOffset.Now.AddMonths(12);

                var tokenExpireTime = double.Parse(result.ExpiresIn);
                var tokenExpireDate = DateTimeOffset.Now.AddSeconds(tokenExpireTime);
                _clientConfig.MailDataStore.Add($"{_clientConfig.MailDataStore.StoreKey}_GMailToken",result.AccessToken, tokenExpireDate);

                if (!string.IsNullOrWhiteSpace(result.RefreshToken))
                    _clientConfig.MailDataStore.Add($"{_clientConfig.MailDataStore.StoreKey}_GMailRefreshData", result, refreshDataExpireDate);


                response.Token = result.AccessToken;
                _token = result.AccessToken;
                response.IsSucces = true;
            }

            return response;
        }

        /// <summary>
        /// GetAuthenticationDetailAsync
        /// </summary>
        /// <returns></returns>
        public async Task<OAuth2AccessResponse> GetAuthenticationDetailAsync()
        {
            var mailResponse = new OAuth2AccessResponse();
            var token = string.Empty;
         
            try
            {

                token = _clientConfig.MailDataStore.Get($"{_clientConfig.MailDataStore.StoreKey}_GMailToken") as string;

                if (string.IsNullOrEmpty(token)) mailResponse.ErrorMessage = "Access Token has expired.";

                if (string.IsNullOrWhiteSpace(token))
                {
                    var refreshData = _clientConfig.MailDataStore.Get($"{_clientConfig.MailDataStore.StoreKey}_GMailRefreshData") as GoogleTokenResponse;

                    if (refreshData != null)
                    {
                        var refreshRquest = new GoogleRefreshTokenRequest()
                        {
                            ClientId = _clientConfig.OAuth2TokenizeRequestInfo.ClientId,
                            ClientSecret = _clientConfig.OAuth2TokenizeRequestInfo.ClientSecret,
                            RefreshToken = refreshData.RefreshToken
                        };

                        var result = await RefreshToken(_clientConfig.OAuth2TokenizeRequestInfo.TokenEndPointUrl, refreshRquest);

                        if (!string.IsNullOrWhiteSpace(result.AccessToken))
                        {
                            var tokenExpireTime = double.Parse(result.ExpiresIn);
                            var tokenExpireDate = DateTimeOffset.Now.AddSeconds(tokenExpireTime);
                            _clientConfig.MailDataStore.Add($"{_clientConfig.MailDataStore.StoreKey}_GMailToken", result.AccessToken, tokenExpireDate);

                            token = result.AccessToken;

                        }

                    }

                }

                if (string.IsNullOrWhiteSpace(token)) mailResponse.ErrorMessage += " Re-authenticate.";
            }
            catch (Exception ex)
            {
                mailResponse.ErrorMessage += ex.Message;
            }

            mailResponse.IsSucces = !string.IsNullOrWhiteSpace(token);
            mailResponse.Token = token;
            _token = token;
            return mailResponse;
        }

        /// <summary>
        /// GetAuthorizationUrl
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizationUrl()
        {
            return $"{_clientConfig.OAuth2TokenizeRequestInfo.AuthorizationEndPointUrl}?client_id={_clientConfig.OAuth2TokenizeRequestInfo.ClientId}&redirect_uri={_clientConfig.OAuth2TokenizeRequestInfo.RedirectUrl}&response_type=code&scope={_clientConfig.OAuth2TokenizeRequestInfo.Scope}&state={_clientConfig.OAuth2TokenizeRequestInfo.State}&access_type=offline";
        }

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendMailResponse> SendEmailAsync(ISendMailRequest request)
        {
            var messageResponse = new SendMailResponse();
            var messageModel = BuildEmailMessage(request);

            try
            {
                if (string.IsNullOrWhiteSpace(_token)) 
                {
                    await GetAuthenticationDetailAsync();
                }
                if (_client == null)
                {                 
                    _client = GetGmailClient(); 
                }

                var result = await _client.Users.Messages.Send(messageModel, "me").ExecuteAsync();

                messageResponse.IsSuccess = true;


            }
            catch (Exception ex)
            {
                messageResponse.ErrorMessage = ex.Message;
             
            }

            return messageResponse;
        }

        /// <summary>
        /// GetTokenByCode
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private async Task<GoogleTokenResponse> GetTokenByCode(string url, GoogleTokenRequest postData)
        {
            var result = new GoogleTokenResponse();
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("code", postData.Code));
            keyValues.Add(new KeyValuePair<string, string>("client_id", postData.ClientId));
            keyValues.Add(new KeyValuePair<string, string>("client_secret", postData.ClientSecret));
            keyValues.Add(new KeyValuePair<string, string>("redirect_uri", postData.RedirectUri));
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

            HttpContent cont = new FormUrlEncodedContent(keyValues);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, cont))
                {
                    using (HttpContent content = response.Content)
                    {
                        string data = await content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            if (data.Contains("error"))
                            {
                                result.ErrorMessage = data;
                            }
                            else
                            {
                                result = JsonToObject.FromJson<GoogleTokenResponse>(data);
                            }

                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private async Task<GoogleTokenResponse> RefreshToken(string url, GoogleRefreshTokenRequest postData)
        {
            var result = new GoogleTokenResponse();
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("refresh_token", postData.RefreshToken));
            keyValues.Add(new KeyValuePair<string, string>("client_id", postData.ClientId));
            keyValues.Add(new KeyValuePair<string, string>("client_secret", postData.ClientSecret));
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));

            HttpContent cont = new FormUrlEncodedContent(keyValues);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, cont))
                {
                    using (HttpContent content = response.Content)
                    {
                        string data = await content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            if (data.Contains("error"))
                            {
                                result.ErrorMessage = data;
                            }
                            else
                            {
                                result = JsonToObject.FromJson<GoogleTokenResponse>(data);
                            }

                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// BuildEmailMessage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Message BuildEmailMessage(ISendMailRequest model)
        {
            Message message = new Message();
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.Subject = model.Subject;
            mail.To.Add(new System.Net.Mail.MailAddress(model.Recipients[0]));

            var htmlMimeType = new ContentType("text/html");
            var textMimeType = new ContentType("text/plain");

            // Add text/plain as an AlternativeView
            var plainText = HtmlText.ConvertHtml(model.Message);
            var plainTextView = AlternateView.CreateAlternateViewFromString(plainText);
            plainTextView.ContentType = textMimeType;
            mail.AlternateViews.Add(plainTextView);

            // Add text/html as an AlternateView
            var htmlView = AlternateView.CreateAlternateViewFromString(model.Message);
            htmlView.ContentType = htmlMimeType;
            mail.AlternateViews.Add(htmlView);

            foreach (var header in model.Headers)
            {
                mail.Headers.Add(header.Key, header.Value);
            }

            MimeKit.MimeMessage mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mail);


            message.Raw = Encode(mimeMessage.ToString());

            return message;
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string Encode(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        /// <summary>
        /// GetGmailClient
        /// </summary>
        /// <returns></returns>
        private GmailService GetGmailClient()
        {
            var tokenData = new TokenResponse { AccessToken = _token, ExpiresInSeconds = 3600, IssuedUtc = DateTime.UtcNow };
            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                 new GoogleAuthorizationCodeFlow.Initializer
                 {
                     ClientSecrets = new ClientSecrets() { ClientId = _clientConfig.OAuth2TokenizeRequestInfo.ClientId, ClientSecret = _clientConfig.OAuth2TokenizeRequestInfo.ClientSecret }
                 }), "me", tokenData);

            GmailService service = new GmailService(new BaseClientService.Initializer
            {
                ApplicationName = "vidReach",
                HttpClientInitializer = credentials,
                DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.Exception | ExponentialBackOffPolicy.UnsuccessfulResponse503
            });
            return service;
        }
    }
}
