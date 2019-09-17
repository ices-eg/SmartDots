using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface
{
   public partial class ViewListEvents : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {

      }

      protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
      {
         if (e.Row.RowType == DataControlRowType.DataRow)
         {
            HyperLink hlnk = (HyperLink)e.Row.FindControl("hplnkViewEvent");
            string strPublished = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Published"));
            HyperLink hlnkReport = (HyperLink)e.Row.FindControl("hlnkReports");

            if (strPublished.Equals("No"))
            {
               hlnk.NavigateUrl = null;
               hlnkReport.Visible = false;
            }
         }
      }
   }
}