using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Contract;

namespace DocSasCorp.Mail.Concrete
{
    /// <summary>
    /// OAuth2AuthorizeRequest
    /// </summary>
    public class OAuth2AuthorizeRequest : IOAuth2AuthorizeRequest
    {
        /// <summary>
        /// ClientId
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// AuthorizationEndPointUrl
        /// </summary>
        public string AuthorizationEndPointUrl { get; set; }
        /// <summary>
        /// RedirectUrl
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// ClientSecret
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// TokenEndPointUrl
        /// </summary>
        public string TokenEndPointUrl { get ; set; }
        /// <summary>
        /// AuthorizedCode
        /// </summary>
        public string AuthorizedCode { get; set; }
    }
}
