using System.Web.Http;
using Newtonsoft.Json;
using Chunker.DataAccess;
using Chunker.Model;
using System;
using Chunker.Filters;
using System.Web.Hosting;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Chunker.Utils;

namespace Chunker.Controllers
{
    public class MaterialController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        [ActionName("dictionary")]
        public HttpResponseMessage Head()
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "value");

                Misc misc = new Misc();
                string errorInfo = string.Empty;
                string updateTime = misc.Get("dictionary_update_time", ref errorInfo);
                if (string.IsNullOrEmpty(updateTime))
                {
                    if (errorInfo == "Not exist")
                        return Request.CreateResponse(HttpStatusCode.NotFound, "");

                    return Request.CreateResponse(HttpStatusCode.BadRequest, errorInfo);
                }

                HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK, "");
                resp.Headers.Add("dictionary_update_time", updateTime);

                return resp;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }

        [ActionName("dictionary")]
        [Compress]
        public IHttpActionResult Get()
        {
            try
            {
                string appPath = HostingEnvironment.ApplicationPhysicalPath;
                string filePath = Path.Combine(appPath, @"Data\\en-chs-dictionary.txt");

                string dicText = File.ReadAllText(filePath, Encoding.Default);

                Misc misc = new Misc();
                string errorInfo = string.Empty;
                string updateTime = misc.Get("dictionary_update_time", ref errorInfo);
                if (string .IsNullOrEmpty(updateTime))
                {
                    if (errorInfo == "Not exist")
                        return NotFound();

                    return BadRequest(errorInfo);
                }

                return Ok(new { UpdateTime = updateTime, Content = dicText });
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        public IHttpActionResult Get(string courseId, string blockId)
        {
            _log.Info("Checking the material of course " + courseId + " and block " + blockId);
            
            try
            {
                string errorInfo = string.Empty;
                ChunkCollectionEnt ent = (new LearnMaterial()).Get(courseId, blockId, ref errorInfo);
                if (ent == null)
                {
                    if (errorInfo == "Course or block not exist")
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

        [ActionName("dictionary")]
        public IHttpActionResult Post()
        {
            try
            {
                string errorInfo = string.Empty;
                Chunk chunk = new Chunk();
                if (!chunk.Delete())
                    return BadRequest("Failed to remove old chunks");

                if (chunk.Add(GetChunks()))
                    return BadRequest("Failed to add new chunks");

                return Ok("Chunks created successfully");
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        private List<ChunkEnt> GetChunks()
        {
            List<ChunkEnt> ents = new List<ChunkEnt>();

            try
            {
                string appPath = HostingEnvironment.ApplicationPhysicalPath;
                string filePath = Path.Combine(appPath, @"Data\\chunks.txt");

                List<string> chunkLines = new List<string>(File.ReadAllLines(filePath, Encoding.Default));

                foreach (string line in chunkLines)
                {
                    string[] arr = line.Split('\t');
                    if (arr.Length != 11)
                    {
                        _log.Error("Chunk " + line + " has wrong format");
                        continue;
                    }

                    ChunkEnt ent = new ChunkEnt();
                    ent.CourseId = int.Parse(arr[0]);
                    ent.BlockId = int.Parse(arr[1]);
                    ent.ChunkId = int.Parse(arr[2]);
                    ent.TargetWord = arr[3];
                    ent.Form = arr[4];
                    ent.ChunkEn = arr[5];
                    ent.ChunkChs = arr[6];
                    ent.WordByWord = arr[7];
                    ent.PronUrl = "https://chunk.oss-cn-shanghai.aliyuncs.com/" + arr[0] + "/" + arr[1] + "/" + arr[2] + ".mp3";
                    ent.Editor = arr[8];
                    ent.Translator = arr[9];
                    ent.Recorder = arr[10];

                    ents.Add(ent);
                }

                return ents;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return null;
            }
        }
    }
}
