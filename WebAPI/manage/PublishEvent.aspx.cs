using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;
using WebInterface.App_Code;

namespace WebInterface.manage
{
   public partial class PublishEvent : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {

         RunDBOperations.validateSession();
         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }
         if (!string.IsNullOrEmpty(Request.QueryString["message"]))
         {
            lblMessage.Text = Request.QueryString["message"].ToString();
         }


         /////////////////////// CHECKs IF THE EVENT WAS SPECIFIED \\\\\\\\\\\\\\\\\\
         string strEventID = Request.QueryString["tblEventID"];
         if (string.IsNullOrEmpty(strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=Event was not specified!");
         }

         /////////////////////// THIS WILL CHECK IF THE USER IS AN EVENT MANAGER \\\\\\\\\\\\\\\\\\
         /*
         if (!RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
         {
            Response.Redirect("ListOperations.aspx?message=user does not have permission to manage this event!");
         }
         */
         fillTheEventDetails(strEventID);

      }

      protected void fillTheEventDetails(string strEventID)
      {
         if (!IsPostBack)
         {
            try
            {
               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection cn = new SqlConnection(ConnectionString))
               {
                  string strSQL = " SELECT     Purpose,   dbo.tblEvent.tblEventID,Closed, dbo.tblEvent.NameOfEvent, dbo.tblEvent.Species, dbo.tblEvent.tblCodeID_TypeOfStucture, dbo.tblEvent.tblCodeID_TypeOfExercice,CONVERT(nvarchar(30), dbo.tblEvent.StartDate, 111) as StartDate, CONVERT(nvarchar(30), dbo.tblEvent.EndDate, 111) as EndDate, dbo.tblEvent.Protocol, dbo.tblEvent.OrganizerEmail, dbo.tblEvent.Institute, dbo.tblEvent.CreateDate, dbo.tblEvent.ModifiedDate, dbo.tblEvent.SmartUser, TypeEvent.Description  as EventType FROM  dbo.tblEvent INNER JOIN dbo.tblCode AS TypeEvent ON dbo.tblEvent.tblCodeID_TypeOfExercice = TypeEvent.tblCodeID ";
                  strSQL = strSQL + " where tblEventID = @tblEventID";
                  cn.Open();
                  using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                  {
                     cmd.Parameters.Add("@tblEventID", SqlDbType.Int);
                     cmd.Parameters["@tblEventID"].Value = strEventID;
                     cmd.CommandType = CommandType.Text;
                     SqlDataReader rdr = cmd.ExecuteReader();
                     while (rdr.Read())
                     {
                        lblEventType.Text = rdr["EventType"].ToString();
                        lblEventName.Text = rdr["NameOfEvent"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                        lblNameOfEvent.Text = rdr["NameOfEvent"].ToString();
                        lblEmailOrganizer.Text = rdr["OrganizerEmail"].ToString();
                        lblPurpose.Text = rdr["Purpose"].ToString();
                        lblStartDate.Text = rdr["StartDate"].ToString();
                        lblEndDate.Text = rdr["EndDate"].ToString();
                        lblSpecies.Text = rdr["Species"].ToString();
                        lblEventID.Text = rdr["EventType"].ToString();


                     }
                  }
               }
            }
            catch (Exception exp)
            {
               SmartUtilities.saveToLog(exp);
            }
         }
      }

      protected void bttPublishEvent_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('Generated Names for Published Images','" + Session["user"].ToString() + "'); update tblSmartImage set GUID_PublishedSmartImage = NEWID() WHERE GUID_PublishedSmartImage is null and tblEventID = " + Request.QueryString["tblEventID"];
         RunDBOperations.runDBSmartDotsSQL(strSQL);

