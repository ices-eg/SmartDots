using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public class DtoUserAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DtoAuthenticationMethod DtoAuthenticationMethod { get; set; }
    }

    public enum DtoAuthenticationMethod { Windows, Basic }
}