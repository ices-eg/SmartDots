using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class CreateToken : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            WebInterface.App_Code.RunDBOperations.validateSession();


            if ("notvalidated".Equals(Session["user"].ToString()))
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }
        }

      

        protected void bttSubmit_Click(object sender, EventArgs e)
        {
            
            string strSQL = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    using (SqlCommand cmd = new SqlCommand("up_web_createToken", cn))
                    {
                        cn.Open();
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user", Session["user"].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@numberDays", ddlNumberOfDays.SelectedValue));
                        cmd.ExecuteNonQuery();
                    }
                }
                Response.Redirect("CreateToken.aspx");
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}