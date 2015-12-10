using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Chunker.Model
{
    public class StudyInfoEnt
    {
        [JsonProperty(PropertyName = "user_guid")]
        public string UserGuid { get; set; }

        [JsonProperty(PropertyName = "current_course_id")]
        public int CurrentCourseId { get; set; }

        [JsonProperty(PropertyName = "current_block_id")]
        public int CurrentBlockId { get; set; }

        [JsonProperty(PropertyName = "achievement_level")]
        public int AchievementLevel { get; set; }

        [JsonProperty(PropertyName = "accuracy")]
        public float Accuracy { get; set; }

        [JsonProperty(PropertyName = "previous_review_time")]
        public DateTime PreviousReviewTime { get; set; }

        [JsonProperty(PropertyName = "previous_review_timelength")]
        public int PreviousReviewTimeLength { get; set; }
    }
}