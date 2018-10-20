using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Exchange.Concrete;

namespace DocSasCorp.Mail.Exchange.Contract
{
    /// <summary>
    /// IExchangeMailClient
    /// </summary>
    public interface IExchangeMailClient: ISendMail
    {
        /// <summary>
        /// AutodiscoverEndpoint
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ExchangeServerAutodiscoverResponse> AutodiscoverEndpoint(string email);

        /// <summary>
        /// CanConnectInbox
        /// </summary>
        /// <returns></returns>
        Task<ExchangeServerInboxAccess> CanConnectInbox();
    }
}
