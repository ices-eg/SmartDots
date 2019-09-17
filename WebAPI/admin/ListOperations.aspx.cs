using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.admin
{
   public partial class ListOperations : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         if (!RunDBOperations.checkIfUserIsAdministrator(Session["user"].ToString()))
         {
            Response.Redirect("index.aspx?messaage=user not autorized to view this page.");
         }
      }
   }
}