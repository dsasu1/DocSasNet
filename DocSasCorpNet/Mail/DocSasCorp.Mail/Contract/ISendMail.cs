using System.Threading.Tasks;
using DocSasCorp.Mail.Concrete;

namespace DocSasCorp.Mail.Contract
{
    /// <summary>
    /// ISendMail
    /// </summary>
    public interface ISendMail
    {
        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SendMailResponse> SendEmailAsync(ISendMailRequest request);
    }
}