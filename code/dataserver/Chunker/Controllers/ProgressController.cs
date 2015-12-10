using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Chunker.Model;
using Newtonsoft.Json;
using Chunker.DataAccess;
using Chunker.Utils;
using Chunker.Filters;

namespace Chunker.Controllers
{
    public class ProgressController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        [AuthenticationFilter]
        public IHttpActionResult Post()
        {
            try
            {
                string body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                ProgressEnt ent = JsonConvert.DeserializeObject<ProgressEnt>(body);

                string errorInfo = string.Empty;

                if ((new Progress()).Post(ent, ref errorInfo))
                    return Ok();

                return BadRequest(errorInfo);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());

                return BadRequest(ex.ToString());
            }
        }

        [AuthenticationFilter]
        public HttpResponseMessage Head(string userGuid, int courseId, int blockId)
        {
            try
            {
                string errorInfo = string.Empty;

                DateTime? dt = (new Progress()).GetUpdateTime(userGuid, courseId, blockId, ref errorInfo);
                if (dt == null)
                {
                    if (errorInfo == "Progress not exist")
                        return Request.CreateResponse(HttpStatusCode.NotFound, "");

                    return Request.CreateResponse(HttpStatusCode.BadRequest, errorInfo);
                }

                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK, "");
                resp.Headers.Add("user_guid", userGuid);
                resp.Headers.Add("course_id", courseId.ToString());
                resp.Headers.Add("block_id", blockId.ToString());

                DateTime dt1 = (DateTime)dt;
                resp.Headers.Add("block_update_time", dt1.ToString("yyyy-MM-ddThh:mm:ssZ"));

                return resp;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [AuthenticationFilter]
        public IHttpActionResult Get(string userGuid, int courseId, int blockId)
        {
            try
            {
                string errorInfo = string.Empty;

                ProgressEnt ent = (new Progress()).Get(userGuid, courseId, blockId, ref errorInfo);
                if (ent == null)
                {
                    if (errorInfo == "Progress not exist")
                        return NotFound();

                    return BadRequest(errorInfo);
                }

                return Ok(ent);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}
