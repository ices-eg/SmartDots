using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebInterface.App_Code;


namespace WebInterface.manage
{
   public partial class ViewMaturityEvent : System.Web.UI.Page
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

         // This is a temporary fix to be able to test the events has otehr users 
         if (!string.IsNullOrEmpty(Request.QueryString["user"]))
         {
            Session["user"] = Request.QueryString["user"].ToString();
         }
         /////////////////////////////////////////////////////////////////////////////////////
         /////////////////////////////////////////////////////////////////////////////////////

         string strEventID = Request.QueryString["tblEventID"];
         /*
         if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString(), strEventID))
         {
             lblMode.Visible = true;
             ddlMode.Visible = true;
         }
         else
         {
             lblMode.Visible = false;
             ddlMode.Visible = false;

         }
         */


         if (!IsPostBack)
         {
            string strMessage = Request.QueryString["message"];
            if (!string.IsNullOrEmpty(strMessage))
            {
               lblMessage.Visible = true;
               lblMessage.Text = strMessage;
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
                        if (!dr["Purpose"].ToString().Contains("Maturity"))
                        {
                           Response.Redirect("ViewEvent.aspx?tblEventID=" + strEventID);
                        }
                        lblNameOfEvent.Text = dr["NameOfEvent"].ToString();
                        lblEventName.Text = dr["NameOfEvent"].ToString();
                        lblSpecies.Text = dr["Species"].ToString();
                        lblEventID.Text = dr["tblEventID"].ToString();
                        lblStartDate.Text = dr["StartDate"].ToString();
                        lblEndDate.Text = dr["EndDate"].ToString();
                        lblEmailOrganizer.Text = dr["OrganizerEmail"].ToString();
                        lblPurpose.Text = dr["Purpose"].ToString();
                        lblEventType.Text = dr["EventType"].ToString();
                        lblClosed.Text = dr["Closed"].ToString();
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

         if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
         {
            hplnkOpenAndCloseEvent.Visible = true;
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
            hplnkOpenAndCloseEvent.Visible = false;
         }
         if (!string.IsNullOrEmpty(strEventID))
         {
            putEventImages(strEventID);
         }

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
             
               strSQL = "SELECT        S.tblEventID, S.FishID, S.tblSmartImageID, S.URL, S.tblSampleID, S.SampleID, S.CatchDate, S.StatRec, S.FishLength, S.FishWeight, S.Comments, S.MaturityStage, S.MaturityScale, S.SampleType, S.SampleOrigin, S.Area,  S.StockCode, S.Sex, S.PreparationMethod, S.GUID_Sample, S.GUID_EventID, S.GUID_SmartImage, S.FileName, S.Closed, S.tblCodeID_Area, dbo.getMode(S.tblSmartImageID) AS Mode, S.NoAnnotations AS NoTotalAnnotations, isnull(NoAnnotations.HasAnnotation,'No') as HasAnnotation ";
               strSQL += "  FROM            dbo.vw_SmartImages AS S LEFT OUTER JOIN";
               strSQL += " (SELECT        SmartUser, tblEventID, FishID, 'Yes' AS HasAnnotation FROM dbo.tblAnnotationsMaturity where SmartUser = '" + Session["user"].ToString() + "' GROUP BY SmartUser, tblEventID, FishID) AS NoAnnotations ON S.tblEventID = NoAnnotations.tblEventID AND S.FishID = NoAnnotations.FishID";
               strSQL += "  WHERE        (S.VisibleDuringEvent = 1 or closed = 1) AND (S.tblEventID = " + strEventID + ") ";

               ////////////////////////////////////////////////////////////////////////////////////////////////////////////
               //////////////// IF THE USER IS THE EVENT MANAGER THEN THE SQL HAS TO CHANGE SO THAT IT CAN SEE HOW MANY ANNOTATIONS THE FISH HAS ////////
               ////////////////////////////////////////////////////////////////////////////////////////////////////////////
               if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
               {
                  strSQL = "SELECT S.tblEventID, S.FishID, S.tblSmartImageID, S.URL, S.tblSampleID, S.SampleID, S.CatchDate, S.StatRec, S.FishLength, S.FishWeight, S.Comments, S.MaturityStage, S.MaturityScale, S.SampleType, S.SampleOrigin, S.Area,  S.StockCode, S.Sex, S.PreparationMethod, S.GUID_Sample, S.GUID_EventID, S.GUID_SmartImage, S.FileName, S.Closed, S.tblCodeID_Area, dbo.getMode(S.tblSmartImageID) AS Mode, S.NoAnnotations AS NoTotalAnnotations, ISNULL(NoAnnotations.NoAnnotations, 0) AS NoAnnotations";
                  strSQL += " FROM            dbo.vw_SmartImages AS S LEFT OUTER JOIN";
                  strSQL += "    (SELECT     count(*) as NoAnnotations,   tblEventID, FishID FROM  dbo.tblAnnotationsMaturity GROUP BY tblEventID, FishID) AS NoAnnotations ON S.tblEventID = NoAnnotations.tblEventID AND S.FishID = NoAnnotations.FishID ";
                  strSQL += "WHERE (S.tblEventID = " + strEventID + ") ";
               }
               strSQL += "  order by FishID,SampleID ";

               ///////////////////////// RUNS THE SQL QUERY \\\\\\\\\\\\\\\\\\\\\\\\\\\\
               cmd.CommandText = strSQL;
               cmd.CommandType = CommandType.Text;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);

               ///////////////////////////////////////////////////////////////////
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  numRecord++;
                  ////////// IN CASE THE ROW IS A MULTIPLE OF 4 IT NEEDS TO START IT FROM THE SCRATCH \\\\\\\\\
                  if (numRecord % 4 == 0)
                  {
                     rowImages = new HtmlTableRow();
                     rowSampleNumber = new HtmlTableRow();
                     rowAnnotationDetails = new HtmlTableRow();
                  }
                  cellImage = new HtmlTableCell();
                  cellSampleName = new HtmlTableCell();
                  cellAnnotations = new HtmlTableCell();

                  //////////////////////Builds the image to add to the cell of image \\\\\\\\\\\\\\\\\\\
                  HyperLink imageSample = new HyperLink();
                  imageSample.ID = "img" + numRecord;
                  imageSample.ImageUrl = dr["URL"].ToString();                  
                  imageSample.NavigateUrl = "viewMaturitySampleAnnotation.aspx?tblEventID=" + strEventID + "&sampleID=" + dr["tblSampleID"].ToString();
                  ///// IF IT IS THE COUNTRY MANAGER THEN NEED TO GO TO ANOTHER PAGE SHOWING ALL THE ANNOTATIONS
                  if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
                  {
//                     imageSample.NavigateUrl = "viewMaturitySampleAnnotation.aspx?tblEventID=" + strEventID + "&sampleID=" + dr["tblSampleID"].ToString();
                     imageSample.NavigateUrl = "viewMaturityUserAnnotations.aspx?tblEventID=" + strEventID + "&sampleID=" + dr["tblSampleID"].ToString();
                  }
                  imageSample.ImageWidth = 200;
                  cellImage.Controls.Add(imageSample);
                  rowImages.Cells.Add(cellImage);
                  ///////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                  ////////////////// Puts the sampleID as title of the Image \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                  cellSampleName.InnerHtml = dr["SampleID"].ToString();
                  rowSampleNumber.Cells.Add(cellSampleName);
                  
                  if (dr["fishID"] != null)
                  {
                     cellAnnotations.InnerHtml = "<br><span style='font-weight:bold;font-size:12px'>FishID:</span> " + dr["FishID"].ToString();
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

                  ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                  //////////////// IF THE USER IS THE EVENT MANAGER THEN IT CAN SEE HOW MANY ANNOTATIONS THE FISH HAS ////////
                  ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                  if (RunDBOperations.checkIfUserIsEventManager(Session["user"].ToString().Trim(), strEventID))
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px;COLOR:#000000'>Number of Annotations:" + dr["NoAnnotations"].ToString() + "</span> ";
                                    }
                  else
                  { ////////////IF THE USER IS NOT THE MANAGER THEN IT CAN ONLY SEE IF IT HAS ANNOTATED THE IMAGE OR NOT \\\\\\\\\\\\\\\\\\\\\\\\\\
                     if (dr["HasAnnotation"].ToString() != "Yes")
                     {
                        cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px;COLOR:#FF0000'>Has annotation:" + dr["HasAnnotation"].ToString() + "</span> ";
                     }
                     else
                     {
                        cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px;COLOR:#000000'>Has annotation:" + dr["HasAnnotation"].ToString() + "</span> ";
                     }
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


   }
}