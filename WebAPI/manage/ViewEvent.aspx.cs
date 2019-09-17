using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebInterface.App_Code;

namespace WebInterface.manage
{
   public partial class ViewEvent : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {

         Webinterface.manage.index1 i = new Webinterface.manage.index1();
         i.validateSession();

         if ("notvalidated".Equals(Session["user"].ToString()))
         {
            Session["urlCheck"] = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.Redirect("index.aspx");
         }
         if (!string.IsNullOrEmpty(Request.QueryString["user"]))
         {
            Session["user"] = Request.QueryString["user"].ToString();
         }

         /////////////////////////////////////////////////////////////////////////////////////
         /////////////////////////////////////////////////////////////////////////////////////

         string strEventID = Request.QueryString["tblEventID"];

         if (!IsPostBack)
         {
            string strMessage = Request.QueryString["message"];
            if (!string.IsNullOrEmpty(strMessage))
            {
               lblMessage.Visible = true;
               lblMessage.Text = strMessage;
            }
            if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
            {
               hplnkEditEvent.Visible = true;
               hplnkEditEvent.NavigateUrl = "EditEvent.aspx?tblEventID=" + strEventID;
               hplnkDownloadData.Visible = true;
               hplnkViewSummary.Visible = true;
               hplnkAnonimisedNameForSmartUsers.Visible = true;
               hplnkBttDownloadReport.Visible = true;               
               hplnkOpenAndCloseEvent.Visible = true;
               hplnkViewReportLog.Visible = true;
               hplnkViewSummary.NavigateUrl = "SummaryAnnotationsPerAgeReader.aspx?tblEventID=" + strEventID;
               hplnkAnonimisedNameForSmartUsers.NavigateUrl = "AnonimisedNameForSmartUsers.aspx?tblEventID=" + strEventID;
               hplnkDownloadData.NavigateUrl = "~/download/DownloadEvent.ashx?tblEventID=" + strEventID;

               //hplnkDownloadReport.NavigateUrl = "~/download/getReport.ashx?tblEventID=" + strEventID;
               hplnkViewReportLog.NavigateUrl = "http://taf.ices.local/TAFtest/api/smartDots/log/" + strEventID;
               if (RunDBOperations.eventIsClosed(strEventID))
               {
                  hplnkOpenAndCloseEvent.Text = "Open Event";
               }
               else
               {
                  hplnkOpenAndCloseEvent.Text = "Close Event";
               }

            }
            else
            {
               hplnkDownloadData.Visible = false;
               hplnkViewSummary.Visible = false;
               hplnkAnonimisedNameForSmartUsers.Visible = false;
               hplnkOpenAndCloseEvent.Visible = false;

            }

            if (!string.IsNullOrEmpty(strEventID))
            {
               try
               {
                  String ConnectionString = ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
                  using (SqlConnection cn = new SqlConnection(ConnectionString))
                  {
                     SqlDataAdapter da = new SqlDataAdapter();
                     SqlCommand cmd = new SqlCommand();
                     DataSet ds = new DataSet();
                     cn.Open();
                     cmd.Connection = cn;
                     cmd.CommandText = "SELECT GUID_EventID, Purpose, vw_ListEvents.tblEventID, NameOfEvent, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, EndDate, Protocol, OrganizerEmail, Institute, CreateDate, ModifiedDate, SmartUser, EventType, TypeOfStructure, intYear, Closed, Published, No.NoSamples, NoFish FROM  dbo.vw_ListEvents INNER JOIN (SELECT COUNT(DISTINCT SampleID) AS NoSamples, COUNT(DISTINCT FishID) as NoFish , tblEventID FROM dbo.tblSamples group by tblEventID) AS No ON dbo.vw_ListEvents.tblEventID = No.tblEventID WHERE  dbo.vw_ListEvents.tblEventID = " + strEventID;
                     cmd.CommandType = CommandType.Text;
                     cmd.ExecuteNonQuery();
                     da.SelectCommand = cmd;
                     da.Fill(ds);
                     ///////////////////////////////////////////////////////////////////
                     foreach (DataRow dr in ds.Tables[0].Rows)
                     {
                        if (dr["Purpose"].ToString().Contains("Maturity"))
                        {
                           Response.Redirect("ViewMaturityEvent.aspx?tblEventID=" + strEventID);
                        }
                        lblNameOfEvent.Text = dr["NameOfEvent"].ToString();
                        lblSpecies.Text = dr["Species"].ToString();
                        lblEventID.Text = dr["tblEventID"].ToString();
                        lblStartDate.Text = dr["StartDate"].ToString();
                        lblEndDate.Text = dr["EndDate"].ToString();
                        lblEmailOrganizer.Text = dr["OrganizerEmail"].ToString();
                        lblPurpose.Text = dr["Purpose"].ToString();
                        lblEventType.Text = dr["EventType"].ToString();
                        lblClosed.Text = dr["Closed"].ToString();
                        lblNumberSamples.Text = dr["NoSamples"].ToString();
                        lblNumberFish.Text = dr["NoFish"].ToString();
                     }
                  }
               }
               catch (Exception exp)
               {
                  String a = exp.Message.ToString();
               }
            }
         }
         if (!string.IsNullOrEmpty(strEventID))
         {
            putEventImages(strEventID);
         }

      }


