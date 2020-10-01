using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace WebAPI
{
   public static class WebApiConfig
   {
      public static void Register(HttpConfiguration config)
      {
         // Web API configuration and services

         // Web API routes
         config.MapHttpAttributeRoutes();

         config.Routes.MapHttpRoute(name: "Authenticate", routeTemplate: "Authenticate/{token}", defaults: new { controller = "Authenticate", token = RouteParameter.Optional });
         config.Routes.MapHttpRoute(name: "GetSettings", routeTemplate: "GetSettings", defaults: new { controller = "GetSettings" });
         config.Routes.MapHttpRoute(name: "GetReadabilityQualities", routeTemplate: "GetReadabilityQualities", defaults: new { controller = "GetReadabilityQualities" });
         config.Routes.MapHttpRoute(name: "getanalysesdynamic", routeTemplate: "getanalysesdynamic", defaults: new { controller = "getanalysesdynamic" });
         config.Routes.MapHttpRoute(name: "GetAnalysis", routeTemplate: "GetAnalysis", defaults: new { controller = "GetAnalysis" });
         config.Routes.MapHttpRoute(name: "GetFiles", routeTemplate: "GetFiles", defaults: new { controller = "GetFiles" });
         config.Routes.MapHttpRoute(name: "GetFile", routeTemplate: "GetFile", defaults: new { controller = "GetFile" });
         config.Routes.MapHttpRoute(name: "getfilewithsampleandannotations", routeTemplate: "getfilewithsampleandannotations", defaults: new { controller = "getfilewithsampleandannotations" });
         config.Routes.MapHttpRoute(name: "addannotation", routeTemplate: "addannotation", defaults: new { controller = "addannotation" });
         config.Routes.MapHttpRoute(name: "updatefile", routeTemplate: "updatefile", defaults: new { controller = "updatefile" });
         config.Routes.MapHttpRoute(name: "updateannotations", routeTemplate: "updateannotations", defaults: new { controller = "updateannotations" });
         config.Routes.MapHttpRoute(name: "deleteannotations", routeTemplate: "deleteannotations", defaults: new { controller = "deleteannotations" });
         config.Routes.MapHttpRoute(name: "GetGuestToken", routeTemplate: "GetGuestToken", defaults: new { controller = "GetGuestToken" });
         config.Routes.MapHttpRoute(name: "ToggleAnalysisUserProgress", routeTemplate: "ToggleAnalysisUserProgress", defaults: new { controller = "ToggleAnalysisUserProgress" });
         config.Routes.MapHttpRoute(name: "AddLarvaeAnnotation", routeTemplate: "AddLarvaeAnnotation", defaults: new { controller = "AddLarvaeAnnotation" });

         //            config.Routes.MapHttpRoute(name: "GetAnalysis", routeTemplate: "api/GetAnalysis", defaults: new { controller = "GetAnalysis" });

         config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

      }
   }
}