            string strEventID = Request.QueryString["tblEventID"];
         if (!string.IsNullOrEmpty(strEventID))
         {
            if (this.flUpReport.HasFile)
            {
               if (!string.IsNullOrEmpty(flUpReport.FileName.ToString()))
               {
                  if (rdbPicturesFreelyAvailable.Checked)
                  {
                     copyImages(strEventID, false, false);
                  }
                  if (rdbPicturesLowResolution.Checked)
                  {
                     copyImages(strEventID, true, false);
                  }
                  if (rdbPicturesLowResolutionWaterMark.Checked)
                  {
                     copyImages(strEventID, true, true);
                  }
                  // Needs to copy the report file and update the link to the report file
                  string strYear = SmartUtilities.getYearEvent(strEventID).ToString();
                  this.flUpReport.SaveAs(Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/") + this.flUpReport.FileName.ToString());
                  this.flupSummaryReport.SaveAs(Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/") + this.flupSummaryReport.FileName.ToString());



                  // this click need to publish the event
                  strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User published the event','" + Session["user"].ToString() + "'); UPDATE [dbo].[tblEvent] SET [Published] = 1, Report = '" + this.flUpReport.FileName + "' , SummaryReport = '" + this.flupSummaryReport.FileName + "' WHERE tblEventID = " + Request.QueryString["tblEventID"];
                  RunDBOperations.runDBSmartDotsSQL(strSQL);

                  Response.Redirect("~/ViewEvent?key=" + strEventID);
               }
               else
               {
                  Response.Redirect("PublishEvent.aspx?tblEventID=" + strEventID + "&Message=Please, select a report before publishing the event.");
               }
            }
            else
            {
               Response.Redirect("PublishEvent.aspx?tblEventID=" + strEventID + "&Message=Please, select a report before publishing the event.");
            }
         }
         else
         {
            Response.Redirect("ListOperations.aspx?&Message=Event was not specified");
         }

      }
      protected void copyEvent(string strEventID, bool resizeImages, bool putWatermark)
      {
         //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         /////////////////////////////////////////// First needs to make a copy the database records \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
         //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         try
         {

            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection _sqlConn = new SqlConnection(ConnectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = _sqlConn;
               command.CommandText = "up_copyEventToEmptyTrainingEvent";
               command.CommandType = CommandType.StoredProcedure;
               string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();
               command.Parameters.Add(new SqlParameter("@originEventID", strEventID));
               command.Parameters.Add(new SqlParameter("@SmartUser", Session["user"].ToString()));
               SqlParameter newTblEventID = new SqlParameter("@newtblEventID", SqlDbType.NVarChar, 200);
               newTblEventID.Direction = ParameterDirection.Output;
               command.Parameters.Add(newTblEventID);
               _sqlConn.Open();
               command.ExecuteReader();

               // Then needs to copy the images
               string strNewtblEventID = command.Parameters["@newtblEventID"].Value.ToString();
               if (!string.IsNullOrEmpty(strNewtblEventID))
               {
                  copyImagesNewEvent(strEventID, strNewtblEventID, resizeImages, putWatermark);
               }               
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            lblMessage.Text = "Message: There was an error copying the event";
            lblMessage.ForeColor = Color.Red;
            return;
         }
         //DONE, should be finished now

      }

      protected bool copyImagesNewEvent(string strEventID, string strNewEventID, bool resizeImages, bool putWatermark)
      {
         string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();

         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = "SELECT GUID_PublishedSmartImage, FileName FROM dbo.tblSmartImage WHERE tblEventID =" + strNewEventID;
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  DataSet ds = new DataSet();
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     string imgFileName = Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/" + dr["FileName"].ToString());
                     var folder = Server.MapPath("~/SampleImages/" + strYear + "/" + strNewEventID);
                     var folderPub = Server.MapPath("~/SampleImages/" + strYear + "/" + strNewEventID + "/pub");
                     // This will make sure the folder exists
                     if (!Directory.Exists(folder))
                     {
                        Directory.CreateDirectory(folder);
                     }
                     // This will make sure the folder for publishing exists
                     if (!Directory.Exists(folderPub))
                     {
                        Directory.CreateDirectory(folderPub);
                     }
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["FileName"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folderPub + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["FileName"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folderPub + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["FileName"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folderPub + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);
                  }
               }
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            String a = exp.Message.ToString();
         }
         return false;

      }


      protected bool copyImages(string strEventID, bool resizeImages, bool putWatermark)
      {
         string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();
         System.Drawing.Image img;
         string imgFileName;

         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = "SELECT GUID_PublishedSmartImage, FileName FROM dbo.tblSmartImage WHERE tblEventID =" + strEventID;
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  DataSet ds = new DataSet();
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     imgFileName = Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/" + dr["FileName"].ToString());
                     var folder = Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/pub");
                     // This will make sure the folder for publishing exists
                     if (!Directory.Exists(folder))
                     {
                        Directory.CreateDirectory(folder);
                     }
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);
                     SmartUtilities.copyImageWithRestrictions(imgFileName, folder + "/" + dr["GUID_PublishedSmartImage"] + ".jpg", resizeImages, putWatermark);

                  }
               }
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            img = null;
         }

         #region This is the last step, case the make a public copy is checked then will copy the records in the database and the images

         // This is the last step, case the make a public copy is checked then will copy the records in the database and the images
         if (chkMakePublicCopy.Checked)
         {
            // This means that will copy the event to a public test event
            copyEvent(strEventID, resizeImages, putWatermark);
         }

         #endregion

         return true;

      }

      protected void fillInImageSize_Click(object sender, EventArgs e)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT * FROM dbo.tblSmartImage";
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  cmd.CommandType = CommandType.Text;
                  SqlDataReader rdr = cmd.ExecuteReader();
                  while (rdr.Read())
                  {
                     string URL = rdr["URL"].ToString();
                     string tblSmartImageID = rdr["tblSmartImageID"].ToString();
//                     SmartUtilities.updateSizeImage(tblSmartImageID, URL);

                  }
               }
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            String a = exp.Message.ToString();
         }
      }


      
      
   }
}