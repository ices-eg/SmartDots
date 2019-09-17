using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.App_Code;

namespace WebAPI.Controllers
{
   public class ToggleAnalysisUserProgressController : ApiController
   {

      public WebApiResult Get(string token, Guid analysisid)
      {
         var webApiResult = new WebApiResult();

         try
         {
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_toggleAnalysisUserProgress";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_EventID", analysisid));

               connection.Open();
               try
               {
                  command.ExecuteReader();
               }
               catch (SqlException e)
               {
                  throw new Exception("Database error", e);
               }

               webApiResult.Result = "true";
            }
         }
         catch (Exception e)
         {
            webApiResult.Result = "false";
         }
         return webApiResult;
      }

      // PUT: api/ToggleAnalysisUserProgress/5
      public WebApiResult Post(string token, [FromBody] Guid analysisid)
      {
         var webApiResult = new WebApiResult();

         try
         {
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_toggleAnalysisUserProgress";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_EventID", analysisid));

               connection.Open();
               try
               {
                  command.ExecuteReader();
               }
               catch (SqlException e)
               {
                  throw new Exception("Database error", e);
               }

               webApiResult.Result = "true";
            }
         }
         catch (Exception e)
         {
            webApiResult.Result = "false";
         }
         return webApiResult;
      }


   }
}
