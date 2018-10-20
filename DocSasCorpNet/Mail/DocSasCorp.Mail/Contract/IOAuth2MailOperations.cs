using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocSasCorp.Mail.Contract;
using DocSasCorp.Mail.Concrete;

namespace DocSasCorp.Mail.Contract
{
    public interface IOAuth2MailOperations
    {
        /// <summary>
        /// GetAuthorizationUrl
        /// </summary>
        /// <returns></returns>
        string GetAuthorizationUrl();

        /// <summary>
        /// AuthenticateAsync
        /// </summary>
        /// <returns></returns>
        Task<OAuth2AccessResponse> AuthenticateAsync(string code);

        /// <summary>
        /// GetAuthenticationDetailAsync
        /// </summary>
        /// <returns></returns>
        Task<OAuth2AccessResponse> GetAuthenticationDetailAsync();

    }
}
