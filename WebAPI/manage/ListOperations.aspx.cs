using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
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
      }
    }
}