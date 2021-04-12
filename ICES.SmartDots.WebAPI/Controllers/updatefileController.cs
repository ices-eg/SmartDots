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
    public class updatefileController : ApiController
    {
        // GET: api/updatefile
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/updatefile/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/updatefile
        public WebApiResult Post(string token, [FromBody] DtoFile file)
        {
            string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";

            var webApiResult = new WebApiResult();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "up_WebAPI_insertScale";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@token", token.ToString()));
                    command.Parameters.Add(new SqlParameter("@GUID_SmartImage", file.Id.ToString()));
                    command.Parameters.Add(new SqlParameter("@scale", file.Scale));
                    connection.Open();
                    SqlDataReader reader;
                    reader = command.ExecuteReader();
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                webApiResult.Result = true;
                return webApiResult;
            }
            catch (Exception e)
            {
                webApiResult.ErrorMessage = e.Message;
                return webApiResult;
            }
        }

        // PUT: api/updatefile/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/updatefile/5
        public void Delete(int id)
        {
        }
    }
}
