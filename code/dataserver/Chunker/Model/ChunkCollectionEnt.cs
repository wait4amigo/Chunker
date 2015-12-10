using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chunker.Model
{
    public class ChunkCollectionEnt
    {
        [JsonProperty(PropertyName = "course_id")]
        public string CourseId { get; set; }

        [JsonProperty(PropertyName = "block_id")]
        public string BlockId { get; set; }

        [JsonProperty(PropertyName = "update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty(PropertyName = "download_url")]
        public string DownloadUrl { get; set; }
    }
}