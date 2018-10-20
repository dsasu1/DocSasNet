using System;
using System.Collections.Generic;
using System.Text;
using DocSasCorp.Mail.Contract;

namespace DocSasCorp.Mail.Concrete
{
    public class OAuth2AccessResponse : IOAuth2AccessResponse
    {
        public bool IsSucces { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
