namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// ISendMailResponse
    /// </summary>
    public interface ISendMailResponse
    {
        /// <summary>
        /// IsSuccess
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        string ErrorMessage { get; set; }
    }
}