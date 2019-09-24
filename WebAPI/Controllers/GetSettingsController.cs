using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.App_Code;

namespace WebAPI.Controllers
{
    public class GetSettingsController : ApiController
    {
        // GET: api/GetSettings
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetSettings/5
        public WebApiResult Get(string token)
        {
            var webApiResult = new WebApiResult();
            try
            {
                var settings = new DtoSmartdotsSettings()
                {
                    CanAttachDetachSample = false,
                    CanBrowseFolder = false,
                    UseSampleStatus = true,
                    CanApproveAnnotation = true,
                    RequireAQ1ForApproval = false,
                    CanMarkEventAsCompleted = true,
                    AutoMeasureScale = false
                };
                webApiResult.Result = settings;
                return webApiResult;

            }
            catch (Exception e)
            {
                webApiResult.ErrorMessage = e.Message;
                return webApiResult;
            }
        }

    }
}
