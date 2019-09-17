using ICSharpCode.SharpZipLib.Zip;
using System;
using WebInterface.App_Code;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.download
{
   /// <summary>
   /// Summary description for downloadListEvents
   /// </summary>
   public class downloadListEvents : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
   {
      public void ProcessRequest(HttpContext context)
      {
         System.Web.HttpResponse Response = context.Response;
         Response.Clear();
         Response.Buffer = true;

         try
         {
            //////////////////////////////////////////////////////////////////////    
            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zStream = new ZipOutputStream(Response.OutputStream);
            zStream.UseZip64 = UseZip64.Off;
            zStream.SetLevel(9);
            ZipEntry zEntry = null;
            string strFileName = "SmartDots_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + HttpContext.Current.Session.SessionID.ToString();

            Write_folder_Stream(ref zStream, ref zEntry, getSmartUsers(context), Response, "ListEvents" + strFileName);

            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Download/ICES-Data-policy.pdf")))
            {
               addFileToZIP(ref zEntry, ref zStream, "ICES-Data-policy.pdf");
            }
            ////////////////////////////////////////////////////////////////////////////////////      
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=SmartDots_ListOfEvents_A" + HttpContext.Current.Session.SessionID.ToString() + "_" + DateTime.Now.Millisecond.ToString() + ".zip");
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


      public void addFileToZIP(ref ZipEntry zEntry, ref ICSharpCode.SharpZipLib.Zip.ZipOutputStream zStream, string strFileName)
      {
         if (File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Download/" + strFileName)))
         {
            zEntry = new ZipEntry(Path.GetFileName(strFileName));
            zStream.PutNextEntry(zEntry);
            writeFileZip(ref zStream, HttpContext.Current.Server.MapPath("~/Download/" + strFileName));
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

      public DataSet getSmartUsers(System.Web.HttpContext context)
      {
         System.Web.HttpRequest Request = context.Request;

         string strSQL = "SELECT tblEventID, Purpose, NameOfEvent, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, EndDate, Protocol, OrganizerEmail, Institute,  EventType, TypeOfStructure, intYear ";
         strSQL = strSQL + " FROM dbo.vw_ListEvents WHERE NameOfEvent not like '%Public training%' ";
         if (Request.QueryString["year"] != null)
         {
            if (Request.QueryString["year"] != "0")
            {
               strSQL = strSQL + " and intYear = " + Request.QueryString["year"];
            }
         }

         return RunDBOperations.getDataset(strSQL);
      }

      public bool IsReusable
      {
         get
         {
            return false;
         }
      }
   }

}