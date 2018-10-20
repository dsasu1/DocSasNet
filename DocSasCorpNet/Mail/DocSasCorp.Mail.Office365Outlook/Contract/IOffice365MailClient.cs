using DocSasCorp.Mail.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Office365Outlook.Contract
{
    public interface IOffice365MailClient : ISendMail, IOAuth2MailOperations
    {
    }
}
