using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public class DtoUser
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string Token { get; set; }
    }
}