      protected void putEventImages(string strEventID)
      {
         HtmlTableRow rowImages = new HtmlTableRow(); // This will show the images in a small image frame
         HtmlTableRow rowSampleNumber = new HtmlTableRow(); // Will be the tible and will show the sample number of the Image 
         HtmlTableRow rowAnnotationDetails = new HtmlTableRow(); // This will show how many annotations each image has

         HtmlTableCell cellImage = new HtmlTableCell();
         HtmlTableCell cellSampleName = new HtmlTableCell();
         HtmlTableCell cellAnnotations = new HtmlTableCell();

         int numRecord = 3; // This is used to control how many columns each line will have;

         try
         {
            String ConnectionString = ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               DataSet ds = new DataSet();
               cn.Open();
               cmd.Connection = cn;
               string strSQL = "";

               strSQL = "SELECT dbo.vw_SmartImages.tblEventID, FishID, dbo.vw_SmartImages.tblSmartImageID, dbo.vw_SmartImages.URL, dbo.vw_SmartImages.tblSampleID, dbo.vw_SmartImages.SampleID, dbo.vw_SmartImages.CatchDate,  ";
               strSQL += "   dbo.vw_SmartImages.StatRec, dbo.vw_SmartImages.FishLength, dbo.vw_SmartImages.FishWeight, dbo.vw_SmartImages.Comments, dbo.vw_SmartImages.MaturityStage, dbo.vw_SmartImages.MaturityScale,";
               strSQL += "  dbo.vw_SmartImages.SampleType, dbo.vw_SmartImages.SampleOrigin, dbo.vw_SmartImages.Area, dbo.vw_SmartImages.StockCode, dbo.vw_SmartImages.Sex, dbo.vw_SmartImages.PreparationMethod, ";
               strSQL += "   dbo.vw_SmartImages.GUID_Sample, dbo.vw_SmartImages.GUID_EventID, dbo.vw_SmartImages.GUID_SmartImage, dbo.vw_SmartImages.FileName, ISNULL(NoAnnotationsWithPermission.NoAnnotations, 0) AS NoAnnotations,";
               strSQL += "   dbo.vw_SmartImages.Closed, dbo.vw_SmartImages.tblCodeID_Area, dbo.getMode(dbo.vw_SmartImages.tblSmartImageID) AS Mode, dbo.vw_SmartImages.NoAnnotations AS NoTotalAnnotations, ";
               strSQL += "   dbo.hasFixedLine(dbo.vw_SmartImages.tblSmartImageID) AS hasFixedLine, isnull(NoAprovedAnnotationsWithPermission.NoApprovedAnnotations, 0) as NoApprovedAnnotations ";
               strSQL += "  FROM dbo.vw_SmartImages LEFT OUTER JOIN ";
               strSQL += "  (SELECT tblSmartImageID, COUNT(tblAnnotationID) AS NoApprovedAnnotations";
               strSQL += "  FROM            (SELECT vw_PermissionForAnnotations_1.tblSmartImageID, vw_PermissionForAnnotations_1.tblAnnotationID";
               strSQL += "                        FROM            dbo.vw_PermissionForAnnotations AS vw_PermissionForAnnotations_1 INNER JOIN";
               strSQL += "                                                  dbo.tblAnnotations AS tblAnnotations_1 ON vw_PermissionForAnnotations_1.tblAnnotationID = tblAnnotations_1.tblAnnotationID";
               strSQL += "                        WHERE        (vw_PermissionForAnnotations_1.SmartUser = '" + Session["user"].ToString() + "') AND(tblAnnotations_1.IsFixed = 0) AND(tblAnnotations_1.IsApproved = 1)";
               strSQL += "                                   GROUP BY vw_PermissionForAnnotations_1.tblSmartImageID, vw_PermissionForAnnotations_1.tblAnnotationID) AS t_1";
               strSQL += "         GROUP BY tblSmartImageID) AS NoAprovedAnnotationsWithPermission ON dbo.vw_SmartImages.tblSmartImageID = NoAprovedAnnotationsWithPermission.tblSmartImageID LEFT OUTER JOIN";
               strSQL += "       (SELECT        tblSmartImageID, COUNT(tblAnnotationID) AS NoAnnotations";
               strSQL += "         FROM(SELECT        dbo.vw_PermissionForAnnotations.tblSmartImageID, dbo.vw_PermissionForAnnotations.tblAnnotationID";
               strSQL += "                                   FROM            dbo.vw_PermissionForAnnotations INNER JOIN";
               strSQL += "                                                             dbo.tblAnnotations ON dbo.vw_PermissionForAnnotations.tblAnnotationID = dbo.tblAnnotations.tblAnnotationID";
               strSQL += "                                   WHERE(dbo.vw_PermissionForAnnotations.SmartUser = '" + Session["user"].ToString() + "') AND(dbo.tblAnnotations.IsFixed = 0)";
               strSQL += "                                   GROUP BY dbo.vw_PermissionForAnnotations.tblSmartImageID, dbo.vw_PermissionForAnnotations.tblAnnotationID) AS t";
               strSQL += "         GROUP BY tblSmartImageID) AS NoAnnotationsWithPermission ON dbo.vw_SmartImages.tblSmartImageID = NoAnnotationsWithPermission.tblSmartImageID";
               strSQL += "  WHERE(dbo.vw_SmartImages.tblEventID = " + strEventID + ")";
               
               if (ddlListArea.SelectedIndex > 0)
               {
                  strSQL += " and tblcodeid_area = " + ddlListArea.SelectedValue;
               }
               if (ddlListQuarter.SelectedIndex > 0)
               {
                  strSQL += " and DATEPART(QUARTER, DCatchDate) = " + ddlListQuarter.SelectedValue;
               }
               if (ddlMode.SelectedIndex > 0)
               {
                  strSQL += " and dbo.getMode(dbo.vw_SmartImages.tblSmartImageID) = " + ddlMode.SelectedValue;
               }
               strSQL += "  ORDER BY dbo.vw_SmartImages.DCatchDate";
               /*
                              if (ddlListReaders.SelectedIndex > 0)
                              {
                                  strSQL += " and vw_SmartImages.tblSmartImageID in (select tblSmartImageID from tblAnnotations where SmartUser = '" + ddlListReaders.SelectedValue.ToString() + "'  and IsFixed = 0 and tblEventID = " + strEventID + ") " ;
                              }
                              if (ddlListExperties.SelectedIndex > 0)
                              {
                                  strSQL += " and vw_SmartImages.tblSmartImageID in ( SELECT        dbo.tblAnnotations.tblSmartImageID FROM dbo.tblAnnotations INNER JOIN dbo.tblEventParticipants ON dbo.tblAnnotations.tblEventID = dbo.tblEventParticipants.tblEventID AND dbo.tblAnnotations.SmartUser = dbo.tblEventParticipants.SmartUser WHERE (dbo.tblAnnotations.IsFixed = 0) AND (dbo.tblAnnotations.tblEventID = " + strEventID + ") AND (dbo.tblEventParticipants.ExpertiseLevel = " + ddlListExperties.SelectedValue.ToString() + ") ) ";
                              }
                              */
               cmd.CommandText = strSQL;
               cmd.CommandType = CommandType.Text;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);
               ///////////////////////////////////////////////////////////////////
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  numRecord++;
                  if (numRecord % 4 == 0)
                  {
                     rowImages = new HtmlTableRow();
                     rowSampleNumber = new HtmlTableRow();
                     rowAnnotationDetails = new HtmlTableRow();
                  }
                  cellImage = new HtmlTableCell();
                  cellSampleName = new HtmlTableCell();
                  cellAnnotations = new HtmlTableCell();

                  //Builds the image to add to the cell of image
                  HyperLink imageSample = new HyperLink();
                  imageSample.ID = "img" + numRecord;
                  imageSample.ImageUrl = dr["URL"].ToString();
                  imageSample.NavigateUrl = "viewDetailsImage.aspx?tblEventID=" + strEventID + "&SmartImageID=" + dr["tblSmartImageID"].ToString();
                  imageSample.ImageWidth = 200;
                  cellImage.Controls.Add(imageSample);
                  rowImages.Cells.Add(cellImage);

                  // Puts the sampleID as title of the Image
                  cellSampleName.InnerHtml = dr["SampleID"].ToString();
                  rowSampleNumber.Cells.Add(cellSampleName);

                  // Puts the number of annotations 
                  if (dr["fishID"] != null)
                  {
                     cellAnnotations.InnerHtml = "<br><span style='font-weight:bold;font-size:12px'>FishID:</span> " + dr["FishID"].ToString();
                  }
                  if (dr["Sex"] != null)
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Sex:</span> " + dr["Sex"].ToString();
                  }
                  if (dr["FishWeight"] != null)
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Fish Weight:</span> " + dr["FishWeight"].ToString();
                  }
                  if (dr["FishLength"] != null)
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Fish Length:</span> " + dr["FishLength"].ToString();
                  }
                  if (dr["Area"] != null)
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Area:</span> " + dr["Area"].ToString();
                  }
                  if (dr["hasFixedLine"].ToString() != "Yes")
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px;COLOR:#FF0000'>Image has a fixe line:</span> " + dr["hasFixedLine"].ToString();
                  }
                  /*
                  if (ddlListReaders.SelectedIndex > 0)
                  {
                      cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>No of age reading(s) from " + ddlListReaders.SelectedValue.ToString() + ":</span> " + dr["NoAnnotationsFromUser"].ToString();
                      cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Age reading from " + ddlListReaders.SelectedValue.ToString() + ":</span> " + dr["Age"].ToString();
                  }*/
                  cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>No. of approved age reading:</span> " + dr["NoApprovedAnnotations"].ToString() + " out of " + dr["NoAnnotations"].ToString();

                  if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString(), strEventID))
                  {
                     //                            cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Modal age:</span> " + dr["Mode"].ToString();
                  }
                  else
                  {
                     //                            cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Modal age:</span> <span style='font-size:12px'>*Only the event managers can see this field</span>";
                  }
                  rowAnnotationDetails.Cells.Add(cellAnnotations);

                  if (numRecord % 4 == 3 || numRecord == (ds.Tables[0].Rows.Count + 2))
                  {
                     tableContent.Rows.Add(rowSampleNumber);
                     tableContent.Rows.Add(rowImages);
                     tableContent.Rows.Add(rowAnnotationDetails);
                     tableContent.Rows.Add(getEmptyLine());
                  }
               }


            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

      }

      protected HtmlTableRow getEmptyLine()
      {
         // This one is not dynamic and it only shows a new line
         HtmlTableRow rowEmptyLine = new HtmlTableRow(); // Just to split the rows. 
         HtmlTableCell cellEmpty = new HtmlTableCell();
         cellEmpty.InnerHtml = "&nbsp;";
         cellEmpty.ColSpan = 4;
         rowEmptyLine.Cells.Add(cellEmpty);

         return rowEmptyLine;
      }

      protected void hplnkOpenEvent_Click(object sender, EventArgs e)
      {
         if (hplnkOpenAndCloseEvent.Text.Contains("Open"))
         {
            string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User re-open the event','" + Session["user"].ToString() + "'); UPDATE [dbo].[tblEvent] SET [Closed] = 0 WHERE tblEventID = " + Request.QueryString["tblEventID"];
            RunDBOperations.runDBSmartDotsSQL(strSQL);
            hplnkOpenAndCloseEvent.Text = "Close Event";
            lblClosed.Text = "No";
         }
         else
         {
            string strSQL = "INSERT INTO [dbo].[tblOperations] ([Description],[SmartUser])VALUES('User closed the event','" + Session["user"].ToString() + "'); UPDATE [dbo].[tblEvent] SET [Closed] = 1 WHERE tblEventID = " + Request.QueryString["tblEventID"];
            RunDBOperations.runDBSmartDotsSQL(strSQL);
            hplnkOpenAndCloseEvent.Text = "Open Event";
            lblClosed.Text = "True";
         }

      }

      protected void hplnkBttDownloadReport_Click(object sender, EventArgs e)
      {
         // string path = "http://taf.ices.local/worker2/smartDotsReport/addJob/77?email=carlos@ices.dk";
         /* HttpClient client = new HttpClient();
 //         HttpResponseMessage response = await client.GetAsync(path);
          if (response.IsSuccessStatusCode)
          {
             return;
          }
          return;*/

         string streventID = Request.QueryString["tblEventID"];
         string strEmail = Session["email"].ToString();

         string address = "http://taf.ices.local/worker2/smartDotsReport/addJob/"+ streventID + "?email=" + strEmail + "&nameOfEvent=" + lblNameOfEvent.Text + "&species=" + lblSpecies.Text;
         using (WebClient client = new WebClient())
         {
            client.DownloadString(address);
         }
         lblDownloadReportLabel.Text = "The report in be sent to your email in a moment.";
         return; 
      }
   }
}