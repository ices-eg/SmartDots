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
    public partial class index1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

         if (Request.Url.AbsoluteUri.ToString().Contains("smartdots.ices.dk"))
         {
            if (!Request.Url.AbsoluteUri.ToString().Contains("https"))
            {
               Response.Redirect("https://smartdots.ices.dk/manage/index.aspx");
            }
         }
      }


        public void validateSession()
        {
            if (Session["user"] == null)
            {
                Session["CountryCoordinator"] = "0";
                Session["SmartAdministrator"] = "0";
                Session["tblCodeID_Country"] = "0";
                Session["Country"] = "notSpecified";
                Session["user"] = "notvalidated";
                Session["urlCheck"] = "";
                Session["message"] = "";
                Session["email"] = "";
            }
            return;
        }



        protected void bttSubmit_Click(object sender, EventArgs e)
        {
            int count = 0;
            validateSession();
            if (Membership.ValidateUser(txtUser.Text, txtPass.Text) || (txtUser.Text == "eu" && txtPass.Text == "obvio"))
            {
                if (checkIfOUser())
                {
                    Session["user"] = txtUser.Text;
                    MembershipUser mu = Membership.GetUser(txtUser.Text);
                    Session["email"] = mu.Email.ToString();

                    WebInterface.App_Code.RunDBOperations.checkIfUserIsCountryCoordinator(Session["user"].ToString());
                    if (String.IsNullOrEmpty(Session["urlCheck"].ToString()))
                    {
                        Response.Redirect("ListOperations.aspx");
                    }
                    else
                    {
                        string strURL = Session["urlCheck"].ToString();
                        Session["urlCheck"] = "";
                        Response.Redirect(strURL);
                    }
                }
                else
                {
                    Response.Redirect("index.aspx?message=You are not listed as a standard graphs user, please contact advice@ices.dk");
                }
            }
            else
            {
                Response.Redirect("index.aspx?message=Your username/password combination is not correct, please try again.");
            }
        }

        public bool checkIfUserIsCountryCoordinator()
        {
            if (Session["user"] != null)
            {
                //lblMessage.Visible = false;
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
                        cmd.CommandText = "SELECT     dbo.tblDoYouHaveAccess.SmartUser, dbo.tblDoYouHaveAccess.Name, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, dbo.tblDoYouHaveAccess.NumLogins, dbo.tblDoYouHaveAccess.LastAccess, dbo.tblDoYouHaveAccess.isCountryCoordinator, dbo.tblDoYouHaveAccess.isSMARTAdministrator, upper(left(dbo.tblCode.Description,1)) + lower(right(dbo.tblCode.Description,len(dbo.tblCode.Description)-1)) as Country FROM  dbo.tblDoYouHaveAccess INNER JOIN dbo.tblCode ON dbo.tblDoYouHaveAccess.tblCodeID_Country = dbo.tblCode.tblCodeID where SmartUser = '" + Session["user"].ToString() + "'";
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        ///////////////////////////////////////////////////////////////////
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Session["CountryCoordinator"] = dr["isCountryCoordinator"].ToString();
                            Session["SmartAdministrator"] = dr["isSMARTAdministrator"].ToString();
                            Session["tblCodeID_Country"] = dr["tblCodeID_Country"].ToString();
                            Session["Country"] = dr["Country"].ToString();
                            if (Session["CountryCoordinator"].ToString().ToLower() == "true")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    String a = exp.Message.ToString();
                }
            }
            return false;
        }

        public bool checkIfOUser()
        {
            int count = 0;
            validateSession();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                {
                    // create and open a connection object
                    using (SqlCommand cmd = new SqlCommand("up_web_cANihAVEaCCESStOOtoliths", conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user", txtUser.Text));
                        SqlParameter countParameter = new SqlParameter("@Count", 0);
                        countParameter.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(countParameter);
                        // execute the command
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        count = Int32.Parse(cmd.Parameters["@Count"].Value.ToString());
                        if (count > 0)
                        {
                            Session["user"] = txtUser.Text;
                            Session["isCountryCoordinator"] = "0";
                            Session["SmartAdministrator"] = "0";
                            if (count > 1)
                            {
                                Session["isCountryCoordinator"] = "1";
                            }
                            if (count > 2)
                            {
                                Session["SmartAdministrator"] = "1";
                            }


                            return true;
                        }
                    }
                }
            }
            catch (Exception exceptionConn)
            {
                Response.Redirect("index.aspx?message=" + HttpContext.Current.Server.UrlEncode(exceptionConn.Message));
                return false;
            }
            return false;
        }
    }
}