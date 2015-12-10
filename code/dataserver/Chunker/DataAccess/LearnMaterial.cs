using System;
using Chunker.Model;
using System.Data;
using log4net;
using System.Reflection;
using Chunker.Utils;

namespace Chunker.DataAccess
{
    public class LearnMaterial : DBBase
    {
        ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public LearnMaterial()
        {

        }

        public ChunkCollectionEnt Get(string courseId, string blockId, ref string errorInfo)
        {
            _log.Debug("Get material info for course " + courseId + " block  " + blockId);

            string sql = string.Format("select update_time, download_url from chunk_collection where course_id='{0}' and block_id='{1}'"
                , courseId, blockId);
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query learn material data");

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Course or block not exist";
                return null;
            }

            ChunkCollectionEnt ent = new ChunkCollectionEnt();

            ent.CourseId = courseId;
            ent.BlockId = blockId;
            ent.UpdateTime = DateTime.Parse(ds.Tables[0].Rows[0][0].ToString() + "Z");
            ent.DownloadUrl = ds.Tables[0].Rows[0][1].ToString();

            return ent;
        }
    }
}