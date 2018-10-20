using System.Collections.Generic;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// ISendMailRequest
    /// </summary>
    public interface ISendMailRequest
    {
        /// <summary>
        /// From Email Address
        /// </summary>
        string From { get; set; }

        /// <summary>
        /// From Name
        /// </summary>
        string FromName { get; set; }

        /// <summary>
        /// Message Subject
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Message  Body
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Message Recipients
        /// </summary>
        string[] Recipients { get; set; }

        /// <summary>
        /// Message Headers
        /// </summary>
        List<KeyValuePair<string, string>> Headers { get; set; }
    }
}