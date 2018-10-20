using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Exchange.Contract;

namespace DocSasCorp.Mail.Exchange.Concrete
{
    public class ExchangeServerInboxAccess : IExchangeServerInboxAccess
    {
        public bool HasAccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
