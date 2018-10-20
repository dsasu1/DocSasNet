using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocSasCorp.Mail.Concrete;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Exchange.Contract;
using DocSasCorp.Mail.MailException;
using Microsoft.Exchange.WebServices.Data;

namespace DocSasCorp.Mail.Exchange.Concrete
{
    /// <summary>
    /// ExchangeMailClient
    /// </summary>
    public class ExchangeMailClient : IExchangeMailClient
    {
        private readonly IBasicAuthMailClientConfig _clientConfig;
        private WebCredentials _webCrendential;
        /// <summary>
        /// ExchangeMailClient
        /// </summary>
        /// <param name="clientConfig"></param>
        public ExchangeMailClient(IBasicAuthMailClientConfig clientConfig)
        {
            _clientConfig = clientConfig ?? throw new DocSasCorpMailException("clientConfig is required");

            if (clientConfig.BasicAuthRequestInfo == null)
                throw new DocSasCorpMailException("BasicAuthRequestInfo is required");
            else if (clientConfig.HostClientRequestInfo == null)
                throw new DocSasCorpMailException("HostClientRequestInfo is required");

            _webCrendential = new WebCredentials(_clientConfig.BasicAuthRequestInfo.Username, _clientConfig.BasicAuthRequestInfo.Password);
        }

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendMailResponse> SendEmailAsync(ISendMailRequest request)
        {
            var messageResponse = new SendMailResponse();
            
            var requestedServerVersion = (ExchangeVersion)_clientConfig.HostClientRequestInfo.Port;
            ExchangeService service = new ExchangeService(requestedServerVersion);
            service.Credentials = _webCrendential;
            service.Url = new Uri(_clientConfig.HostClientRequestInfo.Host);

            EmailMessage message = new EmailMessage(service)
            {
                From = new EmailAddress(request.FromName, request.From),
                Subject = request.Subject
            };
            message.ToRecipients.Add(request.Recipients.First());
            message.Body = request.Message;

            ExtendedPropertyDefinition customHeaders;
            foreach (var header in request.Headers)
            {
                if (header.Key.Equals("List-Unsubscribe", StringComparison.OrdinalIgnoreCase))
                {
                    customHeaders = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders,
                                                                                          header.Key,
                                                                                           MapiPropertyType.String);
                    var splitValues = header.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitValues.Length > 1)
                    {
                        message.SetExtendedProperty(customHeaders, splitValues[0]);
                    }
                    else
                    {
                        message.SetExtendedProperty(customHeaders, header.Value);
                    }
                }
                else
                {
                    customHeaders = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders,
                                                                                           header.Key,
                                                                                            MapiPropertyType.String);
                    message.SetExtendedProperty(customHeaders, header.Value);
                }
            }

            try
            {
                await message.SendAndSaveCopy();
                messageResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                messageResponse.ErrorMessage = ex.Message;
            }

            return messageResponse;
        }

        /// <summary>
        /// AutodiscoverEndpoint
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<ExchangeServerAutodiscoverResponse> AutodiscoverEndpoint(string email)
        {
            var discover = new ExchangeServerAutodiscoverResponse();

            try
            {
              
                ExchangeService service = new ExchangeService();
                service.Credentials = _webCrendential;
                service.AutodiscoverUrl(email, RedirectionUrlValidationCallback);
                discover.Url = service.Url.OriginalString;
                Folder folder = await Folder.Bind(service, WellKnownFolderName.Inbox);
                ExchangeServerInfo esi = folder.Service.ServerInfo;
                var version = GetExchangeExchangeVersion(esi);
                discover.VersionLabel = version.ToString();
                discover.Version = (int)version;
            }
            catch (Exception ex)
            {
                discover.ErrorMessage = ex.Message;
            }

            return discover;
        }

        /// <summary>
        /// CanConnectInbox
        /// </summary>
        /// <returns></returns>
        public async Task<ExchangeServerInboxAccess> CanConnectInbox()
        {
            var resp = new ExchangeServerInboxAccess();
            try
            {
               
                var requestedServerVersion = (ExchangeVersion)_clientConfig.HostClientRequestInfo.Port;
                ExchangeService service = new ExchangeService(requestedServerVersion);
                service.Credentials = _webCrendential;
                service.Url = new Uri(_clientConfig.HostClientRequestInfo.Host);
                Folder folder = await Folder.Bind(service, WellKnownFolderName.Inbox);
                resp.HasAccess = true;
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message;
            }

            return resp;
        }

        /// <summary>
        /// RedirectionUrlValidationCallback
        /// </summary>
        /// <param name="redirectionUrl"></param>
        /// <returns></returns>
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            var redirectionUri = new Uri(redirectionUrl);
            var result = redirectionUri.Scheme == "https";
            return result;
        }

        /// <summary>
        /// GetExchangeExchangeVersion
        /// </summary>
        /// <param name="serverInfo"></param>
        /// <returns></returns>
        private ExchangeVersion GetExchangeExchangeVersion(ExchangeServerInfo serverInfo)
        {
            ExchangeVersion version = ExchangeVersion.Exchange2013_SP1;
            if (serverInfo != null)
            {
                switch (serverInfo.MajorVersion)
                {
                    case 15:
                        version = ExchangeVersion.Exchange2013_SP1;
                        break;

                    case 14:
                        version = ExchangeVersion.Exchange2010_SP2;
                        break;

                    case 8:
                        version = ExchangeVersion.Exchange2007_SP1;
                        break;

                    default:
                        break;
                }
            }

            return version;
        }


    }
}
