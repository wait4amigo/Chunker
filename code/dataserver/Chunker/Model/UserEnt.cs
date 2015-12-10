using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chunker.Model
{
    public class UserEnt
    {
        [JsonProperty(PropertyName = "user_name")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "user_guid")]
        public string UserGuid { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "create_time")]
        public string CreateTime { get; set; }

        [JsonProperty(PropertyName = "update_time")]
        public string UpdateTime { get; set; }
    }
}