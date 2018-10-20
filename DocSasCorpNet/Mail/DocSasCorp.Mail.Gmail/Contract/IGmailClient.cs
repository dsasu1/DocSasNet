using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Contract;
namespace DocSasCorp.Mail.Gmail.Contract
{
    /// <summary>
    /// IGmailClient
    /// </summary>
    public interface IGmailClient : ISendMail, IOAuth2MailOperations
    {
    }
}
