using DocSasCorp.Mail.Concrete;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Helper;
using DocSasCorp.Mail.MailException;
using DocSasCorp.Mail.Smtp.Contract;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DocSasCorp.Mail.Smtp.Concrete
{
    /// <summary>
    /// SmtpMailClient
    /// </summary>
    public class SmtpMailClient : ISmtpMailClient
    {
        private readonly IBasicAuthMailClientConfig _clientConfig;
        private SmtpClient _client;

        /// SmtpMailClient
        /// </summary>
        /// <param name="clientConfig"></param>
        public SmtpMailClient(IBasicAuthMailClientConfig clientConfig)
        {
            _clientConfig = clientConfig ?? throw new DocSasCorpMailException("clientConfig is required");

            if (clientConfig.BasicAuthRequestInfo == null)
                throw new DocSasCorpMailException("BasicAuthRequestInfo is required");
            else if (clientConfig.HostClientRequestInfo == null)
                throw new DocSasCorpMailException("HostClientRequestInfo is required");

            _client = new SmtpClient(_clientConfig.HostClientRequestInfo.Host, _clientConfig.HostClientRequestInfo.Port)
            {
                UseDefaultCredentials = false,
                EnableSsl = _clientConfig.HostClientRequestInfo.IsSsl,
                Credentials = new NetworkCredential(_clientConfig.BasicAuthRequestInfo.Username, _clientConfig.BasicAuthRequestInfo.Password)
            };
        }

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SendMailResponse> SendEmailAsync(ISendMailRequest request)
        {
            var sendMailResponse = new SendMailResponse();

            var tcs = new TaskCompletionSource<SendMailResponse>();

            //track the message within the handler.
            var sendGuid = Guid.NewGuid();

            SendCompletedEventHandler handler = null;
            handler = (o, ea) =>
            {
                if (!(ea.UserState is Guid) || ((Guid)ea.UserState) != sendGuid) return;
                _client.SendCompleted -= handler;
                if (ea.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (ea.Error != null)
                {
                    sendMailResponse.ErrorMessage = ea.Error.Message;
                    tcs.SetResult(sendMailResponse);
                }
                else
                {
                    sendMailResponse.IsSuccess = true;
                    tcs.SetResult(sendMailResponse);
                }
            };

            var msg = new MailMessage(request.From, request.Recipients.First())
            {
                From = new MailAddress(request.From, request.FromName),
                Subject = request.Subject
            };

            var htmlMimeType = new ContentType("text/html");
            var textMimeType = new ContentType("text/plain");

            // Add text/plain as an AlternativeView
            var plainText = HtmlText.ConvertHtml(request.Message);
            var plainTextView = AlternateView.CreateAlternateViewFromString(plainText);
            plainTextView.ContentType = textMimeType;
            msg.AlternateViews.Add(plainTextView);

            // Add text/html as an AlternateView
            var htmlView = AlternateView.CreateAlternateViewFromString(request.Message);
            htmlView.ContentType = htmlMimeType;
            msg.AlternateViews.Add(htmlView);

            foreach (var header in request.Headers)
            {
                msg.Headers.Add(header.Key, header.Value);
            }

            _client.SendCompleted += handler;
            _client.SendAsync(msg, sendGuid);

            return tcs.Task;
        }
    }
}