using DocSasCorp.Mail.Concrete;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Helper;
using DocSasCorp.Mail.MailException;
using DocSasCorp.Mail.SendGrid.Contract;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSasCorp.Mail.SendGrid.Concrete
{
    /// <summary>
    /// 
    /// </summary>
    public class SendGridMailClient : ISendGridMailClient
    {

        private readonly IApiKeyMailClientConfig _clientConfig;
        private SendGridClient _client = null;
        /// SmtpMailClient
        /// </summary>
        /// <param name="clientConfig"></param>
        public SendGridMailClient(IApiKeyMailClientConfig clientConfig)
        {
            _clientConfig = clientConfig ?? throw new DocSasCorpMailException("clientConfig is required");

            if (string.IsNullOrWhiteSpace(clientConfig.ApiKey))
                throw new DocSasCorpMailException("ApiKey is required");


        }

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendMailResponse> SendEmailAsync(ISendMailRequest request)
        {
            var response = new SendMailResponse();

            try
            {
                if (_client == null)
                {
                    _client = new SendGridClient(_clientConfig.ApiKey);
                }

                var recipientList = request.Recipients.Select(recipient => new EmailAddress(recipient)).ToList();

                var from = new EmailAddress(request.From, request.FromName);
                var plainTextContent = HtmlText.ConvertHtml(request.Message);
                var htmlContent = request.Message;
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, recipientList, request.Subject, plainTextContent, htmlContent);
                await _client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
           

            return response;
        }
    }
}
