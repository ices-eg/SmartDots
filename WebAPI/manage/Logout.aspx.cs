using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface.manage
{
   public partial class Logout : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         Session["user"] = "notvalidated";
         Response.Redirect("../index.aspx");
      }
   }
}