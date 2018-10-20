using DocSasCorp.Mail.Concrete;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.MailException;
using DocSasCorp.Mail.Office365Outlook.Contract;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DocSasCorp.Mail.Office365Outlook.Concrete
{
    /// <summary>
    /// 
    /// </summary>
    public class Office365MailClient : IOffice365MailClient
    {
        private readonly IOAuth2MailClientConfig _clientConfig;
        private GraphServiceClient _client = null;
        private string _token = string.Empty;

        /// SmtpMailClient
        /// </summary>
        /// <param name="clientConfig"></param>
        public Office365MailClient(IOAuth2MailClientConfig clientConfig)
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
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<OAuth2AccessResponse> AuthenticateAsync(string code)
        {
            var mailResponse = new OAuth2AccessResponse();
            var refreshTokenExpireDate = DateTimeOffset.Now.AddDays(90);

            var graphScopes = _clientConfig.OAuth2TokenizeRequestInfo.Scope.Split(' ');
            var userTokenCache = new TokenCacheModel.MicrosoftTokenStore(_clientConfig.MailDataStore, refreshTokenExpireDate).GetMsalCacheInstance();
            var cca = new ConfidentialClientApplication(_clientConfig.OAuth2TokenizeRequestInfo.ClientId, _clientConfig.OAuth2TokenizeRequestInfo.RedirectUrl, new ClientCredential(_clientConfig.OAuth2TokenizeRequestInfo.ClientSecret), userTokenCache, null);
            _clientConfig.OAuth2TokenizeRequestInfo.AuthorizedCode = code;
            var result = await cca.AcquireTokenByAuthorizationCodeAsync(code, graphScopes).ConfigureAwait(false); ;
            _clientConfig.MailDataStore.Add($"{_clientConfig.MailDataStore.StoreKey}_MSMailToken", result.AccessToken, result.ExpiresOn);

            mailResponse.Token = result.AccessToken;
            mailResponse.IsSucces = !string.IsNullOrWhiteSpace(result.AccessToken);
            _token = result.AccessToken;
            return mailResponse;
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

                token = _clientConfig.MailDataStore.Get($"{_clientConfig.MailDataStore.StoreKey}_MSMailToken") as string;

                if (string.IsNullOrEmpty(token)) mailResponse.ErrorMessage = "Access Token has expired.";


                if (string.IsNullOrWhiteSpace(token))
                {

                    var msalCache = new TokenCacheModel.MicrosoftTokenStore(_clientConfig.MailDataStore, DateTimeOffset.Now.AddDays(90));
                    var msalSession = msalCache.GetMsalCacheInstance();
                    var cca = new ConfidentialClientApplication(_clientConfig.OAuth2TokenizeRequestInfo.ClientId, _clientConfig.OAuth2TokenizeRequestInfo.RedirectUrl, new ClientCredential(_clientConfig.OAuth2TokenizeRequestInfo.ClientSecret), msalSession, null);

                    var accounts = await cca.GetAccountsAsync();
                    if (accounts != null && accounts.Any())
                    {
                        var graphScopes = _clientConfig.OAuth2TokenizeRequestInfo.Scope.Split(' ');
                        var result = await cca.AcquireTokenSilentAsync(graphScopes, accounts.First(), null, true).ConfigureAwait(false); ;

                        if (result != null)
                        {
                            _clientConfig.MailDataStore.Add($"{_clientConfig.MailDataStore.StoreKey}_MSMailToken", result.AccessToken, result.ExpiresOn);

                            token = result.AccessToken;

                        }
                    }

                    if (string.IsNullOrWhiteSpace(token)) mailResponse.ErrorMessage += " Re-authenticate.";
                }

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
            return $"{_clientConfig.OAuth2TokenizeRequestInfo.AuthorizationEndPointUrl}?client_id={_clientConfig.OAuth2TokenizeRequestInfo.ClientId}&redirect_uri={_clientConfig.OAuth2TokenizeRequestInfo.RedirectUrl}&response_type=code&scope=openid offline_access Profile {_clientConfig.OAuth2TokenizeRequestInfo.Scope}&state={_clientConfig.OAuth2TokenizeRequestInfo.State}";
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
                    _client = new GraphServiceClient(new DelegateAuthenticationProvider(async requestMessage =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);
                    }));
                }
                

                await _client.Me.SendMail(messageModel, true).Request().PostAsync();

                messageResponse.IsSuccess = true;


            }
            catch (Exception ex)
            {
                messageResponse.ErrorMessage = ex.Message;
            }

            return messageResponse;
        }

        /// <summary>
        /// BuildEmailMessage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private Message BuildEmailMessage(ISendMailRequest model)
        {
            var recipientList = model.Recipients.Select(recipient => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = recipient
                }
            }).ToList();
            var email = new Message
            {
                Body = new ItemBody
                {
                    Content = model.Message,
                    ContentType = BodyType.Html
                },
                Subject = model.Subject,
                ToRecipients = recipientList,
            };

            var singleValues = new MessageSingleValueExtendedPropertiesCollectionPage();
            var multiValues = new MessageMultiValueExtendedPropertiesCollectionPage();

            foreach (var header in model.Headers)
            {
                if (header.Key.Equals("List-Unsubscribe", StringComparison.OrdinalIgnoreCase))
                {
                    var splitValues = header.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitValues.Length > 1)
                    {
                        multiValues.Add(new MultiValueLegacyExtendedProperty() { Id = string.Concat("StringArray {00020386-0000-0000-C000-000000000046} Name ", header.Key), Value = splitValues });
                    }
                    else
                    {
                        singleValues.Add(new SingleValueLegacyExtendedProperty() { Id = string.Concat("String {00020386-0000-0000-C000-000000000046} Name ", header.Key), Value = header.Value });
                    }
                }
                else
                {
                    singleValues.Add(new SingleValueLegacyExtendedProperty() { Id = string.Concat("String {00020386-0000-0000-C000-000000000046} Name ", header.Key), Value = header.Value });
                }

            }


            if (singleValues.Count > 0)
            {
                email.SingleValueExtendedProperties = singleValues;
            }

            if (multiValues.Count > 0)
            {
                email.MultiValueExtendedProperties = multiValues;
            }


            return email;
        }
    }
}
