using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Chunker.Filters
{
    public class CompressAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(HttpActionContext ctx)
        {
            GZipEncodePage();
        }

        public static bool IsGZipSupported()
        {
            string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(AcceptEncoding) && (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate")))
                return true;

            return false;
        }

        public static void GZipEncodePage()
        {
            HttpResponse Response = HttpContext.Current.Response;

            if (IsGZipSupported())
            {
                string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

                if (AcceptEncoding.Contains("deflate"))
                {
                    Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "deflate");
                }
                else
                {
                    Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);

                    Response.AppendHeader("Content-Encoding", "gzip");
                }
            }

            // Allow proxy servers to cache encoded and unencoded versions separately
            Response.AppendHeader("Vary", "Content-Encoding");
        }
    }

    public class DeflateCompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
            Response.AppendHeader("Content-Encoding", "deflate");
        }
    }

    public class GZipCompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);

            Response.AppendHeader("Content-Encoding", "gzip");
        }
    }
}