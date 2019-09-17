using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class showSampleImages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strEventID = Request.QueryString["eventID"];
            string strsample = Request.QueryString["sample"];
            if (string.IsNullOrEmpty(strEventID))
            {
                Response.Redirect("index.aspx?message=event was not specified");
            }
            if (string.IsNullOrEmpty(strsample))
            {
                Response.Redirect("index.aspx?message=sample was not specified");
            }
            fillTheEventDetails(strEventID, strsample);
        }


        protected void fillTheEventDetails(string strEventID, string strSampleID)
        {
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string strSQL = " SELECT * FROM [dbo].[vw_SmartImages] ";
                    strSQL = strSQL + " where tblEventID = @tblEventID AND tblSampleID = @tblSampleID";
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                    {
                        cmd.Parameters.Add("@tblEventID", SqlDbType.Int);
                        cmd.Parameters["@tblEventID"].Value = strEventID;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@tblSampleID", SqlDbType.NVarChar);
                        cmd.Parameters["@tblSampleID"].Value = strSampleID;
                        cmd.CommandType = CommandType.Text;
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            lblEventName.Text = rdr["SampleID"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                            string strFileName = rdr["URL"].ToString() ;
                            Image imgOtolite = new Image();
                            imgOtolite.ImageUrl= strFileName;
                            imgOtolite.Width = 1024;

                            Label lblText = new Label();
                            lblText.Text = "SampleID = " + strSampleID;
                            pnlImages.Controls.Add(new LiteralControl("<br />"));
                            pnlImages.Controls.Add(lblText);
                            pnlImages.Controls.Add(new LiteralControl("<br />"));
                            pnlImages.Controls.Add(imgOtolite);
                            pnlImages.Controls.Add(new LiteralControl("<br />"));


                        }
                    }
                }
            }
            catch (Exception exp)
            {
                String a = exp.Message.ToString();
            }
        }
    }
}