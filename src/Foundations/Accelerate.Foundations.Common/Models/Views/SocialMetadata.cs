using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    public class SocialMetadata
    {
        public string? Sitename { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }

        [JsonProperty("quote")]
        public string? Quote { get; set; }

        [JsonProperty("tags")]
        public string? Tags { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("creator")]
        public string? Creator { get; set; }
    }
}
