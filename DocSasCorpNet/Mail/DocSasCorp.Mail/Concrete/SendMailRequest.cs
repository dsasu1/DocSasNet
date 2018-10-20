using DocSasCorp.Mail.Contract;
using System.Collections.Generic;

namespace DocSasCorp.Mail.Concrete
{
    /// <summary>
    /// SendMailRequest
    /// </summary>
    public class SendMailRequest : ISendMailRequest
    {
        /// <summary>
        /// SendMailRequest
        /// </summary>
        public SendMailRequest()
        {
            Headers = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// From Email Address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// From Name
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// Message Subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Message  Body
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Message Recipients
        /// </summary>
        public string[] Recipients { get; set; }
        /// <summary>
        /// Message Headers
        /// </summary>
        public List<KeyValuePair<string, string>> Headers { get; set; }
    }
}