using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.App_Code;

namespace WebAPI.Controllers
{
   public class GetReadabilityQualitiesController : ApiController
   {

      public WebApiResult Get(string token)
      {
         var webApiResult = new WebApiResult();
         try
         {
            List<object> sdQualities = new List<object>();
            Quality qualityAQ1 = new Quality();
            qualityAQ1.Code = "AQ1";
            qualityAQ1.Description = "Easy Age";
            qualityAQ1.HexColor = "#00b300";
            var sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ1, typeof(DtoReadabilityQuality));
            sdQuality.Id = Guid.Parse("942ABE62-0152-4852-80F5-A161BD46147E");
            sdQuality.Color = "#00b300";
            sdQualities.Add(sdQuality);


            Quality qualityAQ2 = new Quality();
            qualityAQ2.Code = "AQ2";
            qualityAQ2.Description = "Difficult to age with age with acceptable precision";
            qualityAQ2.HexColor = "#0000b3";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ2, typeof(DtoReadabilityQuality));
            sdQuality.Id = Guid.Parse("C32E0EC9-15C6-47C0-8DC5-EFA587BE82AF");
            sdQuality.Color = "#0000b3";
            sdQualities.Add(sdQuality);

            Quality qualityAQ3 = new Quality();
            qualityAQ3.Code = "AQ3";
            qualityAQ3.Description = "Unreadable or very difficult to age with acceptable precision";
            qualityAQ3.HexColor = "#b30000";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ3, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#b30000";
            sdQuality.Id = Guid.Parse("4D7FBD36-32C0-4872-9568-6C6DBDF4E384");
            sdQualities.Add(sdQuality);


            Quality qualityAQ3_noAge = new Quality();
            qualityAQ3_noAge.Code = "AQ3-NoAge";
            qualityAQ3_noAge.Description = "Impossible to age";
            qualityAQ3_noAge.HexColor = "#f30000";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ3_noAge, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#f30000";
            sdQuality.Id = Guid.Parse("FA0651BE-0FE6-4E34-ABF2-2A6C9FE3F410");
            sdQualities.Add(sdQuality);


            webApiResult.Result = sdQualities;
            return webApiResult;
         }
         catch (Exception e)
         {
            webApiResult.ErrorMessage = e.Message;
            return webApiResult;
         }
      }

      // POST: api/GetReadabilityQualities
      public WebApiResult Post([FromBody]string value)
      {
         var webApiResult = new WebApiResult();
         try
         {
            List<object> sdQualities = new List<object>();
            Quality qualityAQ1 = new Quality();
            qualityAQ1.Code = "AQ1";
            qualityAQ1.Description = "Code(1) - Rings can be counted with certainty";
            qualityAQ1.HexColor = "#00b300";
            var sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ1, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#00b300";
            sdQuality.Id = Guid.Parse("942ABE62-0152-4852-80F5-A161BD46147E");
            sdQualities.Add(sdQuality);


            Quality qualityAQ2 = new Quality();
            qualityAQ2.Code = "AQ2";
            qualityAQ2.Description = "Code(2) - Rings can be counted, but with difficulty and some doubt";
            qualityAQ2.HexColor = "#0000b3";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ2, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#0000b3";
            sdQuality.Id = Guid.Parse("C32E0EC9-15C6-47C0-8DC5-EFA587BE82AF");
            sdQualities.Add(sdQuality);

            Quality qualityAQ3 = new Quality();
            qualityAQ3.Code = "AQ3";
            qualityAQ3.Description = "Code (3) - Rings can not be counted, the otolith is unreadable";
            qualityAQ3.HexColor = "#b30000";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ3, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#b30000";
            sdQuality.Id = Guid.Parse("4D7FBD36-32C0-4872-9568-6C6DBDF4E384");
            sdQualities.Add(sdQuality);

            Quality qualityAQ3_noAge = new Quality();
            qualityAQ3_noAge.Code = "AQ3 - noage";
            qualityAQ3_noAge.Description = "Code(-9) - NA";
            qualityAQ3_noAge.HexColor = "#f30000";
            sdQuality = (DtoReadabilityQuality)Helper.ConvertType(qualityAQ3_noAge, typeof(DtoReadabilityQuality));
            sdQuality.Color = "#f30000";
            sdQuality.Id = Guid.Parse("FA0651BE-0FE6-4E34-ABF2-2A6C9FE3F410");
            sdQualities.Add(sdQuality);


            webApiResult.Result = sdQualities;
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