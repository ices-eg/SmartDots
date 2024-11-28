using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
   public class WebApiResult
   {
      public dynamic Result { get; set; }
      public string ErrorMessage { get; set; }
      public string WarningMessage { get; set; }
   }
}