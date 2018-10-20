namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IOAuth2AuthorizeRequest
    /// </summary>
    public interface IOAuth2AuthorizeRequest
    {
        /// <summary>
        /// ClientId
        /// </summary>
        string ClientId { get; set; }

        /// <summary>
        /// AuthorizationEndPointUrl
        /// </summary>
        string AuthorizationEndPointUrl { get; set; }

        /// <summary>
        /// RedirectUrl
        /// </summary>
        string RedirectUrl { get; set; }

        /// <summary>
        /// Scope
        /// </summary>
        string Scope { get; set; }

        /// <summary>
        /// State
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// ClientSecret
        /// </summary>
        string ClientSecret { get; set; }

        /// <summary>
        /// TokenEndPointUrl
        /// </summary>
        string TokenEndPointUrl { get; set; }

        /// <summary>
        /// AuthorizedCode
        /// </summary>
        string AuthorizedCode { get; set; }

    }
}