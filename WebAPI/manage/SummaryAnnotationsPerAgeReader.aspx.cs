using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.manage
{
   public partial class SummaryAnnotationsPerAgeReader : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         RunDBOperations.validateSession();
         Page.Form.Attributes.Add("enctype", "multipart/form-data");

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         /////////////////////// CHECKs IF THE EVENT WAS SPECIFIED \\\\\\\\\\\\\\\\\\
         string strEventID = Request.QueryString["tblEventID"];
         if (string.IsNullOrEmpty(strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=Event was not specified!");
         }

         /////////////////////// THIS WILL CHECK IF THE USER IS AN EVENT MANAGER \\\\\\\\\\\\\\\\\\
         if (!RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=user does not have permission to manage this event!");
         }
      }
   }
}