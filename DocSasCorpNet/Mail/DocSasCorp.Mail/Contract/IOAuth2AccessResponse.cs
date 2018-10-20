using System;
using System.Collections.Generic;
using System.Text;

namespace DocSasCorp.Mail.Contract
{
    public interface IOAuth2AccessResponse
    {
        /// <summary>
        /// IsSucces
        /// </summary>
        bool IsSucces { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        string Token { get; set; }
        /// <summary>
        /// ErrorMessage
        /// </summary>
        string ErrorMessage { get; set; }
    }
}
