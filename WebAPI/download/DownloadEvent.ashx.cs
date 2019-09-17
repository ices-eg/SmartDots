using System;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using WebInterface.App_Code;

namespace WebInterface.download
{
   /// <summary>
   /// Summary description for DownloadEvent
   /// </summary>
   public class DownloadEvent : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
   {

      public void ProcessRequest(HttpContext context)
      {
         System.Web.HttpResponse Response = context.Response;
         Response.Clear();
         Response.Buffer = true;
         string tblEventID = "";
         if (context.Request.QueryString["tblEventID"] != null)
         {
            tblEventID = context.Request.QueryString["tblEventID"].ToString();

            try
            {
               //////////////////////////////////////////////////////////////////////    
               ICSharpCode.SharpZipLib.Zip.ZipOutputStream zStream = new ZipOutputStream(Response.OutputStream);
               zStream.UseZip64 = UseZip64.Off;
               zStream.SetLevel(9);
               ZipEntry zEntry = null;
               string strFileName = "SmartDots_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + HttpContext.Current.Session.SessionID.ToString();

               Write_folder_Stream(ref zStream, ref zEntry, getreport_Annotations(context), Response, "Report_Annotations" + strFileName);
               Write_folder_Stream(ref zStream, ref zEntry, getreport_DotsDistances(context), Response, "Report_DotsDistances" + strFileName);

               if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Download/ICES-Data-policy.pdf")))
               {
                  addFileToZIP(ref zEntry, ref zStream, "ICES-Data-policy.pdf");
               }
               ////////////////////////////////////////////////////////////////////////////////////      
               Response.ContentType = "application/zip";
               Response.AddHeader("content-disposition", "attachment; filename=SmartDots_Event_" + tblEventID + "_A" + HttpContext.Current.Session.SessionID.ToString() + "_" + DateTime.Now.Millisecond.ToString() + ".zip");
               zStream.Finish();
               zStream.Close();
               zStream.Dispose();

            }
            catch (Exception e)
            {
               Response.Redirect("~/index.aspx?error=An error has occurred");
            }
            Response.Flush();
         }
         else
         {
            Response.Redirect("~/manage/index.aspx?error=An event was not specified");
         }
      }

      public bool IsReusable
      {
         get
         {
            return false;
         }
      }

      public void addFileToZIP(ref ZipEntry zEntry, ref ICSharpCode.SharpZipLib.Zip.ZipOutputStream zStream, string strFileName)
      {
         if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Download/" + strFileName)))
         {
            zEntry = new ZipEntry(Path.GetFileName(strFileName));
            zStream.PutNextEntry(zEntry);
            writeFileZip(ref zStream, HttpContext.Current.Server.MapPath("~/Download/" + strFileName));
         }
      }

      public void writeFileZip(ref ZipOutputStream zStream, string strFileName)
      {
         byte[] buffer = new byte[4096];
         using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
         {
            int bytesRead;
            do
            {
               bytesRead = fs.Read(buffer, 0, buffer.Length);
               zStream.Write(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);
         }
      }

      public void Write_folder_Stream(ref ZipOutputStream zStream, ref ZipEntry zEntry, DataSet ds, HttpResponse Response, string filename)
      {

         StreamWriter sw = new StreamWriter(zStream);
         zEntry = new ZipEntry(filename + ".csv");
         zStream.PutNextEntry(zEntry);
         Write_tostream(ds, sw, ",");
         sw.Flush();
      }

      protected StreamWriter Write_tostream(DataSet ds, StreamWriter sw, string strSeparator)
      {
         foreach (DataTable dt in ds.Tables)
         {

            int iColCount = dt.Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
               sw.Write(dt.Columns[i]);
               if (i < iColCount - 1)
               {
                  sw.Write(strSeparator);
               }
            }
            sw.Write(sw.NewLine);
            // Now write all the rows.
            foreach (DataRow dr in dt.Rows)
            {
               for (int i = 0; i < iColCount; i++)
               {

                  if (!Convert.IsDBNull(dr[i]))
                  {
                     sw.Write(dr[i].ToString().Trim());
                  }
                  if (i < iColCount - 1)
                  {
                     sw.Write(strSeparator);
                  }
               }
               sw.Write(sw.NewLine);
            }

         }
         return sw;
      }

      public DataSet getreport_Annotations(System.Web.HttpContext context)
      {
         System.Web.HttpRequest Request = context.Request;

         string tblEventID = "";
         if (Request.QueryString["tblEventID"] != null)
         {
            tblEventID = Request.QueryString["tblEventID"].ToString();
         }

         string strSQL = "select * from [dbo].[vw_report_Annotations] where eventid = " + tblEventID;
         return RunDBOperations.getDataset(strSQL);
      }

      public DataSet getreport_DotsDistances(System.Web.HttpContext context)
      {
         System.Web.HttpRequest Request = context.Request;

         string tblEventID = "";
         if (Request.QueryString["tblEventID"] != null)
         {
            tblEventID = Request.QueryString["tblEventID"].ToString();
         }

         string strSQL = "select * from [dbo].[vw_report_DotsDistances] where eventid = " + tblEventID;
         return RunDBOperations.getDataset(strSQL);
      }

   }
}