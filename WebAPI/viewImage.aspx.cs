using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebInterface
{
    public partial class viewImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string strtblEventID = Request.QueryString["tblEventID"];
            string strSmartImageID = Request.QueryString["SmartImageID"];
            if (string.IsNullOrEmpty(strtblEventID))
            {
                Response.Redirect("index.aspx?message=Event wasnot specified");
            }
            if (string.IsNullOrEmpty(strSmartImageID))
            {
                Response.Redirect("index.aspx?message=The image was not specified");
            }
            string strToken = Request.QueryString["token"];
            bool isEventPublic = getIfPublicEvent(strtblEventID);

            if (isEventPublic)
            {
                fillTheEventDetails(strSmartImageID);
                fillTheAnnotationDetails(strSmartImageID);
            }
            else
            {
                if(!string.IsNullOrEmpty(strToken))
                {
                    string strTokenEvent = getEventToken(strtblEventID);
                    if ( strToken.ToUpper().Equals(strTokenEvent.ToUpper()))
                    {
                        fillTheEventDetails(strSmartImageID);
                        fillTheAnnotationDetails(strSmartImageID);
                    }
                }
            }


        }

        protected string getEventToken(string strtblEventID)
        {
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string strSQL = " SELECT * FROM [dbo].tblEvent ";
                    strSQL = strSQL + " where tblEventID =  @tblEventID";
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                    {
                        cmd.Parameters.Add("@tblEventID", SqlDbType.Int);
                        cmd.Parameters["@tblEventID"].Value = strtblEventID;
                        cmd.CommandType = CommandType.Text;
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            return rdr["GUID_EventID"].ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                String a = exp.Message.ToString();
            }

            return "";
        }

        protected bool getIfPublicEvent(string strtblEventID)
        {
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string strSQL = " SELECT * FROM [dbo].tblEvent ";
                    strSQL = strSQL + " where tblEventID =  @tblEventID and Published = 1";
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                    {
                        cmd.Parameters.Add("@tblEventID", SqlDbType.Int);
                        cmd.Parameters["@tblEventID"].Value = strtblEventID;
                        cmd.CommandType = CommandType.Text;
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                String a = exp.Message.ToString();
            }

            return false;
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
                     if (rdr["sex"] != null)
                     {
                        lblSex.Text = rdr["sex"].ToString();
                     }
                     if (rdr["FishLength"] != null)
                     {
                        lblLenght.Text = rdr["FishLength"].ToString();
                     }
                     if (rdr["Area"] != null)
                     {
                        lblArea.Text = rdr["Area"].ToString();
                     }

                     if (rdr["FishWeight"] != null)
                     {
                        lblWeight.Text = rdr["FishWeight"].ToString();
                     }

                     if (rdr["FishID"] != null)
                     {
                        lblFishID.Text = rdr["FishID"].ToString();
                     }

                     if (rdr["CatchDate"] != null)
                     {
                        lblDate.Text = rdr["CatchDate"].ToString();
                     }


                     int newWidth = 640;
                     lblEventName.Text = rdr["SampleID"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                     //string strFileName = rdr["URL"].ToString();
                     string fileName = "~/SampleImages/" + rdr["intYear"].ToString() + "/" + rdr["tblEventID"].ToString() + "/" + rdr["filename"].ToString();
                     Bitmap img = new Bitmap(Server.MapPath(fileName));
                     string url = "http://smartdots.ices.dk/sampleimages/" + rdr["intYear"].ToString() + "/" + rdr["tblEventID"].ToString() + "/pub/" + rdr["GUID_PublishedSmartImage"].ToString() + ".jpg";
                     int intWith = img.Width;
                     int intHeight = img.Height;

                     // We will try to render a canvas here to be able to draw a line and the dots
                     /* old code
                     pnlImages.Controls.Add(new LiteralControl("<canvas id='othImage" + strSmartImageID + "'  width='600' height='450' style='background-repeat:no-repeat;background-size: 100% 100%;background-image:url(" + rdr["URL"].ToString() + ");border:0px;'></canvas>"));
                     string strDrawLinesAndDots = getJavaScriptToDrawLinesAndDots(strSmartImageID, intWith, intHeight, newWidth);
                     ScriptManager.RegisterStartupScript(this, this.GetType(), "showLines", strDrawLinesAndDots, true);
                     */
                     Label lblText = new Label();
                     //                            lblText.Text = "SampleID = " + strSampleID;
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     pnlImages.Controls.Add(lblText);
                     pnlImages.Controls.Add(new LiteralControl("<br />"));
                     // We will try to render a canvas here to be able to draw a line and the dots
                     pnlImages.Controls.Add(new LiteralControl("<canvas id='othImage" + strSmartImageID + "' width='640' height='480' style='background-repeat:no-repeat;background-image:url(\"" + url + "\");background-size: 100% 100%;border:0px;'> Your browser does not support the canvas element.</canvas>"));
                     string strDrawLinesAndDots = getJavaScriptToDrawLinesAndDots(strSmartImageID, intWith, intHeight, newWidth, 480);
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
               string strSQL = " select * from [dbo].[vw_Dots] INNER JOIN dbo.tblEventParticipants ON [vw_Dots].tblEventID = dbo.tblEventParticipants.tblEventID AND [vw_Dots].SmartUser = dbo.tblEventParticipants.SmartUser ";
               strSQL = strSQL + " where Number is not null and tblSmartImageID =  @tblSmartImageID";
               if (ddlListReaders.SelectedIndex > 0)
               {
                  strSQL = strSQL + " and [vw_Dots].AnonimisedName = '" + ddlListReaders.SelectedValue.ToString() + "'";
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
               string strSQL = " select * from [dbo].[vw_Lines]INNER JOIN dbo.tblEventParticipants ON [vw_Lines].tblEventID = dbo.tblEventParticipants.tblEventID AND [vw_Lines].SmartUser = dbo.tblEventParticipants.SmartUser ";
               strSQL = strSQL + " where tblSmartImageID =  @tblSmartImageID";
               if (ddlListReaders.SelectedIndex > 0)
               {
                  strSQL = strSQL + " and [vw_Lines].AnonimisedName = '" + ddlListReaders.SelectedValue.ToString() + "'";
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
                    string strSQL = " SELECT        dbo.vw_Annotations.tblSmartImageID, dbo.vw_Annotations.GUID_SmartImage, dbo.vw_Annotations.GUID_AnnotationID, dbo.vw_Annotations.tblAnnotationID, dbo.vw_Annotations.tblEventID, dbo.vw_Annotations.tblSampleID, dbo.vw_Annotations.TypeAnotation, dbo.vw_Annotations.SmartUser, dbo.vw_Annotations.IsApproved, dbo.vw_Annotations.IsFixed, dbo.vw_Annotations.IsReadOnly, dbo.vw_Annotations.SmartDotsCreateDate,  dbo.vw_Annotations.CreateDate, dbo.vw_Annotations.Closed, dbo.vw_Annotations.Published, dbo.vw_Annotations.AQ_Code, dbo.vw_Annotations.Comment, isnull(dbo.vw_Annotations.noDots,0) as noDots , dbo.tblEventParticipants.ExpertiseLevel,  dbo.tblEventParticipants.AnonimisedName";
                    strSQL = strSQL + " FROM            dbo.vw_Annotations INNER JOIN dbo.tblEventParticipants ON dbo.vw_Annotations.tblEventID = dbo.tblEventParticipants.tblEventID AND dbo.vw_Annotations.SmartUser = dbo.tblEventParticipants.SmartUser ";
                    strSQL = strSQL + " Where vw_Annotations.tblSmartImageID =  @tblSmartImageID and IsFixed = 0 and dbo.tblEventParticipants.Number is not null";
                    if (ddlListReaders.SelectedIndex > 0)
                    {
                        strSQL = strSQL + " and vw_Annotations.AnonimisedName = '" + ddlListReaders.SelectedValue.ToString() +  "'";
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
                            pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                            pnlAnnotationDetails.Controls.Add(getLabel("AnnotationID : " + rdr["tblAnnotationID"].ToString(), 10, strColor));
                            pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                            pnlAnnotationDetails.Controls.Add(getLabel("SmartUser : " + rdr["AnonimisedName"].ToString(), 10, strColor));
                            pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));
                            pnlAnnotationDetails.Controls.Add(getLabel("Age : " + rdr["noDots"].ToString(), 10, strColor));
                            pnlAnnotationDetails.Controls.Add(new LiteralControl("<br />"));


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