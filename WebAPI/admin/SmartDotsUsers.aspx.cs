using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using WebInterface.App_Code;

namespace WebInterface.admin
{
    public partial class SmartDotsUsers : System.Web.UI.Page
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

        protected void lnkUpdateEmails_Click(object sender, EventArgs e)
        {

            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand();
                    DataSet ds = new DataSet();
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandText = "select SmartUser from tblDoYouHaveAccess";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    da.SelectCommand = cmd;
                    da.Fill(ds);
                    ///////////////////////////////////////////////////////////////////
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string strUser = dr["SmartUser"].ToString();
                        MembershipUser mu = Membership.GetUser(strUser);
                        string strUpdateEmail = "update tblDoYouHaveAccess set Email = '"  + mu.Email.ToString() + "' where SmartUser = '" + strUser + "'" ;
                        WebInterface.App_Code.RunDBOperations.runDBSmartDotsSQL(strUpdateEmail);
                    }
                }
            }
            catch (Exception exp)
            {
                String a = exp.Message.ToString();
            }
            return;
        }

      protected void bttAdduser_Click(object sender, EventArgs e)
      {

         if (txtNameUser.Text.Length > 1)
         {
            MembershipUser mu = null;

            string userName = Membership.GetUserNameByEmail(txtNameUser.Text.Trim());
            if (userName != null)
            {
               mu = Membership.GetUser(userName);
               if (mu != null)
               {
                  string strSQL = "";
                  try
                  {
                     using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                     {
                        // create and open a connection object
                        using (SqlCommand cmd = new SqlCommand(strSQL, conn))
                        {
                           cmd.CommandTimeout = 0;
                           cmd.CommandText = "up_web_insertNewSmartUser";
                           cmd.CommandType = CommandType.StoredProcedure;
                           cmd.Parameters.Add(new SqlParameter("@tblCodeID_Country", ddlListCountries.SelectedItem.Value.ToString()));
                           cmd.Parameters.Add(new SqlParameter("@user", mu.UserName.ToString()));
                           cmd.Parameters.Add(new SqlParameter("@email", mu.Email.ToString()));
                           conn.Open();
                           cmd.ExecuteNonQuery();
                        }
                     }
                     lblMessage.Text = "The user " + txtNameUser.Text + " was added successfully.<br>";
                  }
                  catch (Exception exceptionConn)
                  {
                     Response.Redirect("index.aspx?message=" + HttpContext.Current.Server.UrlEncode(exceptionConn.Message));
                     // Response.Redirect("index.aspx?error=" + HttpContext.Current.Server.UrlEncode(exceptionConn.Message + " " + exceptionConn.ToString()));
                     return;
                  }
               }
               else
               {
                  lblMessage.Text = "The user " + txtNameUser.Text + " could not be added because it is not part of the list of ICES users.<br>";
               }
            }
            else
            {
               lblMessage.Text = "Email: " + txtNameUser.Text + " was not found, please check if it is correct.";
            }
         }
         else
         {
            lblMessage.Text = "Please specify the name of the user.";
         }
      }
   }
}