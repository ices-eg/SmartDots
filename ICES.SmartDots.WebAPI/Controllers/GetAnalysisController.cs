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
   public class GetAnalysisController : ApiController
   {
      string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";

      // GET: api/GetAnalysis/5
      public WebApiResult Get(string token, Guid id)
      {
         var webApiResult = new WebApiResult();
         string tblEventID = "0";
         bool bnlShowNucleus = false;
         bool bnlShowEdge = false;
         Analysis analysis = new Analysis();
         List<DtoAnalysisParameter> sdParameters = new List<DtoAnalysisParameter>();

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;

               //string strSQL = "SELECT  dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents WHERE [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and guid_eventID = @GUID_EventID union all SELECT  dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents where Published = 1";
               //               string strSQL = "SELECT  dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents WHERE [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and guid_eventID = @GUID_EventID union all SELECT  dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents where Published = 1";
               //               string strSQL = "SELECT  dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents WHERE [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and guid_eventID = @GUID_EventID ";
               string strSQL = "SELECT  CASE WHEN [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is NULL THEN 'pub/' ELSE '' END  as folder, dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents WHERE [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and guid_eventID = @GUID_EventID union all SELECT CASE WHEN [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is NULL THEN 'pub/' ELSE '' END  as folder, dbo.GetIfValidToken(@token) as SmartUser,  * FROM dbo.vw_ListEvents where Published = 1  and tblEventID not in (SELECT  tblEventID FROM dbo.vw_ListEvents WHERE [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null ) and guid_eventID = @GUID_EventID";

               command.CommandText = strSQL;
               command.Parameters.Add("@token", SqlDbType.VarChar, 60);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_EventID", SqlDbType.VarChar, 60);
               command.Parameters["@GUID_EventID"].Value = id.ToString();


               connection.Open();
               SqlDataReader reader;
               try
               {
                  reader = command.ExecuteReader();

               }
               catch (SqlException e)
               {
                  throw new Exception("Database error", e);
               }
               while (reader.Read())
               {
                  analysis.Comment = "This was done doing the new software";
                  analysis.Id = Guid.Parse(reader["GUID_EventID"].ToString());
                  analysis.Number = int.Parse(reader["tblEventID"].ToString());
                  analysis.HeaderInfo = "Login as " + reader["SmartUser"].ToString() + ", ICES  - " + reader["EventType"].ToString() + " (" + reader["tblEventID"].ToString() + "):" + reader["NameOfEvent"].ToString();

                  if (reader["showNucleus"].ToString() == "1")
                  {
                     bnlShowNucleus = true;
                  }
                  if (reader["showEdge"].ToString() == "1")
                  {
                     bnlShowEdge = true;
                  }

                  Folder f = new Folder();
                  f.Path = "http://smartdots.ices.dk/SampleImages/" + reader["intYear"].ToString() + "/" + reader["tblEventID"].ToString() + "/" + reader["folder"].ToString();
                  tblEventID = reader["tblEventID"].ToString();
                  analysis.Folder = f;

                  List<Parameter> parameters = new List<Parameter>();
                  Parameter p = new Parameter();
                  p.Id = new Guid();
                  p.Code = "OWR";
                  p.Description = "Age, method: Otolith Winter Rings";
                  parameters.Add(p);
                  analysis.Parameters = parameters;
                  /* This for some reason is needed */

                  foreach (var parameter in parameters)
                  {
                     var sdParameter = (DtoAnalysisParameter)Helper.ConvertType(parameter, typeof(DtoAnalysisParameter));
                     sdParameters.Add(sdParameter);
                  }
               }

            }
            // This will convert the variable sdanalysis to return has an object
            var sdanalysis = (DtoAnalysis)Helper.ConvertType(analysis, typeof(DtoAnalysis));
            sdanalysis.AnalysisParameters = sdParameters;

            if (analysis.Folder != null)
            {
               sdanalysis.Folder = (DtoFolder)Helper.ConvertType(analysis.Folder, typeof(DtoFolder));
            }

            if (bnlShowNucleus)
            {
               sdanalysis.ShowNucleusColumn = true;
            }
            if (bnlShowEdge)
            {
               sdanalysis.ShowEdgeColumn = true;
            }

            sdanalysis.UserCanPin = getIfEventManager(token, tblEventID);
            webApiResult.Result = sdanalysis;
            return webApiResult;
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }
      }


      bool getIfEventManager(string token, string tblEventID)
      {

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;

               string strSQL = "select [dbo].[GetIfEventManagerToken](@token, @tblEventID) as Manager";
               command.CommandText = strSQL;
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@tblEventID", SqlDbType.VarChar);
               command.Parameters["@tblEventID"].Value = tblEventID;
               connection.Open();
               SqlDataReader reader;
               try
               {
                  reader = command.ExecuteReader();

               }
               catch (SqlException e)
               {
                  throw new Exception("Database error", e);
               }
               while (reader.Read())
               {
                  if (reader["Manager"] != null)
                  {
                     if (!string.IsNullOrEmpty(reader["Manager"].ToString()))
                     {
                        return true;
                     }
                  }

               }
            }
         }
         catch (Exception e)
         {
            return false;
         }

         return false;

      }

   }
}
