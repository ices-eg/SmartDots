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
   public class GetFilesController : ApiController
   {

      static string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";

      /// <summary>
      /// This will add the list of properties needed for that event;
      /// </summary>
      /// <param name="tblEventID"></param>
      /// <returns></returns>
      public static List<string> getExtraProperties(int tblEventID)
      {
         List<string> listProperties = new List<string>();
         //listProperties.Add("isApproved");

         // HERE WILL OPEN THE DATABASE AND GET ALL THE EXTRA PROPERTIES 
         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               // In this procedure is implemented the logic that if the event has ended then the user has access to all the annotations and becomes readonly
               command.CommandText = "select * from tblEventData where tblTypeID = 14 and tblEventID = " + tblEventID;
               command.CommandType = CommandType.Text;
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
                  listProperties.Add(reader["Value"].ToString());
               }
            }
         }
         catch (Exception e)
         {

         }

         return listProperties;
      }
      // GET: api/GetFiles
      public IEnumerable<string> Get()
      {
         return new string[] { "value1", "value2" };
      }

      // GET: api/GetFiles/5
      public string Get(string token, Guid analysisid, [FromBody] List<string> imagenames)
      {
         return "value";
      }

      // POST: api/GetFiles
      public WebApiResult Post(string token, Guid analysisid, [FromBody] List<string> imagenames)
      {
         // Going to get the extra properties here to avoid asking everytime it comes a sample
         int tblEventID = RunDBOperation.intExecuteScalarQuery("select tblEventID from tblEvent where GUID_EventID = '" + analysisid.ToString() + "'");
         List<string> listProperties = GetFilesController.getExtraProperties(tblEventID);

         var webApiResult = new WebApiResult();
         try
         {
            List<DtoFile> sdFiles = new List<DtoFile>();
            DtoFile dtoFile = new DtoFile();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               // In this procedure is implemented the logic that if the event has ended then the user has access to all the annotations and becomes readonly
               command.CommandText = "up_web_getListFiles";
               command.CommandType = CommandType.StoredProcedure;
               ////////// Will add the parameters to the query, this will be more safe than building the query by concatenating the strings \\\\\\\\\
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_EventID", SqlDbType.VarChar);
               command.Parameters["@GUID_EventID"].Value = analysisid.ToString();

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
                  dtoFile = new DtoFile();
                  if (string.IsNullOrEmpty(reader["GUID_SmartImage"].ToString()))
                  {
                     throw new System.ArgumentException("Not all the samples have images, the event is not ready to read!", "Samples without images");
                  }
                  dtoFile.Id = Guid.Parse(reader["GUID_SmartImage"].ToString());
                  dtoFile.IsReadOnly = bool.Parse(reader["Closed"].ToString());
                  dtoFile.Scale = Decimal.Parse("0.0");
                  // Case the user does not have access to the event then he needs to be given the pulic image
                  dtoFile.Filename = reader["FileName"].ToString();
                  if (reader["NoAnnotations"] != null)
                  {
                     dtoFile.AnnotationCount = int.Parse(reader["NoAnnotations"].ToString());
                  }
                  //  dtoFile.Annotations = GetListAnnotations(token,analysisid);
                  // @Carlos - I'm not sure if it is correct not to load the annotations here;
                  dtoFile.Annotations = new List<DtoAnnotation>();
                  //                        dtoFile.SampleNumber = long.Parse(reader["tblSampleID"].ToString());
                  dtoFile.SampleNumber = reader["SampleID"].ToString();
                  dtoFile.DisplayName = reader["originalfFilename"].ToString();
                  /////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                  /////////////////////////////// BIT TO ADD THE EXTRA PROPERTIES \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                  /////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                  Dictionary<string, object> lstProperties = new Dictionary<string, object>();
                  foreach (string str in listProperties)
                  {
                     lstProperties.Add(str, reader[str].ToString());
                  }
                  dtoFile.SampleProperties = lstProperties;
                  /////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                  /////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                  if (reader["Scale"] != null)
                  {
                     if (!string.IsNullOrEmpty(reader["Scale"].ToString()))
                     {
                        dtoFile.Scale = decimal.Parse(reader["Scale"].ToString());
                     }
                  }
                  //////////////// Not sure what the sample is for \\\\\\\\\\\\\\\
                  DtoSample dtoSample = new DtoSample();
                  dtoSample.Id = Guid.Parse(reader["GUID_Sample"].ToString());
                  if (int.Parse(reader["NoAnnotations"].ToString()) > 0)
                  {
                     if (int.Parse(reader["NoApprovedAnnotations"].ToString()) > 0)
                     {
                        dtoSample.StatusCode = "Done";
                        dtoSample.StatusColor = "#00b300";
                        dtoSample.StatusRank = 3;
                     }
                     else
                     {
                        dtoSample.StatusCode = "Work in progress";
                        dtoSample.StatusColor = "#ff8000";
                        dtoSample.StatusRank = 2;
                     }
                  }
                  else
                  {
                     dtoSample.StatusCode = "To do";
                     dtoSample.StatusColor = "#cc0000";
                     dtoSample.StatusRank = 1;
                  }

                  dtoFile.Sample = dtoSample;
                  sdFiles.Add(dtoFile);

               }
               // Adds also this sample to the result
               webApiResult.Result = sdFiles;
               return webApiResult;
            }
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }
      }

      // PUT: api/GetFiles/5
      public void Put(int id, [FromBody] string value)
      {
      }

      // DELETE: api/GetFiles/5
      public void Delete(int id)
      {
      }
   }
}
