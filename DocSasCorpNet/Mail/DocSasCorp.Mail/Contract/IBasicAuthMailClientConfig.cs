using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IBasicAuthMailClientConfig
    /// </summary>
    public interface IBasicAuthMailClientConfig
    {
        /// <summary>
        /// BasicAuthRequestInfo
        /// </summary>
        IBasicAuthRequest BasicAuthRequestInfo { get; set; }

        /// <summary>
        /// HostClientRequestInfo
        /// </summary>
        IHostClientRequest HostClientRequestInfo { get; set; }
    }
}
