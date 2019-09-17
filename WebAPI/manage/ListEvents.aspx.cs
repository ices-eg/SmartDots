using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class ListEvents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebInterface.App_Code.RunDBOperations.validateSession();
            if (Session["user"] == "notvalidated")
            {
               Response.Redirect("index.aspx?Session has expired!");
            }

      }

      protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
      {
         bool blnCanViewEvent = false;
         if (e.Row.RowType == DataControlRowType.DataRow)
         {

            HyperLink hlnkViewEvent = (HyperLink)e.Row.FindControl("hlnkViewEvent");
            if (int.Parse(DataBinder.Eval(e.Row.DataItem, "canUserViewEvent").ToString()) > 0)
            {
               blnCanViewEvent = true;
            }
            if (!blnCanViewEvent)
            {
               hlnkViewEvent.NavigateUrl = null;
            }
         }
      }

      protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
      {
         hplnkDownloadList.NavigateUrl = "../download/downloadListEvents.ashx?year=" + ddlYear.SelectedValue.ToString();
      }
   }
}