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
    public class addannotationController : ApiController
    {
        string connectionString = "Data Source=sql06.ices.local;Initial Catalog=smartdots;Integrated Security=True;";
        // GET: api/addannotation
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/addannotation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/addannotation
        public WebApiResult Post(string token, [FromBody] DtoAnnotation annotation)
        {
            var webApiResult = new WebApiResult();
            webApiResult.Result = RunDBOperation.AddAnnotation(token, annotation);

            return webApiResult;
        }

        

    }
}
