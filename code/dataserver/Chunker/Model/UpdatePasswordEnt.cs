using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chunker.Model
{
    public class UpdatePasswordEnt
    {
        [JsonProperty(PropertyName = "old_password")]
        public string OldPassword { get; set; }

        [JsonProperty(PropertyName = "new_password")]
        public string NewPassword { get; set; }
    }
}