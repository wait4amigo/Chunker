using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Chunker.Model;
using Newtonsoft.Json;
using Chunker.Utils;

namespace Chunker.DataAccess
{
    public class Progress : DBBase
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        public bool Post(ProgressEnt ent, ref string errorInfo)
        {
            _log.Debug("Saving progress for " + ent.UserGuid);

            List<ChunkProgressEntity> words = ent.DetailResult;

            ProgressEnt oldEnt = Get(ent.UserGuid, ent.CourseId, ent.BlockId, ref errorInfo);
            if (oldEnt != null)
                words.AddRange(oldEnt.DetailResult.FindAll(r => !words.Exists(w => w.ChunkId == r.ChunkId)));

            string sql = string.Empty;
            if (oldEnt != null)
                sql = string.Format("UPDATE detail_progress SET detail_result='{0}' and update_time='{1}' WHERE user_guid='{2}' and course_id={3} and block_id={4}"
                    , JsonConvert.SerializeObject(words), DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"), ent.UserGuid, ent.CourseId, ent.BlockId);
            else
                sql = string.Format("INSERT INTO detail_progress (user_guid, course_id, block_id, detail_result) VALUES ('{0}', {1}, {2}, '{3}')"
                    , ent.UserGuid, ent.CourseId, ent.BlockId, JsonConvert.SerializeObject(words));

            int cnt = ExecuteSql(sql);
            if (cnt == 0)
            {
                errorInfo = "Failed to insert/update database";
                return false;
            }

            return true;
        }

        public DateTime? GetUpdateTime(string userGuid, int courseId, int blockId, ref string errorInfo)
        {
            _log.Debug("Get progress updated time for " + userGuid);

            string sql = string.Format("SELECT update_time FROM detail_progress WHERE user_guid='{0}' and course_id={1} and block_id={2}"
                , userGuid, courseId, blockId);
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query user's data");

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Progress not exist";
                return null;
            }

            DateTime updateTime = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());

            return updateTime;
        }

        public ProgressEnt Get(string userGuid, int courseId, int blockId, ref string errorInfo)
        {
            _log.Debug("Get progress for " + userGuid);

            string sql = string.Format("SELECT update_time, detail_result FROM detail_progress WHERE user_guid='{0}' and course_id={1} and block_id={2}"
                , userGuid, courseId, blockId);
            DataSet ds = Query(sql);
            if (ds == null)
            {
                errorInfo = "Failed to query user's data";
                return null;
            }

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Progress not exist";
                return null;
            }

            ProgressEnt ent = new ProgressEnt();

            ent.UserGuid = userGuid;
            ent.CourseId = courseId;
            ent.BlockId = blockId;
            ent.UpdateTime = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
            ent.DetailResult = JsonConvert.DeserializeObject<List<ChunkProgressEntity>>(ds.Tables[0].Rows[0][1].ToString());

            return ent;
        }
    }
}