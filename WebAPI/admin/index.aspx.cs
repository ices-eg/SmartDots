using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WebInterface.admin
{
    public partial class index : System.Web.UI.Page
    {
        Webinterface.manage.index1 i;
        protected void Page_Load(object sender, EventArgs e)
        {
            i = new Webinterface.manage.index1();
        }

        protected void bttSubmit_Click(object sender, EventArgs e)
        {


            int count = 0;
            i.validateSession();
            if (Membership.ValidateUser(txtUser.Text, txtPass.Text))
            {
                
                if (txtUser.Text == "carlos" || txtUser.Text == "anna" || txtUser.Text == "DaviesJ" || txtUser.Text == "colin" || txtUser.Text == "michala")
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
        
    }
}