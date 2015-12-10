using Chunker.Model;
using Chunker.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Chunker.DataAccess
{
    public class StudyInfo : DBBase
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");
        
        public bool Post(StudyInfoEnt ent, ref string errorInfo)
        {
            _log.Info("Save study info for user " + ent.UserGuid);

            StudyInfoEnt oldEnt = Get(ent.UserGuid, ref errorInfo);

            string sql = string.Empty;
            if (oldEnt != null)
                sql = string.Format("UPDATE study_info SET current_course_id={0}, current_block_id={1}, achievement_level={2}, " +
                    "accuracy={3}, previous_review_time='{4}', previous_review_timelength={5} WHERE user_guid='{6}'"
                    , ent.CurrentCourseId, ent.CurrentBlockId, ent.AchievementLevel, ent.Accuracy, ent.PreviousReviewTime.ToString("yyyy-MM-ddThh:mm:ssZ")
                    , ent.PreviousReviewTimeLength, ent.UserGuid);
            else
                sql = string.Format("INSERT INTO study_info (user_guid, current_course_id, current_block_id, achievement_level, " +
                    "accuracy, previous_review_time, previous_review_timelength) VALUES ('{0}', {1}, {2}, {3}, {4}, '{5}', {6})"
                    , ent.UserGuid, ent.CurrentCourseId, ent.CurrentBlockId, ent.AchievementLevel, ent.Accuracy
                    , ent.PreviousReviewTime.ToString("yyyy-MM-ddThh:mm:ssZ"), ent.PreviousReviewTimeLength); 

            int cnt = ExecuteSql(sql);
            if (cnt == 0)
                throw new Exception("Failed to insert/update database");

            return true;
        }

        public StudyInfoEnt Get(string userGuid, ref string errorInfo)
        {
            _log.Debug("Get study info for user " + userGuid);

            string sql = string.Format("SELECT * FROM study_info WHERE user_guid='{0}'", userGuid);
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query study info");

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Study info not exist";
                return null;
            }

            StudyInfoEnt ent = new StudyInfoEnt();

            ent.UserGuid = userGuid;
            ent.CurrentCourseId = int.Parse(ds.Tables[0].Rows[0]["current_course_id"].ToString());
            ent.CurrentBlockId = int.Parse(ds.Tables[0].Rows[0]["current_block_id"].ToString());
            ent.AchievementLevel = int.Parse(ds.Tables[0].Rows[0]["achievement_level"].ToString());
            ent.Accuracy = float.Parse(ds.Tables[0].Rows[0]["accuracy"].ToString());
            ent.PreviousReviewTime = DateTime.Parse(ds.Tables[0].Rows[0]["previous_review_time"].ToString());
            ent.PreviousReviewTimeLength = int.Parse(ds.Tables[0].Rows[0]["previous_review_timelength"].ToString());

            return ent;
        }
    }
}