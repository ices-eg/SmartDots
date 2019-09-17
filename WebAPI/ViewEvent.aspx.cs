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

namespace WebInterface
{
   public partial class ViewEvent : System.Web.UI.Page
   {
      protected void Page_Load(object sender, EventArgs e)
      {
         /////////////////////////////////////////////////////////////////////////////////////
         /////////////////////////////////////////////////////////////////////////////////////

         string strEventID = Request.QueryString["key"];
         if (string.IsNullOrEmpty(strEventID))
         {
            Response.Redirect("ViewListEvents.aspx?Message=Event was not specified");
         }
         if (RunDBOperations.intExecuteScalarQuery("SELECT cast([Published] as int) FROM [dbo].[tblEvent]where tblEventID = " + strEventID) < 1 )
         {
            Response.Redirect("ViewListEvents.aspx?Message=Event is not published!");
         }

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
                     cmd.CommandText = "SELECT GUID_EventID,SummaryReport,  Purpose, tblEventID, NameOfEvent, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, EndDate, Protocol, REPLICATE('x', CHARINDEX('@', OrganizerEmail)) + RIGHT(OrganizerEmail, LEN(OrganizerEmail) + 1 - CHARINDEX('@', OrganizerEmail)) as  OrganizerEmail, Institute, CreateDate, ModifiedDate, SmartUser, EventType, TypeOfStructure, intYear, Closed, Published, report FROM  dbo.vw_ListEvents where tbleventid = " + strEventID;
                     cmd.CommandType = CommandType.Text;
                     cmd.ExecuteNonQuery();
                     da.SelectCommand = cmd;
                     da.Fill(ds);
                     ///////////////////////////////////////////////////////////////////
                     foreach (DataRow dr in ds.Tables[0].Rows)
                     {
                        lblNameOfEvent.Text = dr["NameOfEvent"].ToString();
                        lblSpecies.Text = dr["Species"].ToString();
                        lblEventID.Text = dr["tblEventID"].ToString();
                        lblStartDate.Text = dr["StartDate"].ToString();
                        lblEndDate.Text = dr["EndDate"].ToString();
                        lblEmailOrganizer.Text = dr["OrganizerEmail"].ToString();
                        lblPurpose.Text = dr["Purpose"].ToString();
                        lblEventType.Text = dr["EventType"].ToString();
                        lblClosed.Text = dr["Closed"].ToString();
                        if (!string.IsNullOrEmpty(dr["Report"].ToString()))
                        {
                           hplnkReport.NavigateUrl = "SampleImages/" + dr["intyear"].ToString() + "/" + strEventID + "/" + dr["Report"].ToString();
                           if (dr["SummaryReport"] != null)
                           {
                              hplnkReportSummary.NavigateUrl = "SampleImages/" + dr["intyear"].ToString() + "/" + strEventID + "/" + dr["SummaryReport"].ToString();
                           }
                           hplnkDownloadData.NavigateUrl = "~/download/DownloadEvent.ashx?tblEventID=" + strEventID;
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
         string strField = Request.QueryString["field"];
         string strOrder = Request.QueryString["order"];
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
               
               
                  strSQL = "SELECT        Img.tblEventID, Img.FishID, GUID_PublishedSmartImage, Img.tblSmartImageID, Img.URL, Img.tblSampleID, Img.SampleID, Img.CatchDate, Img.StatRec, Img.FishLength, Img.FishWeight, Img.Comments, Img.MaturityStage, Img.MaturityScale,  Img.SampleType, Img.SampleOrigin, Img.Area, Img.StockCode, Img.Sex, Img.PreparationMethod, Img.GUID_Sample, Img.GUID_EventID, Img.GUID_SmartImage, Img.FileName, Img.Closed, Img.tblCodeID_Area,  dbo.getMode(Img.tblSmartImageID) AS Mode, Img.NoAnnotations AS NoTotalAnnotations, dbo.hasFixedLine(Img.tblSmartImageID) AS hasFixedLine ";
                  strSQL += " FROM            dbo.vw_SmartImages AS Img ";
                  strSQL += "   WHERE        Img.tblEventID = "  + strEventID ;
               if (!string.IsNullOrEmpty(strField) && !string.IsNullOrEmpty(strOrder))
               {
                  strSQL += " ORDER BY " + strField + " " + strOrder;
               }
               else
               { 
                  strSQL += " ORDER BY Img.DCatchDate asc";
               }
               
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
                  imageSample.ImageUrl = "http://smartdots.ices.dk/sampleimages/" + SmartUtilities.getYearEvent(strEventID) + "/" + strEventID + "/pub/" + dr["GUID_PublishedSmartImage"].ToString() + ".jpg";
                  imageSample.NavigateUrl = "viewImage.aspx?tblEventID=" + strEventID + "&SmartImageID=" + dr["tblSmartImageID"].ToString() ;
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
                  cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>No. of annotations:</span> " + dr["NoTotalAnnotations"].ToString();
                  if (dr["Mode"] != null)
                  {
                     cellAnnotations.InnerHtml += "<br><span style='font-weight:bold;font-size:12px'>Modal age:</span> " + dr["Mode"].ToString();
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

      protected void lnkOrderFishWeighAsc_Click(object sender, EventArgs e)
      {

      }

      protected void ddlMode_SelectedIndexChanged(object sender, EventArgs e)
      {
      }
   }
}