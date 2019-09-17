using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WebAPI.App_Code
{
   public class RunDBOperation
   {
      public static string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";

      public static bool AddAnnotation(string token, DtoAnnotation annotation)
      {
         try
         {
            List<DtoFile> sdFiles = new List<DtoFile>();
            DtoFile dtoFile = new DtoFile();
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_createAnnotation";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_Annotation", annotation.Id.ToString()));
               if (annotation.Comment != null)
                  command.Parameters.Add(new SqlParameter("@Comment", annotation.Comment));
               command.Parameters.Add(new SqlParameter("@IsFixed", annotation.IsFixed.ToString()));
               command.Parameters.Add(new SqlParameter("@IsReadOnly", annotation.IsReadOnly.ToString()));
               command.Parameters.Add(new SqlParameter("@IsApproved", annotation.IsApproved.ToString()));
               if (annotation.LabTechnician != null)
                  command.Parameters.Add(new SqlParameter("@UserName", annotation.LabTechnician.ToString()));
               command.Parameters.Add(new SqlParameter("@DateCreation", annotation.DateCreation.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_SmartImage", annotation.FileId.ToString()));
               if (annotation.Edge != null)
               {
                  command.Parameters.Add(new SqlParameter("@Edge", annotation.Edge.ToString()));
               }
               if (annotation.Nucleus != null)
               {
                  command.Parameters.Add(new SqlParameter("@Nucleus", annotation.Nucleus.ToString()));
               }
               if (annotation.QualityId != null)
               {
                  command.Parameters.Add(new SqlParameter("@AQ_Code", annotation.QualityId.ToString()));
               }

               connection.Open();
               SqlDataReader reader;
               try
               {

                  reader = command.ExecuteReader();
                  // This will add all the lines that come in the annotation 
                  if (annotation.Lines != null)
                  {
                     foreach (DtoLine line in annotation.Lines)
                     {
                        AddLines(token, line);
                     }
                  }
                  // This will add all the dots that come in the annotation 
                  if (annotation.Dots != null)
                  {
                     foreach (DtoDot dot in annotation.Dots)
                     {
                        AddDots(token, dot);
                     }
                  }
               }
               catch (SqlException e)
               {
                  throw new Exception("Database error", e);
               }

               return true;
            }
         }
         catch (Exception e)
         {
            return false;
         }

      }

      /// <summary>
      /// @Carlos - This function calls the procedure to create a new line in the database
      /// </summary>
      /// <param name="token">Authorization token</param>
      /// <param name="line">Object with the line definitions</param>
      /// <returns></returns>
      protected static bool AddLines(string token, DtoLine line)
      {
         try
         {
            using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_createLine";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_LineID", line.Id.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_Annotation", line.AnnotationId.ToString()));
               command.Parameters.Add(new SqlParameter("@LineIndex", line.LineIndex.ToString()));
               command.Parameters.Add(new SqlParameter("@X1", line.X1.ToString()));
               command.Parameters.Add(new SqlParameter("@X2", line.X2.ToString()));
               command.Parameters.Add(new SqlParameter("@Y1", line.Y1.ToString()));
               command.Parameters.Add(new SqlParameter("@Y2", line.Y2.ToString()));
               command.Parameters.Add(new SqlParameter("@Width", line.Width.ToString()));
               if (line.Color != null)
                  command.Parameters.Add(new SqlParameter("@Color", line.Color.ToString()));
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
               return true;
            }
         }
         catch (Exception e)
         {
            return false;
         }
      }

      /// <summary>
      ///  @Carlos - This function calls the procedure to create a new dot in the database
      /// </summary>
      /// <param name="token">Authorization token</param>
      /// <param name="dot">Object with the dot definitions</param>
      /// <returns></returns>
      protected static bool AddDots(string token, DtoDot dot)
      {
         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_createDots";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_DotsID", dot.Id.ToString()));
               command.Parameters.Add(new SqlParameter("@GUID_Annotation", dot.AnnotationId.ToString()));
               command.Parameters.Add(new SqlParameter("@X", dot.X.ToString()));
               command.Parameters.Add(new SqlParameter("@Y", dot.Y.ToString()));
               command.Parameters.Add(new SqlParameter("@DotIndex", dot.DotIndex.ToString()));
               if (dot.DotType != null)
                  command.Parameters.Add(new SqlParameter("@WaterType", dot.DotType.ToString()));
               if (dot.DotShape != null)
                  command.Parameters.Add(new SqlParameter("@Shape", dot.DotShape.ToString()));
               if (dot.Color != null)
                  command.Parameters.Add(new SqlParameter("@Color", dot.Color.ToString()));
               command.Parameters.Add(new SqlParameter("@Width", dot.Width.ToString()));
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
               return true;
            }
         }
         catch (Exception e)
         {
            return false;
         }
      }

      /// <summary>
      ///  @Carlos - This function calls the procedure to register the WebAPI activity in the database
      /// </summary>
      /// <param name="token">Authorization token</param>
      /// <returns></returns>
      public static bool RegisterWebAPILogin(string token)
      {
         try
         {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               SqlCommand command = new SqlCommand();
               command.Connection = connection;
               command.CommandText = "up_WebAPI_insertWebAPIAccess";
               command.CommandType = CommandType.StoredProcedure;
               command.Parameters.Add(new SqlParameter("@token", token.ToString()));
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
               return true;
            }
         }
         catch (Exception e)
         {
            return false;
         }
      }

   }
}