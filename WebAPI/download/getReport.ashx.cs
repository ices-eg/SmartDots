using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace WebInterface.download
{
   /// <summary>
   /// Summary description for getReport
   /// </summary>
   public class getReport : IHttpHandler
   {

      public void ProcessRequest(HttpContext context)
      {
         context.Response.Buffer = true;
         context.Response.Clear();

         if (context.Request.QueryString["tblEventID"] != null)
         {

            string strEventID = context.Request.QueryString["tblEventID"].ToString();
            string strFileName = "SmartDots_Event_" + strEventID + "_Report_Donwload" + DateTime.Now.Millisecond ;
            string strURL = string.Format("http://taf.ices.local/TAFtest/api/smartDots/report/{0}?force=true", strEventID);
            WebRequest request = HttpWebRequest.Create(strURL);
            string strFilePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/" + strFileName + ".zip");
            if (!File.Exists(strFilePath))
            {
               using (var client = new WebClient())
               {
                  client.DownloadFile(strURL, strFilePath);
               }
            }
            context.Response.ContentType = "octet/stream";
            context.Response.AddHeader("content-disposition", "attachment; filename=\"" + strFileName + ".zip\"");

            context.Response.WriteFile("~/temp/" + strFileName + ".zip");
            context.Response.Flush();
            context.Response.Close();
            context.Response.End();
         }
         else
         {
            context.Response.Redirect("~/FindAcronymDocuments.aspx?error=An event was not specified");
         }
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