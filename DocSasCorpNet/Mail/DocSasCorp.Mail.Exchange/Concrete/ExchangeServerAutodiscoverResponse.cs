using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Exchange.Contract;

namespace DocSasCorp.Mail.Exchange.Concrete
{
    /// <summary>
    /// ExchangeServerAutodiscoverResponse
    /// </summary>
    public class ExchangeServerAutodiscoverResponse : IExchangeServerAutodiscoverResponse
    {
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// VersionLabel
        /// </summary>
        public string VersionLabel { get; set; }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
