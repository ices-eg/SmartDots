using SmartDots.Helpers;

namespace SmartDots.Model.Security
{
    public class DtoUserAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DtoAuthenticationMethod DtoAuthenticationMethod { get; set; }
    }
}
