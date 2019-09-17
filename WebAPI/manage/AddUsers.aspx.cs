using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webinterface.manage
{
    public partial class AddUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WebInterface.App_Code.RunDBOperations.validateSession();
            string strMessage = Request.QueryString["Message"];
            if (!string.IsNullOrEmpty(strMessage))
            {
                lblMessage.Visible = true;
                if (Session["message"].ToString().Length > 5)
                   // lblMessage.Text = strMessage;
                {
                    lblMessage.Text = "<br>" + Session["message"].ToString();
                }
            }


            if ("notvalidated".Equals(Session["user"].ToString()))
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }
            if (WebInterface.App_Code.RunDBOperations.checkIfUserIsCountryCoordinator(Session["user"].ToString()))
            {
                lblCountryName.Text = "List of current users for : " + Session["Country"].ToString();
            }
            else
            {
                Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
                Response.Redirect("index.aspx");
            }


        }
       

        protected void bttAddUsers_Click(object sender, EventArgs e)
        {
            string strMessage = "";
            string strUsers = txtUsers.Text;
            if (strUsers.Length > 1)
            {

                string[] words = strUsers.Split(',');
                foreach (string user in words)
                {
                    if (user.Length > 1)
                    {
                        MembershipUser mu = Membership.GetUser(user.Trim());
                        try
                        {

                            if (mu == null) // Then it tries to see if it is the email.
                            {
                                if (user != null)
                                {
                                    if (user.Length > 2)
                                    {
                                        string userName = Membership.GetUserNameByEmail(user.Trim());
                                        if (userName != null)
                                        {
                                            if (userName.Length > 2)
                                            {
                                                mu = Membership.GetUser(userName);
                                            }
                                        }
                                    }
                                }


                            }
                        }
                        catch (Exception exceptionE)
                        {
                            strMessage += "<br>" + exceptionE.Message.ToString() + " : " + user.ToString() + "<br>";
                        }
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
                                        cmd.Parameters.Add(new SqlParameter("@tblCodeID_Country", Session["tblCodeID_Country"].ToString()));
                                        cmd.Parameters.Add(new SqlParameter("@user", mu.UserName.ToString()));
                                        cmd.Parameters.Add(new SqlParameter("@email", mu.Email.ToString()));
                                        conn.Open();
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                strMessage += "The user " + user + " was added successfully.<br>";
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
                            strMessage += "The user " + user + " could not be added because it is not part of the list of ICES users.<br>";
                        }
                    }
                }
            }
            else
            {
                Response.Redirect("AddUsers.aspx?Message=Please list some users before pushing the add users button.");
            }
            Session["message"] = strMessage.ToString();
            Response.Redirect("AddUsers.aspx?Message=Users added successfully ");

        }
    }
}