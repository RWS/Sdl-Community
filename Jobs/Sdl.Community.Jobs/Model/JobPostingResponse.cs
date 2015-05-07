using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class JobPostingResponse
    {
        [JsonProperty(PropertyName = "_links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<JobPosting> JobPostings { get; set; }
    }
}
