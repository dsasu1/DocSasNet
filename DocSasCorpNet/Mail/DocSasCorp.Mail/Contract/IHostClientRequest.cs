using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IHostClientRequest
    /// </summary>
    public interface IHostClientRequest
    {
        /// <summary>
        /// Host
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// IsSsl
        /// </summary>
        bool IsSsl { get; set; }
    }
}
