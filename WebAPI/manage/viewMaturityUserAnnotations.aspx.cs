using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.manage
{
   public partial class viewMaturityUserAnnotations : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         RunDBOperations.validateSession();

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         // This is a temporary fix to be able to test the events has otehr users 
         if (!string.IsNullOrEmpty(Request.QueryString["user"]))
         {
            Session["user"] = Request.QueryString["user"].ToString();
         }
         
         string strSampleID = Request.QueryString["SampleID"];
         string strEventID = Request.QueryString["tblEventID"];
         if (string.IsNullOrEmpty(strSampleID))
         {
            Response.Redirect("index.aspx?message=The image was not specified");
         }

         fillTheEventDetails(strSampleID);
         fillTheAnnotationsDetails(strSampleID, strEventID);
      }
      protected void fillTheAnnotationsDetails(string strSampleID, string strEventID)
      { 
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT dbo.tblAnnotationsMaturity.tblAnnotationsMaturityID, dbo.tblAnnotationsMaturity.tblEventID, dbo.tblAnnotationsMaturity.Comment, dbo.tblAnnotationsMaturity.SmartUser, dbo.tblAnnotationsMaturity.CreateDate, Sex.Description as Sex, Maturity.Description AS Maturity ";
               strSQL = strSQL + " FROM dbo.tblAnnotationsMaturity INNER JOIN dbo.tblCode AS Maturity ON dbo.tblAnnotationsMaturity.tblCodeID_Maturity = Maturity.tblCodeID INNER JOIN dbo.tblCode AS Sex ON dbo.tblAnnotationsMaturity.tblCodeID_Sex = Sex.tblCodeID ";
               strSQL = strSQL + " WHERE (dbo.tblAnnotationsMaturity.FishID IN (SELECT FishID FROM dbo.tblSamples WHERE (tblSampleID = @tblSampleID))) AND (dbo.tblAnnotationsMaturity.tblEventID = @tblEventID)";
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSampleID", SqlDbType.Int);
                  cmd.Parameters["@tblSampleID"].Value = strSampleID;
                  cmd.Parameters.Add("@tblEventID", SqlDbType.Int);
                  cmd.Parameters["@tblEventID"].Value = strEventID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  int intRecNo = 0;
                  while (rdr.Read())
                  {      
                     intRecNo++;
                     string strColor = intRecNo % 2 == 0 ? "#F15D2A" : "#2b95a0"; // Sets if the color will be orange or green (ICES colors)
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br /><br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("AnnotationID : " + rdr["tblAnnotationsMaturityID"].ToString(), 14, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("SmartUser : " + rdr["SmartUser"].ToString(), 14, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("Sex : " + rdr["Sex"].ToString(), 14, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("Maturity: " + rdr["Maturity"].ToString(), 14, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     if (rdr["Comment"] != null)
                     {
                        if (!string.IsNullOrEmpty(rdr["Comment"].ToString()))
                        {
                           pnlAnnotationDetails.Controls.Add(getLabel("Comment : " + rdr["Comment"].ToString(), 14, strColor));
                           pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                        }
                     }

                  }
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }
      }

      protected Label getLabel(string strText, int size, string strColor)
      {
         Label lblText = new Label();
         lblText.Font.Size = size;
         lblText.Font.Name = "Calibri";
         lblText.Text = strText;
         lblText.ForeColor = System.Drawing.ColorTranslator.FromHtml(strColor);
         return lblText;
      }
      protected void fillTheEventDetails(string strSampleID)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {

               string strSQL = " SELECT * FROM [dbo].[vw_SmartImages] ";
               strSQL = strSQL + " where (VisibleDuringEvent = 1 or closed = 1 ) and tblsampleid =  @tblsampleid";
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSampleID", SqlDbType.Int);
                  cmd.Parameters["@tblSampleID"].Value = strSampleID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     lblArea.Text = rdr["Area"].ToString();
                     lblfishID.Text = rdr["FishID"].ToString();
                     lblLenght.Text = rdr["FishLength"].ToString();
                     lblWeight.Text = rdr["FishWeight"].ToString();
                     lblDate.Text = rdr["CatchDate"].ToString();


                     lblEventName.Text = rdr["SampleID"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                     string strFileName = rdr["URL"].ToString();
                     string fileName = "../SampleImages/" + rdr["intYear"].ToString() + "/" + rdr["tblEventID"].ToString() + "/" + rdr["fileName"].ToString();

                     Label lblText = new Label();
                     //                            lblText.Text = "SampleID = " + strSampleID;
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
         }
      }
   }
}