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
   public partial class viewMaturitySampleAnnotation : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         RunDBOperations.validateSession();

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         string strSampleID = Request.QueryString["sampleID"];
         string strEventID = Request.QueryString["tblEventID"];
         if (string.IsNullOrEmpty(strSampleID))
         {
            Response.Redirect("index.aspx?message=The image was not specified");
         }

         fillTheEventDetails(strSampleID);

         if (RunDBOperations.intExecuteScalarQuery("select tblAnnotationsMaturityID  from tblAnnotationsMaturity where tblEventID = " + strEventID + " and FishID in (select fishid from tblSamples where tblSampleID = " + strSampleID + ") and SmartUser = '" + Session["user"] + "'") > 0)
         {
            bttUpdateAnnotationDatabase.Text = "Update annotation in the database";
            if (!Page.IsPostBack)
            {
               txtComments.Text = RunDBOperations.executeScalarQuery("select Comment from tblAnnotationsMaturity where tblEventID = " + strEventID + " and FishID in (select fishid from tblSamples where tblSampleID = " + strSampleID + ") and SmartUser = '" + Session["user"] + "' ");
            }
         }


         if (RunDBOperations.checkIfEventIsClosed(strEventID))
         {
            bttUpdateAnnotationDatabase.Visible = false;
         }

      }
      protected void fillTheEventDetails(string strSampleID)
      {
         bool bnlFirst = true;
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT * FROM [dbo].[vw_SmartImages] ";
               strSQL = strSQL + " WHERE (VisibleDuringEvent = 1 or closed = 1 ) and FishID IN  (select FishID from tblSamples where tblsampleid = @tblsampleid )";
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSampleID", SqlDbType.Int);
                  cmd.Parameters["@tblSampleID"].Value = strSampleID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     if (bnlFirst)
                     {
                        if (!string.IsNullOrEmpty(rdr["Area"].ToString()))
                        {
                           lblArea.Text = rdr["Area"].ToString();
                        }
                        if (!string.IsNullOrEmpty(rdr["FishID"].ToString()))
                        {
                           Label1.Text = rdr["FishID"].ToString();
                        }
                        if (!string.IsNullOrEmpty(rdr["FishLength"].ToString()))
                        {
                           lblLenght.Text = rdr["FishLength"].ToString();
                        }
                        if (!string.IsNullOrEmpty(rdr["FishWeight"].ToString()))
                        {
                           lblWeight.Text = rdr["FishWeight"].ToString();
                        }
                        if (!string.IsNullOrEmpty(rdr["CatchDate"].ToString()))
                        {
                           lblDate.Text = rdr["CatchDate"].ToString();
                        }
                        bnlFirst = false;
                     }
                     lblEventName.Text = rdr["SampleID"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                     string strFileName = rdr["URL"].ToString();
                     string fileName = "../SampleImages/" + rdr["intYear"].ToString() + "/" + rdr["tblEventID"].ToString() + "/" + rdr["fileName"].ToString();

                     Label lblText = new Label();
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     pnlImages.Controls.Add(lblText);
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     HyperLink hplnkImage = new HyperLink();
                     hplnkImage.NavigateUrl = rdr["URL"].ToString();
                     hplnkImage.Target = "_blank";
                     hplnkImage.ImageUrl = rdr["URL"].ToString();
                     hplnkImage.ImageWidth = Unit.Pixel(640);
                     pnlImages.Controls.Add(hplnkImage);
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     // We will try to render a canvas here to be able to draw a line and the dots
                     //pnlImages.Controls.Add(new LiteralControl("<canvas id='othImage" + strSampleID + "' width='600' height='450' style='background-repeat:no-repeat;background-image:url(" + rdr["URL"].ToString() + ");background-size: 100% 100%;border:0px;'> Your browser does not support the canvas element.</canvas>"));
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                  }
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
            txtComments.Text = exp.Message.ToString();
            //            Response.Redirect("index.aspx?message=" + exp.Message.ToString());
         }
      }


      private bool updateAnnotation(string strEventID, string strSampleID)
      {
         string strMessage = "Thanks, you annotation is now saved in the database.";

         try
         {
            string strFishID = RunDBOperations.executeScalarQuery("select fishID from tblSamples where tblEventID = " + strEventID + " and tblsampleid = " + Request.QueryString["sampleID"].ToString());

            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection _sqlConn = new SqlConnection(ConnectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = _sqlConn;
               command.CommandText = "up_WebAPI_createMaturityAnnotation";
               command.CommandType = CommandType.StoredProcedure;
               string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();
               command.Parameters.Add(new SqlParameter("@tblEventID", strEventID));
               command.Parameters.Add(new SqlParameter("@tblSampleID", strSampleID));
               command.Parameters.Add(new SqlParameter("@fishID", strFishID));
               command.Parameters.Add(new SqlParameter("@tblCodeID_Sex", ddlSex.SelectedValue.ToString()));
               command.Parameters.Add(new SqlParameter("@tblCodeID_Maturity", ddlMaturity.SelectedValue.ToString()));
               command.Parameters.Add(new SqlParameter("@Comment", txtComments.Text.ToString()));
               command.Parameters.Add(new SqlParameter("@user", Session["user"].ToString()));
               _sqlConn.Open();
               command.ExecuteReader();
               lblMessage.Text = strMessage;
               lblMessage.ForeColor = Color.Red;
            }
         }
         catch (Exception err)
         {
            SmartUtilities.saveToLog(err);
            lblMessage.Text = "Message: " + err.Message.ToString();
            lblMessage.ForeColor = Color.Red;
            SmartUtilities.saveToLog(err);
            return false;
         }
         return true;
      }

      protected void bttUpdateAnnotationDatabase_Click(object sender, EventArgs e)
      {
         string strEventID = Request.QueryString["tblEventID"].ToString();
         string strSampleID = Request.QueryString["sampleID"].ToString();
         if (int.Parse(ddlSex.SelectedValue) < 1)
         {
            lblMessage.Text = "To be able to annotate you need to specify the sex of the fish!";
            lblMessage.Visible = true;
            lblMessage.ForeColor = Color.Red;
            return;
         }
         if (int.Parse(ddlMaturity.SelectedValue) < 1)
         {
            lblMessage.Text = "To be able to annotate you need to specify the maturity stage!";
            lblMessage.Visible = true;
            lblMessage.ForeColor = Color.Red;
            return;
         }

         if (updateAnnotation(strEventID, strSampleID))
         {
            Response.Redirect("ViewMaturityEvent?tblEventID=" + strEventID + "&Operation=Succeeded", false);
         }
         return;
      }
   }
}