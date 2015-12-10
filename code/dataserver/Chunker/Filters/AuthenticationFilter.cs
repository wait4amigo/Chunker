using Chunker.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Chunker.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute, IActionFilter
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        public AuthenticationFilter()
        {
        }

        public override void OnActionExecuting(HttpActionContext ctx)
        {
            if (ctx.Request.Headers.Authorization == null)
            {
                ctx.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
                ctx.Response.Content = new StringContent("No authorization info in request header");
            }
            else
            {
                string[] parts = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(ctx.Request.Headers.Authorization.Parameter)).Split(':');
                try
                {
                    if (parts.Length != 2)
                    {
                        ctx.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Forbidden);
                        ctx.Response.Content = new StringContent("No authorization info in request header");
                    }
                    else
                    {
                        string errorInfo = string.Empty;
                        if (!(new User()).Login(parts[0], parts[1], ref errorInfo))
                        {
                            if (errorInfo == "User not exist")
                            {
                                ctx.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.NotFound);
                                ctx.Response.Content = new StringContent(errorInfo);
                            }
                            else
                            {
                                ctx.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
                                ctx.Response.Content = new StringContent(errorInfo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    ctx.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
                    ctx.Response.Content = new StringContent(ex.ToString());
                }
            }
        }
    }
}