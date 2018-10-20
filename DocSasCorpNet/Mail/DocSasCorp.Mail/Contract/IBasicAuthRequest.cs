using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IBasicAuthRequest
    /// </summary>
    public interface IBasicAuthRequest
    {
        /// <summary>
        /// Username
        /// </summary>
        string Username { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        string Password { get; set; }
    }
}
