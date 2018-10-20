using System;

namespace DocSasCorp.Mail.MailException
{
    /// <summary>
    /// DocSasCorpMailException
    /// </summary>
    public class DocSasCorpMailException : ApplicationException
    {
        /// <summary>
        /// DocSasCorpMailException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public DocSasCorpMailException(string message, Exception ex) : base(message, ex)
        {
        }

        /// <summary>
        /// DocSasCorpMailException
        /// </summary>
        /// <param name="message"></param>
        public DocSasCorpMailException(string message) : base(message)
        {
        }
    }
}