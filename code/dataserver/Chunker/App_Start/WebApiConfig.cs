using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Chunker
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "HealthApi",
                routeTemplate: "api/v1/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "UserApi",
                routeTemplate: "api/v1/{controller}/{userGuid}",
                defaults: new { userGuid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "MaterialApi",
                routeTemplate: "api/v1/material/{courseId}/{blockId}",
                defaults: new
                {
                    controller = "Material",
                    action = "Get",
                    courseId = RouteParameter.Optional,
                    blockId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/v1/material/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "ChunkApi",
                routeTemplate: "api/v1/chunk/{courseId}/{blockId}",
                defaults: new
                {
                    controller = "Chunk",
                    action = "Get",
                    courseId = RouteParameter.Optional,
                    blockId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "UserApi_UpdatePassword",
                routeTemplate: "api/v1/{controller}/{userGuid}/{verifyCode}",
                defaults: new { userGuid = RouteParameter.Optional, verifyCode = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ProgressApi",
                routeTemplate: "api/v1/{controller}/{userGuid}/{courseId}/{blockId}",
                defaults: new { userGuid = RouteParameter.Optional, courseId = RouteParameter.Optional, blockId = RouteParameter.Optional }
            );
        }
    }
}
