using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.App_Code;

namespace WebAPI.Controllers
{
   public class getanalysesdynamicController : ApiController
   {
      string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";


      // GET: api/getanalysesdynamic/5
      public WebApiResult Get(string token)
      {
         var webApiResult = new WebApiResult();
         var dtoAnalyses = new List<object>();
         dynamic dtoAnalysis = new ExpandoObject();

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //string strSQL = "SELECT * FROM [dbo].[vw_ListEvents] where [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null ";
               //string strSQL = "SELECT * FROM [dbo].[vw_ListEvents] where [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and ( StartDate_date <= getdate() or [dbo].[GetIfEventManagerToken](@token,tblEventID) is not null) and Purpose like '%Age%' order by tblEventID desc";
               string strSQL = "SELECT  [dbo].[GetUserRole](@token,tblEventID) as Role, [dbo].[getisUserFinish](@token,tblEventID) as isUserFinish,[dbo].[GetReaderProgressInEventByToken](@token,tblEventID) as Progress, GUID_EventID, Purpose, tblEventID, NameOfEvent, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, StartDate_date, EndDate, Protocol, OrganizerEmail, Institute, CreateDate, ModifiedDate, SmartUser,  EventType, TypeOfStructure, intYear, Closed, Published,  report FROM [dbo].[vw_ListEvents] where [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and ( StartDate_date <= getdate() or [dbo].[GetIfEventManagerToken](@token,tblEventID) is not null) and Purpose like '%Age%' " +
                 " union all  " +
                 "SELECT [dbo].[GetUserRole](@token,tblEventID) as Role,[dbo].[getisUserFinish](@token,tblEventID) as isUserFinish, [dbo].[GetReaderProgressInEventByToken](@token,tblEventID) as Progress, GUID_EventID, Purpose, tblEventID, NameOfEvent, Species, tblCodeID_TypeOfStucture, tblCodeID_TypeOfExercice, StartDate, StartDate_date, EndDate, Protocol, REPLICATE('x', CHARINDEX('@', OrganizerEmail)) + RIGHT(OrganizerEmail, LEN(OrganizerEmail)  + 1 - CHARINDEX('@', OrganizerEmail)) AS OrganizerEmail , Institute, CreateDate, ModifiedDate, SmartUser,  EventType, TypeOfStructure, intYear, Closed, Published, report FROM [dbo].[vw_ListEvents]  where Purpose like '%Age%'  and Published = 1 " +
                 " and tblEventID not in (SELECT tblEventID FROM [dbo].[vw_ListEvents] where [dbo].[GetIfValidTokenForEvent](@token,[tblEventID]) is not null and ( StartDate_date <= getdate() or [dbo].[GetIfEventManagerToken](@token,tblEventID) is not null) and Purpose like '%Age%' )" +
                 "order by tblEventID desc";
               command.CommandText = strSQL;
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.CommandText = strSQL;
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
                  dtoAnalysis = new ExpandoObject(); // This will be a dynamic object so anything can be inside. 
                  dtoAnalysis.ID = reader["GUID_EventID"].ToString();

                  dtoAnalysis.Purpose = reader["Purpose"].ToString();
                  dtoAnalysis.EventID = reader["tblEventID"].ToString();
                  dtoAnalysis.NameOfEvent = reader["NameOfEvent"].ToString();

                  if (reader["Species"] != null)
                  {
                     dtoAnalysis.Species = reader["Species"].ToString();
                  }
                  if (reader["StartDate"] != null)
                  {
                     dtoAnalysis.StartDate = reader["StartDate"].ToString();
                  }
                  if (reader["EndDate"] != null)
                  {
                     dtoAnalysis.EndDate = reader["EndDate"].ToString();
                  }
                  if (reader["OrganizerEmail"] != null)
                  {
                     dtoAnalysis.OrganizerEmail = reader["OrganizerEmail"].ToString();
                  }
                  if (reader["Institute"] != null)
                  {
                     dtoAnalysis.Institute = reader["Institute"].ToString();
                  }
                  if (reader["EventType"] != null)
                  {
                     dtoAnalysis.EventType = reader["EventType"].ToString();
                  }
                  if (reader["intYear"] != null)
                  {
                     dtoAnalysis.Year = reader["intYear"].ToString();
                  }
                  if (bool.Parse(reader["Closed"].ToString()))
                  {
                     dtoAnalysis.Closed = "Event is closed";
                  }
                  else
                  {
                     dtoAnalysis.Closed = "Event is open";
                  }
                  if (bool.Parse(reader["Published"].ToString()))
                  {
                     dtoAnalysis.Closed = "Event is public";
                  }

                  // Checks the progress of the user in the current event!
                  if (reader["isUserFinish"] != null)
                  {
                     if (reader["isUserFinish"].ToString() == "1")
                     {
                        dtoAnalysis.UserProgress = "Finished";
                     }
                     else
                     {
                        dtoAnalysis.UserProgress = "Ongoing";
                     }
                  }
                  else
                  {
                     dtoAnalysis.UserProgress = "NA";
                  }
                  if (reader["Role"] != null)
                  {
                     dtoAnalysis.Role = reader["Role"].ToString();
                  }

                  if (reader["Progress"] != null)
                  {
                        dtoAnalysis.Progress = reader["Progress"].ToString();
                  }

                  dtoAnalyses.Add(dtoAnalysis);
               }
               //var userProgress = ctx.UserProgress.FirstOrDefault(x => x.AnalysisId == analysis.Id && x.UserId == userid);
              

               // Get's the analyses into the finnal object
               webApiResult.Result = dtoAnalyses;
            }
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }


         //Returns the result
         return webApiResult;
      }

      // POST: api/getanalysesdynamic
      public void Post([FromBody]string value)
      {
      }

      // PUT: api/getanalysesdynamic/5
      public void Put(int id, [FromBody]string value)
      {
      }

      // DELETE: api/getanalysesdynamic/5
      public void Delete(int id)
      {
      }
   }
}
