namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// IOAuth2MailClientConfig
    /// </summary>
    public interface IOAuth2MailClientConfig
    {
        /// <summary>
        /// OAuth2TokenizeRequest
        /// </summary>
        IOAuth2TokenizeRequest OAuth2TokenizeRequestInfo { get; set; }

        /// <summary>
        /// MailDataStore
        /// </summary>
        IMailDataStore MailDataStore { get; set; }

    }
}