using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace WebInterface.App_Code
{
   public class RunDBOperations
   {

      public static bool runDBSmartDotsSQL(string strSQL)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               cn.Open();
               using (SqlCommand cmd = new SqlCommand(strSQL, cn))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  return true;
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }
         return false;
      }
      public static void validateSession()
      {
         if (HttpContext.Current.Session["user"] == null)
         {
            HttpContext.Current.Session["CountryCoordinator"] = "0";
            HttpContext.Current.Session["SmartAdministrator"] = "0";
            HttpContext.Current.Session["tblCodeID_Country"] = "0";
            HttpContext.Current.Session["Country"] = "notSpecified";
            HttpContext.Current.Session["user"] = "notvalidated";
            HttpContext.Current.Session["urlCheck"] = "";
            HttpContext.Current.Session["message"] = "";
         }
         return;
      }
      /// <summary>
      /// This will return a dataset based on the SQL query run in the DB
      /// </summary>
      /// <param name="strSQL"></param>
      /// <returns></returns>
      public static DataSet getDataset(string strSQL)
      {
         try
         {

            String ConnectionString = ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               DataSet ds = new DataSet();
               conn.Open();
               cmd.Connection = conn;
               cmd.CommandType = CommandType.Text;
               cmd.CommandText = strSQL;
               cmd.CommandTimeout = 0;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);
               return ds;
            }
         }
         catch (Exception e) { return null; }
      }

      /// <summary>
      /// This function will help to return specific results where only one row is needed
      /// </summary>
      /// <param name="strSQL"></param>
      /// <returns></returns>
      public static string executeScalarQuery(string strSQL)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               cn.Open();
               cmd.Connection = cn;
               cmd.CommandText = strSQL;
               string strResult = (string)cmd.ExecuteScalar();
               return strResult;
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return null;
      }

      public static bool boolExecuteScalarQuery(string strSQL)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlCommand cmd = new SqlCommand();
               cn.Open();
               cmd.Connection = cn;
               cmd.CommandText = strSQL;
               bool blnResult = (bool)cmd.ExecuteScalar();

               return blnResult;
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return false;
      }

      public static int intExecuteScalarQuery(string strSQL)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlCommand cmd = new SqlCommand();
               cn.Open();
               cmd.Connection = cn;
               cmd.CommandText = strSQL;
               Int32 count = (Int32)cmd.ExecuteScalar();

               return count;
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return 0;
      }


      /// <summary>
      /// This function will return if the user is a country coordinator or now and setup the other session variables
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static bool checkIfUserIsAdministrator(string strUser)
      {
         if (strUser != null)
         {
            //lblMessage.Visible = false;
            try
            {
               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection cn = new SqlConnection(ConnectionString))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  SqlCommand cmd = new SqlCommand();
                  DataSet ds = new DataSet();
                  cn.Open();
                  cmd.Connection = cn;
                  cmd.CommandText = "SELECT     dbo.tblDoYouHaveAccess.SmartUser, dbo.tblDoYouHaveAccess.Name, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, dbo.tblDoYouHaveAccess.NumLogins, dbo.tblDoYouHaveAccess.LastAccess, dbo.tblDoYouHaveAccess.isCountryCoordinator, dbo.tblDoYouHaveAccess.isSMARTAdministrator, upper(left(dbo.tblCode.Description,1)) + lower(right(dbo.tblCode.Description,len(dbo.tblCode.Description)-1)) as Country FROM  dbo.tblDoYouHaveAccess INNER JOIN dbo.tblCode ON dbo.tblDoYouHaveAccess.tblCodeID_Country = dbo.tblCode.tblCodeID where SmartUser = '" + strUser + "'";
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     HttpContext.Current.Session["CountryCoordinator"] = dr["isCountryCoordinator"].ToString();
                     HttpContext.Current.Session["SmartAdministrator"] = dr["isSMARTAdministrator"].ToString();
                     HttpContext.Current.Session["tblCodeID_Country"] = dr["tblCodeID_Country"].ToString();
                     HttpContext.Current.Session["Country"] = dr["Country"].ToString();
                     if (HttpContext.Current.Session["SmartAdministrator"].ToString().ToLower() == "true")
                     {
                        return true;
                     }
                     else
                     {
                        return false;
                     }
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
            }
         }
         return false;
      }

      /// <summary>
      /// This function will return if the user is a country coordinator or now and setup the other session variables
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static bool checkIfUserIsCountryCoordinator(string strUser)
      {
         if (strUser != null)
         {
            //lblMessage.Visible = false;
            try
            {
               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection cn = new SqlConnection(ConnectionString))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  SqlCommand cmd = new SqlCommand();
                  DataSet ds = new DataSet();
                  cn.Open();
                  cmd.Connection = cn;
                  cmd.CommandText = "SELECT     dbo.tblDoYouHaveAccess.SmartUser, dbo.tblDoYouHaveAccess.Name, dbo.tblDoYouHaveAccess.Email, dbo.tblDoYouHaveAccess.tblCodeID_Country, dbo.tblDoYouHaveAccess.NumLogins, dbo.tblDoYouHaveAccess.LastAccess, dbo.tblDoYouHaveAccess.isCountryCoordinator, dbo.tblDoYouHaveAccess.isSMARTAdministrator, upper(left(dbo.tblCode.Description,1)) + lower(right(dbo.tblCode.Description,len(dbo.tblCode.Description)-1)) as Country FROM  dbo.tblDoYouHaveAccess INNER JOIN dbo.tblCode ON dbo.tblDoYouHaveAccess.tblCodeID_Country = dbo.tblCode.tblCodeID where SmartUser = '" + strUser + "'";
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     HttpContext.Current.Session["CountryCoordinator"] = dr["isCountryCoordinator"].ToString();
                     HttpContext.Current.Session["SmartAdministrator"] = dr["isSMARTAdministrator"].ToString();
                     HttpContext.Current.Session["tblCodeID_Country"] = dr["tblCodeID_Country"].ToString();
                     HttpContext.Current.Session["Country"] = dr["Country"].ToString();
                     if (HttpContext.Current.Session["CountryCoordinator"].ToString().ToLower() == "true")
                     {
                        return true;
                     }
                     else
                     {
                        return false;
                     }
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
            }
         }
         return false;
      }

      /// <summary>
      /// This function will return if the user is a country coordinator or now and setup the other session variables
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static bool checkIfUserIsParticipatesOrManagesEvent(string strUser, string strEventID)
      {
         if (strUser != null)
         {
            //lblMessage.Visible = false;
            try
            {
               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection cn = new SqlConnection(ConnectionString))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  SqlCommand cmd = new SqlCommand();
                  DataSet ds = new DataSet();
                  cn.Open();
                  cmd.Connection = cn;
                  cmd.CommandText = "SELECT     [dbo].[checkIfUserIsParticipatesOrManagesEvent]( '" + strUser + "'," + strEventID + ") as No";
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     if (int.Parse(dr["No"].ToString()) == 0)
                     {
                        return false;
                     }
                     else
                     {
                        return true;
                     }
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
            }
         }
         return false;
      }
      /// <summary>
      /// This function will return true if the user is an event manager
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static bool checkIfEventIsClosed(string strEventID)
      {
         try
         {
            String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
               SqlDataAdapter da = new SqlDataAdapter();
               SqlCommand cmd = new SqlCommand();
               DataSet ds = new DataSet();
               cn.Open();
               cmd.Connection = cn;
               cmd.CommandText = "select closed from tblEvent where tblEventID = " + strEventID ;
               cmd.CommandType = CommandType.Text;
               cmd.ExecuteNonQuery();
               da.SelectCommand = cmd;
               da.Fill(ds);
               ///////////////////////////////////////////////////////////////////
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  if (dr["closed"].ToString() == "True")
                  {
                     return true;
                  }
                  else
                  {
                     return false;
                  }
               }
            }
         }
         catch (Exception exp)
         {
            String a = exp.Message.ToString();
         }

         return false;
      }

      /// <summary>
      /// This function will return true if the user is an event manager
      /// </summary>
      /// <param name="strUser"></param>
      /// <returns></returns>
      public static bool checkIfUserIsEventManager(string strUser, string strEventID)
      {
         if (strUser != null)
         {
            try
            {
               String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartDotsCS"].ConnectionString;
               using (SqlConnection cn = new SqlConnection(ConnectionString))
               {
                  SqlDataAdapter da = new SqlDataAdapter();
                  SqlCommand cmd = new SqlCommand();
                  DataSet ds = new DataSet();
                  cn.Open();
                  cmd.Connection = cn;
                  cmd.CommandText = "SELECT     [dbo].[GetIfEventManager]( '" + strUser + "'," + strEventID + ") as No";
                  cmd.CommandType = CommandType.Text;
                  cmd.ExecuteNonQuery();
                  da.SelectCommand = cmd;
                  da.Fill(ds);
                  ///////////////////////////////////////////////////////////////////
                  foreach (DataRow dr in ds.Tables[0].Rows)
                  {
                     if (int.Parse(dr["No"].ToString()) == 0)
                     {
                        return false;
                     }
                     else
                     {
                        return true;
                     }
                  }
               }
            }
            catch (Exception exp)
            {
               String a = exp.Message.ToString();
            }
         }
         return false;
      }

      /// <summary>
      /// This is to check if an event is open or closed
      /// </summary>
      /// <param name="strEventID">The event id</param>
      /// <returns></returns>
      public static bool eventIsClosed(string strEventID)
      {
         int intOpen = (int)intExecuteScalarQuery("SELECT cast(Closed as int) as closed FROM[dbo].[tblEvent] where Published = 0 and tblEventID = " + strEventID);
         if (intOpen > 0)
         {
            return true;
         }
         return false;
      }
   }
}