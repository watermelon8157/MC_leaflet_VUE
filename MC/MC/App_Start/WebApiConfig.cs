using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RCS
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            // https://docs.microsoft.com/zh-tw/aspnet/web-api/overview/security/enabling-cross-origin-requests-in-web-api
            // 啟用CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GetAbgListApi",
                routeTemplate: "api/{controller}/{action}/{chart_no}",
                defaults: new { action = RouteParameter.Optional, chart_no = RouteParameter.Optional }
            );

            //參考網址http://huan-lin.blogspot.com/2013/01/aspnet-web-api-and-json.html 
            //預設直接傳JsonData
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
