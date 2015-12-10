using Chunker.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Chunker.DataAccess
{
    public class Chunk : DBBase
    {
        ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Chunk()
        {

        }

        public bool Delete()
        {
            string sql = "DELETE FROM Chunk";
            ExecuteSql(sql);

            _log.Info("All chunks deleted");

            return true;
        }

        public bool Add(List<ChunkEnt> ents)
        {
            if (ents == null || !ents.Any())
                return false;

            foreach (ChunkEnt ent in ents)
            {
                string curTime = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ");
                string sql = string.Format("INSERT INTO Chunk (course_id, block_id, chunk_id, target_word, form, chunk_en, chunk_chs, word_by_word, pron_url, sketch_url, create_time, update_time, editor, translator, recorder) "
                    + " VALUES ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}')"
                    , ent.CourseId, ent.BlockId, ent.ChunkId, ent.TargetWord, ent.Form, ent.ChunkEn, ent.ChunkChs, ent.WordByWord
                    , ent.PronUrl, ent.SketchUrl, curTime, curTime, ent.Editor, ent.Translator, ent.Recorder);
                int cnt = ExecuteSql(sql);
                if (cnt == 0)
                    throw new Exception("Failed to update database");
            }

            _log.Info("Chunks created successfully");

            return true;
        }

        public List<ChunkEnt> Get(int courseId, int blockId, int fromChunkId, int toChunkId, ref string errorInfo)
        {
            _log.Debug(string.Format("Get chunk info for course {0}, block {1}, from {2}, to {3} ", courseId, blockId, fromChunkId, toChunkId));

            string sql = string.Format("SELECT * FROM chunk WHERE course_id={0} and block_id={1} and chunk_id>={2} and chunk_id<={3}"
                , courseId, blockId, fromChunkId, toChunkId);

            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query chunk info");

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Chunk info not exist";
                return null;
            }

            List<ChunkEnt> ents = new List<ChunkEnt>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ChunkEnt ent = new ChunkEnt();

                ent.CourseId = int.Parse(row["course_id"].ToString());
                ent.BlockId = int.Parse(row["block_id"].ToString());
                ent.ChunkId = int.Parse(row["chunk_id"].ToString());
                ent.TargetWord = row["target_word"].ToString();
                ent.Form = row["form"].ToString();
                ent.ChunkEn = row["chunk_en"].ToString();
                ent.ChunkChs = row["chunk_chs"].ToString();
                ent.WordByWord = row["word_by_word"].ToString();
                ent.PronUrl = row["pron_url"].ToString();
                ent.SketchUrl = row["sketch_url"].ToString();
                ent.CreateTime = DateTime.Parse(row["create_time"].ToString());
                ent.UpdateTime = DateTime.Parse(row["update_time"].ToString());
                ent.Editor = row["editor"].ToString();
                ent.Translator = row["translator"].ToString();
                ent.Recorder = row["recorder"].ToString();

                ents.Add(ent);
            }

            return ents;
        }
    }
}