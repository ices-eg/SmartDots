using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class ManageEvent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

           WebInterface.App_Code.RunDBOperations.validateSession();
           if (Session["user"] == "notvalidated")
            {
                Response.Redirect("index.aspx?Session has expired!");
            }
        }

        protected void fillTheEventDetails(string strEventID)
        {
            
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                HyperLink hlnkEditEvent = (HyperLink)e.Row.FindControl("hlnkEditEvent");
                if (int.Parse(DataBinder.Eval(e.Row.DataItem, "EventCoordinator").ToString()) > 0)
                {
                    hlnkEditEvent.Visible = true;
                }
                else
                {
                    hlnkEditEvent.Visible = false;
                }
            }
        }
    }
}