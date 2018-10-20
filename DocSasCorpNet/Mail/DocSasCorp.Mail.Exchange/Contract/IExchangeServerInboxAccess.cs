using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Contract;

namespace DocSasCorp.Mail.Exchange.Contract
{
    /// <summary>
    /// IExchangeServerInboxAccess
    /// </summary>
    public interface IExchangeServerInboxAccess
    {
        /// <summary>
        /// HasAccess
        /// </summary>
        bool HasAccess { get; set; }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        string ErrorMessage { get; set; }
    }
}
