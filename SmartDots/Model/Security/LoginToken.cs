using System;

namespace SmartDots.Model.Security
{
    public class LoginToken
    {
        public string TokenId { get; set; }
        public string ApiUrl { get; set; }
        public string User { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
