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
    public class updateannotationsController : ApiController
    {
        // GET: api/updateannotations
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/updateannotations/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/updateannotations
        public WebApiResult Post(string token, [FromBody] List<DtoAnnotation> annotations)
        {
            
            var webApiResult = new WebApiResult();
            try
            {
                foreach (DtoAnnotation annotation in annotations)
                {
                    ///////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                    ///////////////// @Carlos: Needs to delete the previous annotations for that image \\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                    //////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\                    
                    using (SqlConnection connection = new SqlConnection(RunDBOperation.connectionString))
                    {                        
                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandText = "up_WebAPI_deleteAnnotation";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@token", token.ToString()));
                        command.Parameters.Add(new SqlParameter("@GUID_Annotation", annotation.Id.ToString()));
                        connection.Open();
                        SqlDataReader reader;
                        reader = command.ExecuteReader();
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    RunDBOperation.AddAnnotation(token, annotation);
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

        // PUT: api/updateannotations/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/updateannotations/5
        public void Delete(int id)
        {
        }
    }
}
