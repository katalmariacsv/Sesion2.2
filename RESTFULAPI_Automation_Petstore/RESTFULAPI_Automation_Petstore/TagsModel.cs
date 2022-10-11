using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTFULAPI_Automation_Petstore
{
    public class TagsModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
