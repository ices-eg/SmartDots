using Newtonsoft.Json;
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
   public class GetGuestTokenController : ApiController
   {

      // GET: api/GetGuestToken
      public WebApiResult Get()
      {
         var webApiResult = new WebApiResult();
         try
         {
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_getGuestToken";
               command.CommandType = CommandType.StoredProcedure;
               connection.Open();
               SqlParameter outputIdParam = new SqlParameter("@token", SqlDbType.NVarChar, 60) { Direction = ParameterDirection.Output };
               command.Parameters.Add(outputIdParam);
               command.ExecuteNonQuery();
               string strToken = outputIdParam.Value.ToString();
               webApiResult.Result = strToken;
            }
            return webApiResult;
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }

      }
      public WebApiResult Post()
      {
         var webApiResult = new WebApiResult();
         try
         {
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_getGuestToken";
               command.CommandType = CommandType.StoredProcedure;
               connection.Open();
               SqlParameter outputIdParam = new SqlParameter("@token", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
               command.Parameters.Add(outputIdParam);
               command.ExecuteNonQuery();
               string strToken = outputIdParam.Value.ToString();
            }
            webApiResult.Result = true;
            return webApiResult;
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }
      }
   }
}
