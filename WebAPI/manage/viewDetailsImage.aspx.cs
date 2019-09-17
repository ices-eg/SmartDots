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
   public partial class viewDetailsImage : System.Web.UI.Page
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

         string strSmartImageID = Request.QueryString["SmartImageID"];
         if (string.IsNullOrEmpty(strSmartImageID))
         {
            Response.Redirect("index.aspx?message=The image was not specified");
         }

         fillTheEventDetails(strSmartImageID);
         fillTheAnnotationDetails(strSmartImageID);
      }

      protected void fillTheEventDetails(string strSmartImageID)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT * FROM [dbo].[vw_SmartImages] ";
               strSQL = strSQL + " where tblSmartImageID =  @tblSmartImageID";
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSmartImageID", SqlDbType.Int);
                  cmd.Parameters["@tblSmartImageID"].Value = strSmartImageID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     lblArea.Text = rdr["Area"].ToString();
                     lblSex.Text = rdr["sex"].ToString();
                     lblLenght.Text = rdr["FishLength"].ToString();
                     lblWeight.Text = rdr["FishWeight"].ToString();
                     lblDate.Text = rdr["CatchDate"].ToString();

                     int newWidth = 600;
                     lblEventName.Text = rdr["SampleID"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                     string strFileName = rdr["URL"].ToString();
                     string fileName = "../SampleImages/" + rdr["intYear"].ToString() + "/" + rdr["tblEventID"].ToString() + "/" + rdr["fileName"].ToString();
                     Bitmap img = new Bitmap(Server.MapPath(fileName));
                     int intWith = img.Width;
                     int intHeight = img.Height;

                     Label lblText = new Label();
                     //                            lblText.Text = "SampleID = " + strSampleID;
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     pnlImages.Controls.Add(lblText);
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     // We will try to render a canvas here to be able to draw a line and the dots
                     pnlImages.Controls.Add(new LiteralControl("<canvas id='othImage" + strSmartImageID + "' width='600' height='450' style='background-repeat:no-repeat;background-image:url(\"" + rdr["URL"].ToString() + "\");background-size: 100% 100%;border:0px;'> Your browser does not support the canvas element.</canvas>"));
                     string strDrawLinesAndDots = getJavaScriptToDrawLinesAndDots(strSmartImageID, intWith, intHeight, newWidth, 450);
                     ScriptManager.RegisterStartupScript(this, this.GetType(), "showLines", strDrawLinesAndDots, true);
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

      public string getJavaScriptToDrawLinesAndDots(string strSmartImageID, int intWidth, int intHeight, int intNewWidth, int intNewHeight)
      {
         string strJavaScript = "var c = document.getElementById('othImage" + strSmartImageID + "');var ctx=c.getContext('2d');";
         strJavaScript += getDrawDots(strSmartImageID, intWidth, intHeight, intNewWidth, intNewHeight);
         strJavaScript += getDrawLine(strSmartImageID, intWidth, intHeight, intNewWidth, intNewHeight);

         return strJavaScript;
      }

      private string getDrawDots(string strSmartImageID, int imgWidth, int imgHeigh, int newWidth, int newHeight)
      {
         string strDots = "";

         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " select * from [dbo].[vw_Dots]  INNER JOIN dbo.vw_PermissionForAnnotations ON dbo.vw_Dots.tblAnnotationID = dbo.vw_PermissionForAnnotations.tblAnnotationID ";
               strSQL = strSQL + " where vw_Dots.tblSmartImageID =  @tblSmartImageID and vw_PermissionForAnnotations.SmartUser = N'" + Session["user"].ToString() + "' ";
               if (ddlListReaders.SelectedIndex > 0)
               {
                  strSQL = strSQL + " and vw_Dots.SmartUser = '" + ddlListReaders.SelectedValue.ToString() + "'";
               }

               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSmartImageID", SqlDbType.Int);
                  cmd.Parameters["@tblSmartImageID"].Value = strSmartImageID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     int x = (int)((int.Parse(rdr["x"].ToString()) * newWidth) / imgWidth);
                     int y = (int)((int.Parse(rdr["y"].ToString()) * newHeight) / imgHeigh);
                     strDots += "drawDot(ctx, " + x.ToString() + ", " + y.ToString() + ", 2, '" + rdr["color"].ToString().Replace("#FF", "#") + "');";
                  }
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return strDots;

      }

      private string getDrawLine(string strSmartImageID, int imgWidth, int imgHeigh, int newWidth, int newHeight)
      {
         string strLines = "";
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " select * from [dbo].[vw_Lines] INNER JOIN dbo.vw_PermissionForAnnotations ON dbo.[vw_Lines].tblAnnotationID = dbo.vw_PermissionForAnnotations.tblAnnotationID ";
               strSQL = strSQL + " where [vw_Lines].tblSmartImageID =  @tblSmartImageID and vw_PermissionForAnnotations.SmartUser = N'" + Session["user"].ToString() + "' ";
               if (ddlListReaders.SelectedIndex > 0)
               {
                  strSQL = strSQL + " and vw_Lines.SmartUser = '" + ddlListReaders.SelectedValue.ToString() + "'";
               }
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSmartImageID", SqlDbType.Int);
                  cmd.Parameters["@tblSmartImageID"].Value = strSmartImageID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     int x1 = (int)((int.Parse(rdr["x1"].ToString()) * newWidth) / imgWidth);
                     int x2 = (int)((int.Parse(rdr["x2"].ToString()) * newWidth) / imgWidth);

                     //int newHeight = (int)(imgHeigh * newWidth) / imgWidth;
                     int y1 = (int)((int.Parse(rdr["y1"].ToString()) * newHeight) / imgHeigh);
                     int y2 = (int)((int.Parse(rdr["y2"].ToString()) * newHeight) / imgHeigh);
                     strLines += "drawLine(ctx, " + x1.ToString() + ", " + y1.ToString() + ", " + x2.ToString() + ", " + y2.ToString() + ",1, '" + rdr["color"].ToString().Replace("#FF", "#") + "');";
                  }
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return strLines;

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
      protected void fillTheAnnotationDetails(string strSmartImageID)
      {
         int intRecNo = 0;
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT        dbo.vw_Annotations.tblSmartImageID, dbo.vw_Annotations.GUID_SmartImage, dbo.vw_Annotations.GUID_AnnotationID, dbo.vw_Annotations.tblAnnotationID, dbo.vw_Annotations.tblEventID, dbo.vw_Annotations.tblSampleID, dbo.vw_Annotations.TypeAnotation, dbo.vw_Annotations.SmartUser, dbo.vw_Annotations.IsApproved, dbo.vw_Annotations.IsFixed, dbo.vw_Annotations.IsReadOnly, dbo.vw_Annotations.SmartDotsCreateDate,  dbo.vw_Annotations.CreateDate, dbo.vw_Annotations.Closed, dbo.vw_Annotations.Published, dbo.vw_Annotations.AQ_Code, AQCode, dbo.vw_Annotations.Comment, dbo.vw_Annotations.noDots FROM            dbo.vw_Annotations INNER JOIN dbo.vw_PermissionForAnnotations ON dbo.vw_Annotations.tblAnnotationID = dbo.vw_PermissionForAnnotations.tblAnnotationID ";
               strSQL = strSQL + " Where vw_Annotations.tblSmartImageID =  @tblSmartImageID and dbo.vw_PermissionForAnnotations.SmartUser = N'" + Session["user"].ToString() + "' and IsFixed = 0";
               if (ddlListReaders.SelectedIndex > 0)
               {
                  strSQL = strSQL + " and vw_Annotations.smartuser = '" + ddlListReaders.SelectedValue.ToString() + "'";
               }
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.Parameters.Add("@tblSmartImageID", SqlDbType.Int);
                  cmd.Parameters["@tblSmartImageID"].Value = strSmartImageID;
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     intRecNo++;
                     string strColor = intRecNo % 2 == 0 ? "#F15D2A" : "#2b95a0"; // Sets if the color will be orange or green (ICES colors)
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br /><br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("AnnotationID : " + rdr["tblAnnotationID"].ToString(), 10, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("SmartUser : " + rdr["SmartUser"].ToString(), 10, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("Age : " + rdr["noDots"].ToString() + "(" + rdr["IsApproved"].ToString() + ")", 10, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     pnlAnnotationDetails.Controls.Add(getLabel("AQ: " + rdr["AQCode"].ToString(), 10, strColor));
                     pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                     if (rdr["Comment"] != null)
                     {
                        if (!string.IsNullOrEmpty(rdr["Comment"].ToString()))
                        {
                           pnlAnnotationDetails.Controls.Add(getLabel("Comment : " + rdr["Comment"].ToString(), 10, strColor));
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
   }
}