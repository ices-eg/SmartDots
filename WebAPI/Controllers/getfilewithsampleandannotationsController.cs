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
   public class getfilewithsampleandannotationsController : ApiController
   {
      string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";


      // GET: api/GetFile
      public IEnumerable<string> Get()
      {
         return new string[] { "value1", "value2" };
      }
      public WebApiResult Post(string token, Guid id, bool withAnnotations, bool withSample)
      {
         var webApiResult = new WebApiResult();
         return webApiResult;
      }
      // GET: api/GetFile/5
      public WebApiResult Get(string token, Guid id, bool withAnnotations, bool withSample)
      {
         var webApiResult = new WebApiResult();
         try
         {
            DtoFile dtoFile = new DtoFile();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               command.CommandText = "up_WebAPI_getFileWhistSampleAnnotations";
               command.CommandType = CommandType.StoredProcedure;
               ////////// Will add the parameters to the query, this will be more safe than building the query by concatenating the strings \\\\\\\\\
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_SmartImage", SqlDbType.VarChar);
               command.Parameters["@GUID_SmartImage"].Value = id.ToString();

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
                  dtoFile.Id = Guid.Parse(reader["GUID_SmartImage"].ToString());

                  // @Carlos - TODO : This will have to be in the logic of the WebAPI, if the event is closed then this has to be turned t true
                  dtoFile.IsReadOnly = bool.Parse(reader["Closed"].ToString());
                  if (reader["Scale"] != null)
                  {
                     if (!string.IsNullOrEmpty(reader["Scale"].ToString()))
                     {
                        dtoFile.Scale = Decimal.Parse(reader["Scale"].ToString());
                     }
                  }
                  dtoFile.Filename = reader["FileName"].ToString();

                  ///////////////////////////////////////////////////////////////////////////////
                  ///////////////////////////////////////////////////////////////////////////////
                  // @Carlos - Todo: Need to load all the anotations that have been done for that image
                  // Here will be decided if the user sees all the anotations or only the ones he has done
                  ///////////////////////////////////////////////////////////////////////////////
                  ///////////////////////////////////////////////////////////////////////////////
                  if (withAnnotations) // This means that smartdots does not want annotations
                  {
                     dtoFile.Annotations = GetListAnnotations(token, dtoFile.Id);
                     dtoFile.AnnotationCount = dtoFile.Annotations.Count;
                  }
                  ///////////////////////////////////////////////////////////////////////////////
                  ///////////////////////////////////////////////////////////////////////////////
                  dtoFile.SampleNumber = reader["SampleID"].ToString();

                  if (int.Parse(reader["EventIsReadOnlyForThisUser"].ToString()) == 0)
                  {
                     // this means that the user is a country coordinator that has only the rights to see the annotations of his age readers
                     dtoFile.IsReadOnly = true;
                  }
                  else
                  {
                     // In case the workshop is closed then the file will be read only;
                     dtoFile.IsReadOnly = bool.Parse(reader["Closed"].ToString());
                  }

                  //////////////// This sample is to display the fields in the DB for that sample \\\\\\\\\\\\\\\
                  DtoSample dtoSample = new DtoSample();
                  dtoSample.Id = Guid.Parse(reader["GUID_Sample"].ToString());
                  if (withSample) // In this case it is a dummy sample, but it is not needed if passed with false
                  {

                     if (int.Parse(reader["NoAnnotations"].ToString()) > 0)
                     {
                        if (int.Parse(reader["NoApprovedAnnotations"].ToString()) > 0)
                        {
                           dtoSample.StatusCode = "Done";
                           dtoSample.StatusColor = "#00b300";
                        }
                        else
                        {
                           dtoSample.StatusCode = "Work in progress";
                           dtoSample.StatusColor = "#ff8000";
                        }
                     }
                     else
                     {
                        dtoSample.StatusCode = "To do";
                        dtoSample.StatusColor = "#cc0000";
                     }


                     dtoSample.StatusRank = 20;
                     dtoFile.Sample = dtoSample;
                     dtoFile.Sample.DisplayedProperties = new Dictionary<string, string>();
                     dtoFile.SampleId = Guid.Parse(reader["GUID_Sample"].ToString());

                     if (reader["Scale"] != null)
                     {
                        if (!string.IsNullOrEmpty(reader["Scale"].ToString()))
                        {
                           dtoFile.Scale = decimal.Parse(reader["Scale"].ToString());
                        }
                     }
                     if (reader["FishID"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("FishID", reader["FishID"].ToString());
                     }

                     if (reader["Species"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Species", reader["Species"].ToString());
                     }
                     if (reader["CatchDate"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Catch Date", reader["CatchDate"].ToString());
                     }
                     if (reader["FishLength"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Fish Length", reader["FishLength"].ToString());
                     }
                     if (reader["FishWeight"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Fish Weight", reader["FishWeight"].ToString());
                     }
                     if (reader["Area"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Area", reader["Area"].ToString());
                     }
                     if (reader["StockCode"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Stock Code", reader["StockCode"].ToString());
                     }
                     if (reader["StatRec"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Statistical Rectangle", reader["StatRec"].ToString());
                     }
                     if (reader["Sex"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Sex", reader["Sex"].ToString());
                     }
                     if (reader["SampleOrigin"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Sample Origin", reader["SampleOrigin"].ToString());
                     }
                     if (reader["SampleType"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Sample Type", reader["SampleType"].ToString());
                     }
                     if (reader["MaturityScale"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Maturity Scale", reader["MaturityScale"].ToString());
                     }
                     if (reader["MaturityStage"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Maturity Stage", reader["MaturityStage"].ToString());
                     }
                     if (reader["PreparationMethod"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Preparation Method", reader["PreparationMethod"].ToString());
                     }
                     if (reader["Comments"] != null)
                     {
                        dtoFile.Sample.DisplayedProperties.Add("Comments", reader["Comments"].ToString());
                     }
                  }
               }
               // Adds also this sample to the result
               webApiResult.Result = dtoFile;
               return webApiResult;
            }
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }
      }

      /// <summary>
      /// @Carlos : This was done to return all the list of annotations
      /// </summary>
      /// <param name="token">Token the user has provided to give access to work with this event</param>
      /// <param name="fileId">The Smart image GUID</param>
      /// <returns></returns>
      protected List<DtoAnnotation> GetListAnnotations(string token, Guid fileId)
      {
         List<DtoAnnotation> listAnnotations = new List<DtoAnnotation>();

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               command.CommandText = "up_WebAPI_getListAnnotations";
               command.CommandType = CommandType.StoredProcedure;
               ////////// Will add the parameters to the query, this will be more safe than building the query by concatenating the strings \\\\\\\\\
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_SmartImage", SqlDbType.VarChar);
               command.Parameters["@GUID_SmartImage"].Value = fileId.ToString();
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
                  DtoAnnotation dtoAnnotation = new DtoAnnotation();

                  dtoAnnotation.Id = Guid.Parse(reader["GUID_AnnotationID"].ToString());
                  if (reader["SmartUser"] != null)
                  {
                     dtoAnnotation.LabTechnician = reader["SmartUser"].ToString();
                  }
                  dtoAnnotation.IsApproved = bool.Parse(reader["IsApproved"].ToString());

                  dtoAnnotation.IsReadOnly = bool.Parse(reader["IsReadOnly"].ToString());

                  if (int.Parse(reader["EventIsReadOnlyForThisUser"].ToString()) == 0)
                  {
                     // this means that the user is a country coordinator that has only the rights to see the annotations of his age readers
                     dtoAnnotation.IsReadOnly = true;
                  }
                  else
                  {
                     dtoAnnotation.IsReadOnly = bool.Parse(reader["IsReadOnly"].ToString());
                  }

                  dtoAnnotation.IsFixed = bool.Parse(reader["IsFixed"].ToString());
                  dtoAnnotation.FileId = fileId;
                  dtoAnnotation.Lines = GetListLines(token, Guid.Parse(reader["GUID_AnnotationID"].ToString()));
                  dtoAnnotation.Dots = GetListDots(token, Guid.Parse(reader["GUID_AnnotationID"].ToString()));
                  dtoAnnotation.Result = dtoAnnotation.Dots.Count;
                  dtoAnnotation.DateCreation = DateTime.Parse(reader["SmartDotsCreateDate"].ToString());
                  dtoAnnotation.ParameterId = new Guid();
                  if (reader["Comment"] != null)
                  {
                     dtoAnnotation.Comment = reader["Comment"].ToString();
                  }
                  if (reader["Edge"] != null)
                  {
                     dtoAnnotation.Edge = reader["Edge"].ToString();
                  }
                  if (reader["Nucleus"] != null)
                  {
                     dtoAnnotation.Nucleus = reader["Nucleus"].ToString();
                  }
                  if (reader["AQ_Code"] != null)
                  {
                     if (!String.IsNullOrEmpty(reader["AQ_Code"].ToString()))
                     {
                        dtoAnnotation.QualityId = Guid.Parse(reader["AQ_Code"].ToString());
                     }
                  }
                  listAnnotations.Add(dtoAnnotation);
               }
            }
         }
         catch (Exception e)
         {
            return listAnnotations;
         }
         return listAnnotations;
      }


      protected List<DtoLine> GetListLines(string token, Guid AnnotationId)
      {
         List<DtoLine> listLines = new List<DtoLine>();

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               command.CommandText = "up_WebAPI_getListLines";
               command.CommandType = CommandType.StoredProcedure;
               ////////// Will add the parameters to the query, this will be more safe than building the query by concatenating the strings \\\\\\\\\
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_AnnotationID", SqlDbType.VarChar);
               command.Parameters["@GUID_AnnotationID"].Value = AnnotationId.ToString();
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
                  DtoLine dtoLine = new DtoLine();
                  dtoLine.Id = Guid.Parse(reader["GUID_LineID"].ToString());
                  dtoLine.AnnotationId = Guid.Parse(reader["GUID_AnnotationID"].ToString());
                  if (reader["Color"] != null)
                  {
                     dtoLine.Color = reader["Color"].ToString();
                  }
                  dtoLine.X1 = int.Parse(reader["X1"].ToString());
                  dtoLine.X2 = int.Parse(reader["X2"].ToString());
                  dtoLine.Y1 = int.Parse(reader["Y1"].ToString());
                  dtoLine.Y2 = int.Parse(reader["Y2"].ToString());
                  dtoLine.LineIndex = int.Parse(reader["LineIndex"].ToString());
                  dtoLine.Width = int.Parse(reader["Width"].ToString());
                  listLines.Add(dtoLine);
               }
            }
         }
         catch (Exception e)
         {
            return listLines;
         }
         return listLines;
      }




      protected List<DtoDot> GetListDots(string token, Guid AnnotationId)
      {
         List<DtoDot> listDots = new List<DtoDot>();

         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               //This will pull all the samples for that event where the user has permssion
               command.CommandText = "up_WebAPI_getListDots";
               command.CommandType = CommandType.StoredProcedure;
               ////////// Will add the parameters to the query, this will be more safe than building the query by concatenating the strings \\\\\\\\\
               command.Parameters.Add("@token", SqlDbType.VarChar);
               command.Parameters["@token"].Value = token;
               command.Parameters.Add("@GUID_AnnotationID", SqlDbType.VarChar);
               command.Parameters["@GUID_AnnotationID"].Value = AnnotationId.ToString();
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
                  DtoDot dtoDot = new DtoDot();
                  dtoDot.Id = Guid.Parse(reader["GUID_DotsID"].ToString());
                  dtoDot.AnnotationId = Guid.Parse(reader["GUID_AnnotationID"].ToString());
                  if (reader["Color"] != null)
                  {
                     dtoDot.Color = reader["Color"].ToString();
                  }
                  if (reader["Shape"] != null)
                  {
                     if (!string.IsNullOrEmpty(reader["Shape"].ToString()))
                     {
                        dtoDot.DotShape = reader["Shape"].ToString();
                     }
                  }
                  if (reader["WaterType"] != null)
                  {
                     dtoDot.DotType = reader["WaterType"].ToString();
                  }
                  dtoDot.X = int.Parse(reader["X"].ToString());
                  dtoDot.Y = int.Parse(reader["Y"].ToString());
                  dtoDot.DotIndex = int.Parse(reader["DotIndex"].ToString());
                  dtoDot.Width = int.Parse(reader["Width"].ToString());


                  listDots.Add(dtoDot);
               }
            }
         }
         catch (Exception e)
         {
            return listDots;
         }
         return listDots;
      }

   }
}
