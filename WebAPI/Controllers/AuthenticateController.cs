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
    public class AuthenticateController : ApiController
    {
        string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";


        // POST: api/Authenticate
        public WebApiResult Post([FromBody] DtoUserAuthentication userauthentication)
        {
            var webApiResult = new WebApiResult();

            string strUser = "";
            DtoUser dtoUser = new DtoUser();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    //GetLogboekTable
                    command.CommandText = "select [dbo].[GetIfValidToken](@token)";
                    command.Parameters.Add(new SqlParameter("@token", userauthentication.Username));
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
                    var results = new List<Dictionary<string, object>>();
                    var cols = new List<string>();
                    for (var i = 0; i < reader.FieldCount; i++) cols.Add(reader.GetName(i));
                    while (reader.Read())
                    {
                        strUser = reader[0].ToString();
                    }
                    if (string.IsNullOrEmpty(strUser))
                    {
                        throw new Exception("Token is invalid!", null);
                    }


                    RunDBOperation.RegisterWebAPILogin(userauthentication.Username);
                    dtoUser = new DtoUser() { Id = new Guid(), AccountName = strUser, Token = userauthentication.Username };
                    webApiResult.Result = dtoUser;

                }
            }
            catch (Exception e)
            {
                webApiResult.ErrorMessage = e.Message;
                return webApiResult;
            }
            return webApiResult;
        }
        
    }
}
