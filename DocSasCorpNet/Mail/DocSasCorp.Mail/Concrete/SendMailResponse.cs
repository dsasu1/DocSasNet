using DocSasCorp.Mail.Contract;

namespace DocSasCorp.Mail.Concrete
{
    /// <summary>
    /// SendMailResponse
    /// </summary>
    public class SendMailResponse : ISendMailResponse
    {
        /// <summary>
        /// IsSuccess
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}