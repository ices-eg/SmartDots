using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.manage
{
   public partial class AnonimisedNameForSmartUsers : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         RunDBOperations.validateSession();
         Page.Form.Attributes.Add("enctype", "multipart/form-data");

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         /////////////////////// CHECKs IF THE EVENT WAS SPECIFIED \\\\\\\\\\\\\\\\\\
         string strEventID = Request.QueryString["tblEventID"];
         if (string.IsNullOrEmpty(strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=Event was not specified!");
         }

         /////////////////////// THIS WILL CHECK IF THE USER IS AN EVENT MANAGER \\\\\\\\\\\\\\\\\\
         if (!RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=user does not have permission to manage this event!");
         }

         try
         {

            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection _sqlConn = new SqlConnection(ConnectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = _sqlConn;
               command.CommandText = "updateAnonimisedNameForReaders";
               command.CommandType = CommandType.StoredProcedure;
               string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();
               command.Parameters.Add(new SqlParameter("@tblEventID", strEventID));
               _sqlConn.Open();
               command.ExecuteReader();
            }
         }
         catch (Exception err)
         {
            SmartUtilities.saveToLog(err);
            lblMessage.Text = "Message: Please check if the message is correct: " + err.Message.ToString();
            lblMessage.ForeColor = Color.Red;
            SmartUtilities.saveToLog(err);
            return;
         }
      }
   }
}