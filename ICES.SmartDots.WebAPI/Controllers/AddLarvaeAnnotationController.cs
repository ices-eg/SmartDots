using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.App_Code;

namespace WebAPI.Controllers
{
    public class AddLarvaeAnnotationController : ApiController
    {


         public IEnumerable<string> Get()
         {
            return new string[] { "value1", "value2" };
         }

      // POST: api/addannotation
      public WebApiResult Post(string token, [FromBody] DtoAnnotation annotation)
      {
         var webApiResult = new WebApiResult();
         webApiResult.Result = RunDBOperation.AddLarvaeAnnotation(token, annotation);

         return webApiResult;
      }
   }

   
}
