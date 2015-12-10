using Chunker.DataAccess;
using Chunker.Filters;
using Chunker.Model;
using Chunker.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chunker.Controllers
{
    public class ChunkController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        [Compress]
        public IHttpActionResult Get(int courseId, int blockId)
        {
            try
            {
                int fromChunkId = int.Parse(Util.GetQueryString(Request, "from"));
                int toChunkId = int.Parse(Util.GetQueryString(Request, "to"));

                Chunk chunk = new Chunk();
                string errorInfo = string.Empty;
                List<ChunkEnt> ents = chunk.Get(courseId, blockId, fromChunkId, toChunkId, ref errorInfo);
                if (ents == null || ents.Count == 0)
                {
                    if (errorInfo == "Chunk info not exis")
                        return NotFound();

                    return BadRequest(errorInfo);
                }

                return Ok(ents);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}
