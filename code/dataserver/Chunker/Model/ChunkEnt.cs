using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chunker.Model
{
    public class ChunkEnt
    {
        [JsonProperty(PropertyName = "course_id")]
        public int CourseId { get; set; }

        [JsonProperty(PropertyName = "block_id")]
        public int BlockId { get; set; }

        [JsonProperty(PropertyName = "chunk_id")]
        public int ChunkId { get; set; }

        [JsonProperty(PropertyName = "target_word")]
        public string TargetWord { get; set; }

        [JsonProperty(PropertyName = "form")]
        public string Form { get; set; }

        [JsonProperty(PropertyName = "chunk_en")]
        public string ChunkEn { get; set; }

        [JsonProperty(PropertyName = "chunk_chs")]
        public string ChunkChs { get; set; }

        [JsonProperty(PropertyName = "word_by_word")]
        public string WordByWord { get; set; }

        [JsonProperty(PropertyName = "pron_url")]
        public string PronUrl { get; set; }

        [JsonProperty(PropertyName = "sketch_url")]
        public string SketchUrl { get; set; }

        [JsonProperty(PropertyName = "create_time")]
        public DateTime CreateTime { get; set; }

        [JsonProperty(PropertyName = "update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty(PropertyName = "editor")]
        public string Editor { get; set; }

        [JsonProperty(PropertyName = "translator")]
        public string Translator { get; set; }

        [JsonProperty(PropertyName = "recorder")]
        public string Recorder { get; set; }
    }
}