using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace Webinterface.manage
{
   public partial class EditEvent : System.Web.UI.Page
   {
      static string strMessageSuccess = "";
      static string strMessageError = "";
      private Uri _serviceUri = new Uri("http://datsu.ices.dk/DatsuRest/");


      protected void Page_Load(object sender, EventArgs e)
      {
         RunDBOperations.validateSession();
         Page.Form.Attributes.Add("enctype", "multipart/form-data");

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }

         /////////////////////////////////////////////////////////////////////////////////////
         /////////////////////////////////////////////////////////////////////////////////////

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


         /////////////// Check if the event is the event organizer to be able to delete the event  \\\\\\\\\\\\\\\\\\
         if (SmartUtilities.getEventField(strEventID, "SmartUser").Trim().ToUpper().Contains(Session["user"].ToString().Trim().ToUpper()))
         {
            if (!eventHasAnnotations(strEventID))
            {
               lnkDeleteEvent.Visible = true;
            }
         }
         // Checks if a event is closed to put the publish event visible
         if (RunDBOperations.eventIsClosed(strEventID))
         {
            lnkPublishEvent.Visible = true;
         }

         fillTheEventDetails(strEventID);

         if (IsPostBack)
         {
            //gv_SamplesAndImages.DataSourceID = "SqlSamplesEvent";
            //gv_SamplesAndImages.DataBind();
         }

         checkifNoAgeReadersInEvent(); // Show a lable if there are no age readers in the event;            

      }



      public bool eventHasAnnotations(string strEventID)
      {
         if (RunDBOperations.intExecuteScalarQuery("SELECT COUNT(*)  FROM [dbo].[tblAnnotations] where tblEventID = " + strEventID) > 0)
         {
            return true;
         }
         return false;
      }


      protected void checkifNoAgeReadersInEvent()
      {
         DataView view = (DataView)sqlDSAgeReadersInTheExercise.Select(DataSourceSelectArguments.Empty);
         if (view.Count == 0)
         {
            lblNoAgeReadersIntheExercise.Visible = true;
         }
         else
         {
            lblNoAgeReadersIntheExercise.Visible = false;
         }

      }

      protected string screenFile()
      {

         if (UploadFile.HasFile)
         {
            try
            {
               Stopwatch sw = new Stopwatch();
               //                    this.lbMsg.Text = "";
               sw.Start();
               using (var client = new HttpClient())
               {
                  using (var content = new MultipartFormDataContent())
                  {
                     string request = "{FileName:'" + UploadFile.FileName + "', EmailAddress:'" + Session["email"].ToString() + "', DataType:'SMARTDOTS'}";
                     content.Add(new StringContent(request), "Request");
                     content.Add(new StreamContent(UploadFile.FileContent), "File", UploadFile.FileName);
                     client.BaseAddress = _serviceUri;
                     client.Timeout = TimeSpan.FromMilliseconds(24000000);
                     client.DefaultRequestHeaders.Accept.Clear();
                     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                     var clientresponse = client.PostAsync("api/ScreenFile", content).Result;
                     if (clientresponse.IsSuccessStatusCode)
                     {
                        screenResults response = clientresponse.Content.ReadAsAsync<screenResults>().Result;
                        if (!string.IsNullOrEmpty(response.SessionID))
                        {
                           sw.Stop();
                           if (response.NumberOfErrors < 1)
                           {
                              return response.SessionID;
                           }
                           lblMessageSamples.Text = string.Format("File screened successfully.  SessionID = {0} File Size = {1}KBs Time taken = {2} secs.", response.SessionID, (double)(UploadFile.FileContent.Length / 1000), (double)(sw.ElapsedMilliseconds / 1000));
                           hlnkResult.NavigateUrl = "viewScreenResult.aspx?sessionid=" + response.SessionID + "&groupError=0";
                        }
                        else
                           lblMessageSamples.Text = string.Format("Failed to screen file. Error message : " + response.ScreenResultURL);
                     }
                     else
                        lblMessageSamples.Text = string.Format("Failed to call service. error message: {0}", clientresponse.StatusCode);
                  }
               }
            }
            catch (Exception ex)
            {
               SmartUtilities.saveToLog(ex);
               lblMessageSamples.Text = string.Format("Failed to uploaded file. error message: {0}", ex.ToString());
            }
         }
         else
         {
            lblMessageSamples.Text = string.Format("Please select a file to upload stations");
         }
         return null;
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
                  string strSQL = " SELECT     Purpose,   dbo.tblEvent.tblEventID,Closed, dbo.tblEvent.NameOfEvent, dbo.tblEvent.Species, dbo.tblEvent.tblCodeID_TypeOfStucture, dbo.tblEvent.tblCodeID_TypeOfExercice, includeAQ3AnnotationsInReport, CONVERT(nvarchar(30), dbo.tblEvent.StartDate, 111) as StartDate, CONVERT(nvarchar(30), dbo.tblEvent.EndDate, 111) as EndDate, dbo.tblEvent.Protocol, dbo.tblEvent.OrganizerEmail, dbo.tblEvent.Institute, dbo.tblEvent.CreateDate, dbo.tblEvent.ModifiedDate, dbo.tblEvent.SmartUser, TypeEvent.Description  as EventType FROM  dbo.tblEvent INNER JOIN dbo.tblCode AS TypeEvent ON dbo.tblEvent.tblCodeID_TypeOfExercice = TypeEvent.tblCodeID ";
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
                        if (rdr["Purpose"].ToString().Contains("Maturity"))
                        {
                           Response.Redirect("EditMaturityEvent.aspx?tblEventID=" + strEventID);
                        }

                        lblPurpose.Text = "Purpose: " + rdr["Purpose"].ToString();
                        txtEventName.Text = rdr["NameOfEvent"].ToString();
                        txtStartDate.Text = rdr["StartDate"].ToString();
                        hpnkViewEvent.NavigateUrl = "~/manage/ViewEvent.aspx?tblEventID=" + strEventID;
                        txtEndDate.Text = rdr["EndDate"].ToString();
                        lblSpecies.Text = rdr["Species"].ToString();
                        lblTypeOfEvent.Text = rdr["EventType"].ToString();
                        lblEventName.Text = rdr["NameOfEvent"].ToString() + " (EventID:" + rdr["tblEventID"].ToString() + ")";
                        if (rdr["Closed"].ToString() == "True")
                        {
                           lnkCloseEvent.Text = "The event is closed";
                        }
                        if (rdr["includeAQ3AnnotationsInReport"].ToString() == "True")
                        {
                           chkIncludeAQ3Annotations.Checked = true;
                        }
                        else
                        {
                           lnkCloseEvent.Visible = true;
                           lnkCloseEvent.Text = "Close event";
                        }

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


      protected void lnkEditEventDetails_Click(object sender, EventArgs e)
      {
         if (lnkEditEventDetails.Text == "Cancel the operation of editing the event details")
         {
            pnlEditEventDetails.Visible = false;
            lnkEditEventDetails.Text = "Edit Event Details";
         }
         else
         {
            pnlEditEventDetails.Visible = true;
            lnkEditEventDetails.Text = "Cancel the operation of editing the event details";
         }

      }

      protected void lnkAddMoreSamplesToEvent_Click(object sender, EventArgs e)
      {
         if (lnkAddMoreSamplesToEvent.Text.Contains("Add"))
         {
            pnlUploadMoreSamples.Visible = true;
            lnkAddMoreSamplesToEvent.Text = "Cancel the operation of ading more samples to the events";
         }
         else
         {
            pnlUploadMoreSamples.Visible = false;
            lnkAddMoreSamplesToEvent.Text = "Add More samples to the events";
         }
      }

      protected void lnkViewListOfAgeReaders_Click(object sender, EventArgs e)
      {
         if (lnkViewListOfAgeReaders.Text == "View the list of  the age readers")
         {
            pnlListAgeReaders.Visible = true;
            lnkViewListOfAgeReaders.Text = "Hide the list of  the age readers";
         }
         else
         {
            pnlListAgeReaders.Visible = false;
            lnkViewListOfAgeReaders.Text = "View the list of  the age readers";
         }
      }
      protected bool checkIfUserIsAlreadyInTheEvent(string strEventID, string strUserEmail)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT * FROM            dbo.tblEventParticipants INNER JOIN dbo.tblDoYouHaveAccess ON dbo.tblEventParticipants.SmartUser = dbo.tblDoYouHaveAccess.SmartUser";
               strSQL = strSQL + " WHERE(dbo.tblEventParticipants.tblEventID = " + strEventID + ") AND (dbo.tblDoYouHaveAccess.Email = N'" + strUserEmail + "')";
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
                     // This means that there is already a user in the system for this event; 
                     return true;
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


      protected void gv_ListUsersExperties_RowCommand(object sender, GridViewCommandEventArgs e)
      {
         if (e.CommandName == "AddUser")
         {
            // Retrieve the row index stored in the 
            // CommandArgument property.
            int index = Convert.ToInt32(e.CommandArgument);

            // Retrieve the row that contains the button 
            // from the Rows collection.
            GridViewRow row = gv_ListUsersExperties.Rows[index];
            string strUserEmail = row.Cells[2].Text;
            string strEventID = Request.QueryString["tblEventID"].ToString();
            if (checkIfUserIsAlreadyInTheEvent(strEventID, strUserEmail))
            {
               lblAddUsersMessage.Visible = true;
               lblAddUsersMessage.Text = "The user: " + strUserEmail + " already is listed has participant in the event";
            }
            else
            {
               try
               {

                  String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                  using (SqlConnection cn = new SqlConnection(ConnectionString))
                  {
                     string StrSQL = "INSERT INTO [dbo].[tblEventParticipants]   ([tblEventID],[SmartUser],[Role],[ExpertiseLevel],[ProvidesDataForAssessment]) SELECT  top 1  " + strEventID + " AS EventID, SmartUser, 0 AS[Role], ExpertiseLevel as [ExpertiseLevel], ExpertiseLevel AS[ProvidesDataForAssessment] FROM dbo.tblAgeReadersSkills WHERE     (SmartUser IN(SELECT     SmartUser FROM          dbo.tblDoYouHaveAccess WHERE(Email = '" + strUserEmail + "'))) AND(Species IN(SELECT Species FROM          dbo.tblEvent WHERE(tblEventID = " + strEventID + "))) order by ExpertiseLevel desc ";
                     cn.Open();
                     using (SqlCommand cmd = new SqlCommand(StrSQL, cn))
                     {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        gv_ageReadersListedExercise.DataBind();
                        lblAddUsersMessage.Visible = true;
                        lblAddUsersMessage.Text = "The user:" + strUserEmail + " was addedd successfully.";
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
         checkifNoAgeReadersInEvent(); // Show a lable if there are no age readers in the event;
      }


      protected bool isUserWithAccess(string strUserEmail)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               string strSQL = " SELECT * FROM dbo.tblDoYouHaveAccess ";
               strSQL = strSQL + " WHERE  Email = N'" + strUserEmail + "' or SmartUser = '" + strUserEmail + "'";
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
                     // This means that there is already a user in the system for this event; 
                     return true;
                  }
               }
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
         }
         return false;

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
                  if (isUserWithAccess(user))
                  {
                     string strUserEmail = user;
                     string strEventID = Request.QueryString["tblEventID"].ToString();

                     if (checkIfUserIsAlreadyInTheEvent(strEventID, strUserEmail))
                     {
                        strMessage += "<br>The user: " + strUserEmail + " already is listed has participant in the event";
                     }
                     else
                     {
                        try
                        {
                           using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                           {
                              string strSQL = "INSERT INTO [dbo].[tblEventParticipants]   ([tblEventID],[SmartUser],[Role],[ExpertiseLevel],[ProvidesDataForAssessment]) ";
                              // This one will add the user if he already has a skill 
                              strSQL = strSQL + "select top 1  *  from ( ";
                              strSQL = strSQL + "SELECT  top 1  " + strEventID + " AS EventID, SmartUser, 0 AS[Role], ExpertiseLevel as [ExpertiseLevel], ExpertiseLevel AS[ProvidesDataForAssessment] FROM dbo.tblAgeReadersSkills WHERE     (SmartUser IN(SELECT     SmartUser FROM          dbo.tblDoYouHaveAccess WHERE(Email = '" + strUserEmail + "'))) AND(Species IN(SELECT Species FROM          dbo.tblEvent WHERE(tblEventID = " + strEventID + ")))  ";
                              strSQL = strSQL + " union all ";
                              strSQL = strSQL + "SELECT  top 1  " + strEventID + " AS EventID, SmartUser, 0 AS[Role], 0 as [ExpertiseLevel], 0 AS[ProvidesDataForAssessment] FROM dbo.[tblDoYouHaveAccess] WHERE     (SmartUser IN(SELECT     SmartUser FROM          dbo.tblDoYouHaveAccess WHERE(Email = '" + strUserEmail + "' or SmartUser = '" + strUserEmail + "'))) ";
                              strSQL = strSQL + ") as t";
                              // create and open a connection object
                              using (SqlCommand cmd = new SqlCommand(strSQL, conn))
                              {
                                 conn.Open();
                                 cmd.CommandType = CommandType.Text;
                                 cmd.ExecuteNonQuery();
                              }
                           }
                           strMessage += "<br>The user " + user + " was added successfully to the event.<br>";
                           txtUsers.Text = "";
                        }
                        catch (Exception exceptionConn)
                        {
                           SmartUtilities.saveToLog(exceptionConn);
                           strMessage = exceptionConn.Message.ToString();
                           // Response.Redirect("index.aspx?error=" + HttpContext.Current.Server.UrlEncode(exceptionConn.Message + " " + exceptionConn.ToString()));
                           return;
                        }
                     }
                  }
                  else
                  {
                     strMessage += "<br>The user " + user + " could not be added because it was not found.<br>";
                  }
               }
            }
         }
         else
         {
            strMessage += "<br>Please add some user emails before pushing the button.";
         }
         lblAddUsersMessage.Visible = true;
         lblAddUsersMessage.Text = strMessage;
         gv_ageReadersListedExercise.DataBind();

      }
      protected string getSampleNumberFromFileName(string filename)
      {
         int indexIfUnderscore = filename.IndexOf("_");
         if (indexIfUnderscore > 0)
         {
            return (filename.Substring(0, indexIfUnderscore));
         }
         int indexIfDot = filename.IndexOf(".");
         if (indexIfDot > 0)
         {
            return (filename.Substring(0, indexIfDot));
         }
         return filename;

      }
      protected string canFindThatSampleNumber(string strFileName, string strEventID)
      {

         // string strSQL = "SELECT  SampleID FROM dbo.vw_EventSamples  where sampleID = @SampleID and EventID = @EventID ";

         string strSQL = "SELECT tblSampleID FROM dbo.vw_EventSamples WHERE(tblEventID = @EventID) AND(@FileName LIKE SampleID + '%')";
         SqlDataReader rdr = null;
         SqlCommand cmd = new SqlCommand();
         try
         {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
            {
               conn.Open();
               cmd = new SqlCommand(strSQL, conn);
               cmd.Parameters.Add("@FileName", SqlDbType.NVarChar);
               cmd.Parameters["@FileName"].Value = strFileName;
               cmd.Parameters.Add("@eventID", SqlDbType.Int);
               cmd.Parameters["@eventID"].Value = strEventID;
               rdr = cmd.ExecuteReader();
               while (rdr.Read())
               {
                  return rdr["tblSampleID"].ToString();

               }
            }
         }
         catch (Exception e)
         {
            SmartUtilities.saveToLog(e);
         }
         return string.Empty;

      }



      protected void gv_SamplesAndImages_RowDataBound(object sender, GridViewRowEventArgs e)
      {
         if (e.Row.RowType == DataControlRowType.DataRow)
         {

            HyperLink imgButtonConnectedWithSamples = (HyperLink)e.Row.FindControl("imgButtonConnectedWithSamples");
            //                ImageButton imgButtonConnectedWithSamples = (ImageButton)e.Row.FindControl("imgButtonConnectedWithSamples");
            //                if (checkIfSampleHasImages(DataBinder.Eval(e.Row.DataItem, "tblEventID").ToString(), DataBinder.Eval(e.Row.DataItem, "tblSampleID").ToString()))
            if (int.Parse(DataBinder.Eval(e.Row.DataItem, "CountImages").ToString()) > 0)
            {
               imgButtonConnectedWithSamples.ImageUrl = "~/icons/connected.png";
               imgButtonConnectedWithSamples.NavigateUrl = "showSampleImages.aspx?EventID=" + DataBinder.Eval(e.Row.DataItem, "tblEventID").ToString() + "&sample=" + DataBinder.Eval(e.Row.DataItem, "tblSampleID").ToString();
               imgButtonConnectedWithSamples.Target = "_blank";
            }
         }
      }



      protected void bttEditEvent_Click(object sender, EventArgs e)
      {
         int intLengthEventName = txtEventName.Text.Length;
         if (intLengthEventName > 4)
         {
            string strSQL = String.Format("update tblEvent  set ModifiedDate = getdate(), StartDate = '{0}', EndDate = '{1}', NameOfEvent = '{2}' where tblEventID = {3}", txtStartDate.Text, txtEndDate.Text, txtEventName.Text, Request.QueryString["tblEventID"].ToString());
            RunDBOperations.runDBSmartDotsSQL(strSQL);
         }
         else
         {
            lblEditEventLabel.Text = "Event name is not valid";
         }

      }




      public static string Right(string sValue, int iMaxLength)
      {
         //Check if the value is valid
         if (string.IsNullOrEmpty(sValue))
         {
            //Set valid empty string as string could be null
            sValue = string.Empty;
         }
         else if (sValue.Length > iMaxLength)
         {
            //Make the string no longer than the max length
            sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
         }

         //Return the string
         return sValue;
      }

      protected void AddSamples_UploadComplete(object sender, AjaxControlToolkit.AjaxFileUploadEventArgs e)
      {
         string fileName = Path.GetFileName(e.FileName);
         //string strSampleNumberFromFile = getSampleNumberFromFileName(fileName);
         string strEventID = Request.QueryString["tblEventID"].ToString();
         string strExtention = EditEvent.Right(fileName, 3);
         string strSampleNumberFromFile = canFindThatSampleNumber(fileName, strEventID);
         if (!string.IsNullOrEmpty(strSampleNumberFromFile))
         {
            var folder = Server.MapPath("~/SampleImages/" + SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()) + "/" + Request.QueryString["tblEventID"].ToString());
            if (!Directory.Exists(folder))
            {
               Directory.CreateDirectory(folder);
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////// This will inset the image record and return the id so that the iamge is saved with the ID. \\\\\\\\\\\\\\\\\\\\\\\\\\\
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // this function will try to extract the extension of the image!!!


            try
            {

               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection _sqlConn = new SqlConnection(ConnectionString))
               {
                  SqlCommand command = new SqlCommand();
                  command.Connection = _sqlConn;
                  command.CommandText = "up_web_addImageToEvent";
                  command.CommandType = CommandType.StoredProcedure;
                  string strYear = SmartUtilities.getYearEvent(Request.QueryString["tblEventID"].ToString()).ToString();
                  command.Parameters.Add(new SqlParameter("@Year", strYear));
                  command.Parameters.Add(new SqlParameter("@tblEventID", strEventID));
                  command.Parameters.Add(new SqlParameter("@tblSampleID", strSampleNumberFromFile));
                  command.Parameters.Add(new SqlParameter("@originalfFilename", fileName));
                  command.Parameters.Add(new SqlParameter("@extension", strExtention));

                  SqlParameter GUID_SmartImage = new SqlParameter("@GUID_SmartImage", SqlDbType.NVarChar, 200);
                  GUID_SmartImage.Direction = ParameterDirection.Output;
                  command.Parameters.Add(GUID_SmartImage);

                  _sqlConn.Open();
                  command.ExecuteReader();
                  string strGUID_SmartImage = command.Parameters["@GUID_SmartImage"].Value.ToString();
                  if (!string.IsNullOrEmpty(strGUID_SmartImage))
                  {
                     AddSamples.SaveAs(Server.MapPath("~/SampleImages/" + strYear + "/" + strEventID + "/" + strGUID_SmartImage + "." + strExtention));
                     strMessageSuccess += "<br>A sample has matched the image: " + fileName;
                     //                     gv_SamplesAndImages.DataSourceID = "SqlSamplesEvent";
                     gv_SamplesAndImages.DataSourceID = "SQLSampleData";
                     gv_SamplesAndImages.DataBind();

                  }
                  else
                  {
                     lblMessage.Text = "Message:There was an error, it was not possible to create the check!";
                     lblMessage.ForeColor = Color.Red;
                     return;
                  }

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
         else
         {
            strMessageError += "<br>Could not find any sample that matched that image: " + fileName;
         }
      }


      protected void AddSamples_UploadCompleteAll(object sender, AjaxControlToolkit.AjaxFileUploadCompleteAllEventArgs e)
      {

         //  gv_SamplesAndImages.DataSourceID = "SqlSamplesEvent";
         //  gv_SamplesAndImages.DataBind();
         //Response.Redirect("EditEvent.aspx?EventID=" + Request.QueryString["eventID"].ToString());
         //ClientScript.RegisterStartupScript(typeof(Page), "autoPostback", ClientScript.GetPostBackEventReference(this, String.Empty), true);
         //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "DoPostBack", "__doPostBack(sender, e)", true);
      }

      protected void bttAddSamples_Click(object sender, EventArgs e)
      {
         string SessionID = "";

         string strEventID = Request.QueryString["tblEventID"].ToString();
         try
         {
            if (UploadFile.HasFile)
            {
               SessionID = screenFile();

               if (!string.IsNullOrEmpty(SessionID))
               {
                  // Import the data to the Database.

                  using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString))
                  {
                     using (SqlCommand cmd = new SqlCommand("up_ImportDATSU_session", conn))
                     {
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@smartUser", Session["user"].ToString()));
                        cmd.Parameters.Add(new SqlParameter("@eventID", strEventID));
                        cmd.Parameters.Add(new SqlParameter("@sessionID", SessionID));
                        cmd.ExecuteNonQuery();
                        lblMessageSamples.Text = "<br>The samples were uploaded correctly.";
                        lblMessageSamples.Visible = true;
                        // This is to check if the table will refresh                        
                        gv_SamplesAndImages.DataSourceID = "SQLSampleData";
                        gv_SamplesAndImages.DataBind();
                     }

                  }
               }
               else
               {
                  lblMessageSamples.Visible = true;
                  //                        lblMessageSamples.Text = "Sample file not valid, please check your sample file!<br>" + result.ErrorMessage.ToString().Replace("Country", "EDMO code (Institude code)");
               }
            }
            else
            {
               lblMessageSamples.Visible = true;
               lblMessageSamples.Text = "Please select a file.";
            }
         }
         catch (Exception ex)
         {
            SmartUtilities.saveToLog(ex);
            lblMessageSamples.Visible = true;
            lblMessageSamples.Text = "<br>There was an execption, please contact the administrator. <br>" + ex.ToString();
            SmartUtilities.saveToLog(ex);

         }

      }

      protected void gv_ageReadersListedExercise_RowDeleted(object sender, GridViewDeletedEventArgs e)
      {
         checkifNoAgeReadersInEvent(); // Show a lable if there are no age readers in the event;
      }

      protected void Button1_Click(object sender, EventArgs e)
      {
         lblAddSampleSuccess.Visible = true;
         lblAddSamplesError.Visible = true;
         lblAddSampleSuccess.Text = strMessageSuccess;
         lblAddSamplesError.Text = strMessageError;
         EditEvent.strMessageSuccess = "";
         EditEvent.strMessageError = "";
         gv_SamplesAndImages.DataSourceID = "SQLSampleData";
         gv_SamplesAndImages.DataBind();
      }

      protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
      {

      }

      public void updateRankingUsers(string strEventID)
      {
         string strUpdate = "UPDATE tblEventParticipants SET Number = TheID FROM(SELECT  (Row_Number()   OVER(ORDER BY ExpertiseLevel desc, [SmartUser])) * 2 AS TheId, tblEventID, SmartUser FROM tblEventParticipants where tblEventID = " + strEventID + ") as a INNER JOIN dbo.tblEventParticipants ON a.tblEventID = tblEventParticipants.tblEventID AND a.SmartUser = tblEventParticipants.SmartUser where tblEventParticipants.tblEventID = " + strEventID;
         RunDBOperations.runDBSmartDotsSQL(strUpdate);
      }

      protected void bttChangeScale_Click(object sender, EventArgs e)
      {
         string strEventID = Request.QueryString["tblEventID"];
         int intScale;
         bool isInt = int.TryParse(txtScale.Text, out intScale);
         if (isInt)
         {
            string strScale = txtScale.Text.ToString();
            string strUpdateSQL;
            if (string.IsNullOrEmpty(txtSamplesScaleContains.Text))
            {
               strUpdateSQL = string.Format("update [dbo].[tblSmartImage] set Scale = {0}  where tblEventID = {1}", strScale, strEventID);
            }
            else
            {
               strUpdateSQL = string.Format("update [dbo].[tblSmartImage] set Scale = {0}   FROM  dbo.tblSmartImage INNER JOIN dbo.tblSamples ON dbo.tblSmartImage.tblSampleID = dbo.tblSamples.tblSampleID  where tblEventID = {1} and sampleID like '%{2}%'", strScale, strEventID, txtSamplesScaleContains.Text);
            }
            RunDBOperations.runDBSmartDotsSQL(strUpdateSQL);
            lblMessageScale.Text = "The scale has changed to all the images of this event.";
            txtScale.Text = "";
            lblMessageScale.Visible = true;
         }
         else
         {
            lblMessageScale.Text = "Please specify a valid number for the scale";
            lblMessageScale.Visible = true;
         }

      }

      protected void buttonSuggestNo_Click(object sender, EventArgs e)
      {
         string strEventID = Request.QueryString["tblEventID"];
         if (!string.IsNullOrEmpty(strEventID))
         {
            updateRankingUsers(strEventID);
            Response.Redirect("EditEvent?tblEventID=" + strEventID);
         }
      }

      protected void lnkCloseEvent_Click(object sender, EventArgs e)
      {
         // this click need to close the event
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User closed the event','" + Session["user"].ToString() + "'); UPDATE [dbo].[tblEvent] SET [Closed] = 1 WHERE tblEventID = " + Request.QueryString["tblEventID"];
         RunDBOperations.runDBSmartDotsSQL(strSQL);
      }


      protected void bttAddDelegate_Click(object sender, EventArgs e)
      {
         string strEmailUser = SmartUtilities.isNameValidSmartDotsUser(txtDelegateName.Text.ToString());
         try
         {
            // Missing to check if the email is already in the event as a delegate
            if (strEmailUser != null)
            {
               if (!RunDBOperations.checkIfUserIsEventManager(strEmailUser, Request.QueryString["tblEventID"].ToString()))
               {
                  string strBodyMessage = "Dear " + strEmailUser + ",<br>";
                  strBodyMessage += "<br><br>You have been delegated as the event manager for SmartDots event ID " + Request.QueryString["tblEventID"].ToString() + " - " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "NameOfEvent") + " for the species " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "Species") + " which is scheduled to begin on " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "StartDate") + ". You can edit the event details and upload images and data to your event <a href='https://smartdots.ices.dk/manage/EditEvent?tblEventID=" + Request.QueryString["tblEventID"].ToString() + "'> here  </a>";
                  strBodyMessage += "<br><br>Best regards,<br><br>The SmartDots team";
                  SmartUtilities.sendEmail(strEmailUser, "SmartDots event delegate to the event: " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "NameOfEvent"), strBodyMessage);
                  // If the user is approved then need to sent the email from aknoledgement to the user
                  string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User added a new delegate','INSERT INTO[dbo].[tblEventData] ([tblEventID],[Value],[tblTypeID]) VALUES(" + Request.QueryString["tblEventID"] + "," + txtDelegateName.Text.ToString() + ",1)'";
                  RunDBOperations.runDBSmartDotsSQL(strSQL);
                  strSQL = "INSERT INTO [dbo].[tblEventData] ([tblEventID] ,[Value] ,[tblTypeID]) VALUES (" + Request.QueryString["tblEventID"] + ",'" + strEmailUser + "',1)";
                  RunDBOperations.runDBSmartDotsSQL(strSQL);
                  lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " was successfully added has event manager, an email has been sent to the user.";
               }
               else
               {
                  lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " is already an event manager!";
                  txtDelegateName.Text = "";
               }
            }
            else
            {
               lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " was not found, please try again!";
               txtDelegateName.Text = "";
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            lblAddDelagateText.Text = "An exception has happend, please contact smartdots@ices.dk";
         }
         gvEventDelegates.DataBind();
      }


      protected void gvEventDelegates_RowDataBound(object sender, GridViewRowEventArgs e)
      {
         ImageButton imgDeleteUser = (ImageButton)e.Row.FindControl("imgDeleteUserDelegation");
         if (imgDeleteUser != null)
         {
            if (DataBinder.Eval(e.Row.DataItem, "role").ToString().Contains("organizer"))
            {
               imgDeleteUser.Visible = false;
            }
         }

      }

      protected void gv_ListUsersExperties_SelectedIndexChanged(object sender, EventArgs e)
      {

      }

      protected void lnkPublishEvent_Click(object sender, EventArgs e)
      {
         // this click need to close the event
         // Need to setup this to allow the users to upload the report of the event;
         //string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User " + Session["user"].ToString() + " publish the event','" + Request.QueryString["tblEventID"] + "'); UPDATE [dbo].[tblEvent] SET [Published] = 1 WHERE tblEventID = " + Request.QueryString["tblEventID"];
         //RunDBOperations.runDBSmartDotsSQL(strSQL);
         Response.Redirect("PublishEvent.aspx?tblEventID=" + Request.QueryString["tblEventID"].ToString());

      }

      protected void lnkDeleteEvent_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User: " + Session["user"].ToString() + " deleted the event','" + Request.QueryString["tblEventID"] + "'); delete from [dbo].[tblEvent] WHERE tblEventID = " + Request.QueryString["tblEventID"];
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         Response.Redirect("ListOperations.aspx");
      }

      protected void bttDeleteImages_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User: " + Session["user"].ToString() + " delete some images','" + Request.QueryString["tblEventID"] + " and samplesLike'); EXEC [dbo].[up_WebInterface_deleteImages] @samplePart = N'" + txtSampleDeleteImages.Text.ToString() + "', @tblEventID = " + Request.QueryString["tblEventID"] + ", @smartuser = N'" + Session["user"].ToString() + "'";
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         gv_SamplesAndImages.DataBind();
      }

      protected void bttDeleteSamples_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User: " + Session["user"].ToString() + " delete some samples','" + Request.QueryString["tblEventID"] + " and samplesLike'); EXEC [dbo].[up_WebInterface_deleteSamples] @samplePart = N'" + txtSamplesDelete.Text.ToString() + "', @tblEventID = " + Request.QueryString["tblEventID"] + ", @smartuser = N'" + Session["user"].ToString() + "'";
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         gv_SamplesAndImages.DataBind();
      }

      protected void bttUpdateStrataWhereSample_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User: " + Session["user"].ToString() + " Updated Strata','" + Request.QueryString["tblEventID"] + " where samples like " + txtSampleContainsToUpdateStrata.Text.ToString() + "');";
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         if (txtSampleContainsToUpdateStrata.Text.Length < 1)
         {
            strSQL = string.Format("update tblSamples set strata = NULL where tblEventID = {1} and SampleID like '%{0}%'", txtSampleContainsToUpdateStrata.Text.ToString(), Request.QueryString["tblEventID"]);
         }
         else
         {
            strSQL = string.Format("update tblSamples set strata = '{2}' where tblEventID = {1} and SampleID like '%{0}%'", txtSampleContainsToUpdateStrata.Text.ToString(), Request.QueryString["tblEventID"], txtStrataValue.Text.ToString());
         }
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         gv_SamplesAndImages.DataBind();
      }

      protected void bttUpdateStrataToAnoterField_Click(object sender, EventArgs e)
      {
         string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User: " + Session["user"].ToString() + " Updated Strata','" + Request.QueryString["tblEventID"] + " where samples like " + txtSampleContainsToUpdateStrata.Text.ToString() + "');";
         RunDBOperations.runDBSmartDotsSQL(strSQL);
//         strSQL = string.Format("update tblSamples set strata = {0} where tblEventID = {1}",ddlStrataFields.SelectedValue.ToString(), Request.QueryString["tblEventID"]);
         strSQL = string.Format("update tblSamples set strata = tblCode.Code FROM dbo.tblSamples LEFT OUTER JOIN dbo.tblCode ON dbo.tblSamples.{0} = dbo.tblCode.tblCodeID WHERE tblEventID = {1}", ddlStrataFields.SelectedValue.ToString(), Request.QueryString["tblEventID"]);
         RunDBOperations.runDBSmartDotsSQL(strSQL);
         gv_SamplesAndImages.DataBind();
      }

      protected void lnkSendEmailParticipants_Click(object sender, EventArgs e)
      {
         
      }

      protected void sendAgeReaders_Email(string strEmailUser)
      {
         try
         {
            // Missing to check if the email is already in the event as a delegate
            if (strEmailUser != null)
            {
               if (!RunDBOperations.checkIfUserIsEventManager(strEmailUser, Request.QueryString["tblEventID"].ToString()))
               {
                  string strBodyMessage = "Dear " + strEmailUser + ",<br>";
                  strBodyMessage += "<br><br>You have been listed as a participant for SmartDots event ID " + Request.QueryString["tblEventID"].ToString() + " - " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "NameOfEvent") + " for the species " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "Species") + " which is scheduled to begin on " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "StartDate") + ". <br> <br> You can download smartdots <a href='https://github.com/ices-eg/SmartDots/blob/master/README.md#getting-started'> here  </a>";
                  strBodyMessage += "<br><br>Best regards,<br><br>The SmartDots team";
                  SmartUtilities.sendEmail(strEmailUser, ">You have been listed as a participant for SmartDots event: " + SmartUtilities.getEventField(Request.QueryString["tblEventID"], "NameOfEvent"), strBodyMessage);                  
                  lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " was successfully added has event manager, an email has been sent to the user.";
               }
               else
               {
                  lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " is already an event manager!";
                  txtDelegateName.Text = "";
               }
            }
            else
            {
               lblAddDelagateText.Text = "User " + txtDelegateName.Text.ToString() + " was not found, please try again!";
               txtDelegateName.Text = "";
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            lblAddDelagateText.Text = "An exception has happend, please contact smartdots@ices.dk";
         }
        
      }

      protected void bttSendEmailToParticipants_Click(object sender, EventArgs e)
      {
         lblAddUsersMessage.Visible = true;
         try
         {
            // If the user is approved then need to sent the email from aknoledgement to the user
            string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User sent ','INSERT INTO[dbo].[tblEventData] ([tblEventID],[Value],[tblTypeID]) VALUES(" + Request.QueryString["tblEventID"] + "," + txtDelegateName.Text.ToString() + ",1)'";
            RunDBOperations.runDBSmartDotsSQL(strSQL);

            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               DataSet ds = new DataSet();
               cn.Open();
               cmd.Connection = cn;
               cmd.CommandText = "SELECT S.Email FROM dbo.tblEventParticipants AS P INNER JOIN dbo.tblDoYouHaveAccess AS S ON P.SmartUser = S.SmartUser WHERE P.tblEventID = " + Request.QueryString["tblEventID"];
               cmd.CommandType = CommandType.Text;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);
               ///////////////////////////////////////////////////////////////////
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  sendAgeReaders_Email(dr["Email"].ToString());
               }
               lblAddUsersMessage.Text = "Email was sent to the age readers";
            }
         }
         catch (Exception exp)
         {
            SmartUtilities.saveToLog(exp);
            String a = exp.Message.ToString();
            lblAddUsersMessage.Text = "There was a problem sending the email, please contact smartdots@ices.dk";
         }
         
      }


      protected void hplnkBttDownloadReport_Click(object sender, EventArgs e)
      {       
         string streventID = Request.QueryString["tblEventID"];
         string strEmail = Session["email"].ToString();
         

         string address = "http://taf.ices.local/worker2/smartDotsReport/addJob/" + streventID + "?email=" + strEmail + "&nameOfEvent=" + lblEventName.Text + "&species=" + lblSpecies.Text;

         
         using (WebClient client = new WebClient())
         {
            client.DownloadString(address);
         }
         lblDownloadReportLabel.Text = "The report in be sent to your email in a moment.";
         return;
      }

      protected void chkIncludeAQ3Annotations_CheckedChanged(object sender, EventArgs e)
      {
         string strEventID = Request.QueryString["tblEventID"];
         if (chkIncludeAQ3Annotations.Checked)
         {
            RunDBOperations.executeScalarQuery("update tblEvent set includeAQ3AnnotationsInReport = 1 where tblEventID = " + strEventID);
         }
         else
         {
            RunDBOperations.executeScalarQuery("update tblEvent set includeAQ3AnnotationsInReport = 0 where tblEventID = " + strEventID);
         }
      }
   }

}
