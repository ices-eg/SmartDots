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
    public class GetFileController : ApiController
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
        // GET: api/GetFile/5 this is a dummy controler!!!
        public WebApiResult Get(string token, Guid id, bool withAnnotations, bool withSample)
        {
            var webApiResult = new WebApiResult();


            return webApiResult;
        }
        
    }
}
