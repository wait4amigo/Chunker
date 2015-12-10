using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chunker.Model
{
    public enum LearnResult
    {
        CORRECT,
        ERROR,
        REPEATERROR
    }

    public class ProgressEnt
    {
        [JsonProperty(PropertyName = "user_guid")]
        public string UserGuid { get; set; }

        [JsonProperty(PropertyName = "course_id")]
        public int CourseId { get; set; }

        [JsonProperty(PropertyName = "block_id")]
        public int BlockId { get; set; }

        [JsonProperty(PropertyName = "detail_result")]
        public List<ChunkProgressEntity> DetailResult { get; set; }

        [JsonProperty(PropertyName = "update_time")]
        public DateTime UpdateTime { get; set; }
    }

    public class ChunkProgressEntity
    {
        [JsonProperty(PropertyName = "chunk_id")]
        public int ChunkId { get; set; }

        [JsonProperty(PropertyName = "study_time")]
        public DateTime StudyTime { get; set; }

        [JsonProperty(PropertyName = "result")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LearnResult Result { get; set; }
    }
}