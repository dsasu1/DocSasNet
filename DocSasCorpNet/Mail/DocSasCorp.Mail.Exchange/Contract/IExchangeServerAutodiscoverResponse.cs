using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Exchange.Contract
{
    /// <summary>
    /// IExchangeServerAutodiscoverResponse
    /// </summary>
    public interface IExchangeServerAutodiscoverResponse
    {
        /// <summary>
        /// Url
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        int Version { get; set; }
        /// <summary>
        /// ErrorMessage
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// VersionLabel
        /// </summary>
        string VersionLabel { get; set; }
    }
}
