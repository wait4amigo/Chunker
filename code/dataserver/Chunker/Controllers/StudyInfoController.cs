using Chunker.DataAccess;
using Chunker.Filters;
using Chunker.Model;
using Chunker.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Chunker.Controllers
{
    public class StudyInfoController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        [AuthenticationFilter]
        public IHttpActionResult Post()
        {
            try
            {
                string body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                StudyInfoEnt ent = JsonConvert.DeserializeObject<StudyInfoEnt>(body);

                string errorInfo = string.Empty;

                if ((new StudyInfo()).Post(ent, ref errorInfo))
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
        public IHttpActionResult Get(string userGuid)
        {
            try
            {
                string errorInfo = string.Empty;

                StudyInfoEnt ent = (new StudyInfo()).Get(userGuid, ref errorInfo);
                if (ent == null)
                {
                    if (errorInfo == "Study info not exist")
                        return NotFound();

                    return BadRequest(errorInfo);
                }

                return Ok(JsonConvert.SerializeObject(ent));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}
