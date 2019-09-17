using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class viewScreenResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ("notvalidated".Equals(Session["user"].ToString()))
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["sessionid"].ToString()))
            {
                Response.Redirect("index.aspx");
            }
            DataView dvSql = (DataView)SqlHeaderDataSource.Select(DataSourceSelectArguments.Empty);
            foreach (DataRowView drvSql in dvSql)
            {
                lblCountry.Text = drvSql["Country"].ToString();
                lblDataset.Text = drvSql["Description"].ToString();
                lblMYear.Text = drvSql["usryear"].ToString();
                lblFileName.Text = drvSql["filename"].ToString();
                lblSubmitingDate.Text = drvSql["sess_date"].ToString();
 //               lblMaxErrorsReturn.Text = drvSql["errlimit"].ToString();
                MembershipUser mu = Membership.GetUser(Session["user"].ToString());
                string strEmailUser = mu.Email.ToString();
                lblEmail.Text = strEmailUser;
 //               lblNumRecordsFile.Text = drvSql["records"].ToString();

               

                hplnkViewReport.NavigateUrl = "http://admin.ices.dk/DatsuReports/DatsuErrorReport.aspx?id=" + drvSql["tblSU_SessionID"].ToString();
                hplnkViewReport.Visible = true;
            }

            string strGroupError = Request.QueryString["groupError"];
            if (!string.IsNullOrEmpty(strGroupError))
            {
                hplnCleanFilter.Visible = true;
                hplnCleanFilter.NavigateUrl = "viewScreenResult.aspx?sessionid=" + Request.QueryString["sessionid"].ToString();

                string strSQL = "SELECT  tblSU_ReportedErrors.tblSU_SessionID, tblSU_ReportedErrors.Line, tblSU_Error.description AS type, tblSU_ReportedErrors.ErrValue,  tblSU_ReportedErrors.ErrField, tblSU_Check.description, tblSU_Check.CriticalOnDome FROM         tblSU_ReportedErrors INNER JOIN                      tblSU_Check ON tblSU_ReportedErrors.CheckID = tblSU_Check.tblSU_CheckID INNER JOIN                    tblSU_Error ON tblSU_Check.tblSU_ErrorID = tblSU_Error.tblSU_ErrorID WHERE    ( ErrField not like  'Summaries of data: Timeout expired%' ) and (tblSU_ReportedErrors.Line IS NOT NULL) AND (tblSU_ReportedErrors.tblSU_SessionID = " + Request.QueryString["sessionid"] + ") AND (dbo.tblSU_Check.tblSU_CheckID = " + Request.QueryString["groupError"] + ")";
                SQLdtsCheckErrors.SelectCommand = strSQL;
                SQLdtsCheckErrors.DataBind();
                GridView2.DataBind();
            }
        }
    }
}