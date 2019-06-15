using System;
using Newtonsoft.Json;

namespace iosapp
{
    public class TaskItem
    {
        [JsonProperty("_id")]
        public string id { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("completed")]
        public Boolean completed { get; set; }

        public TaskItem(string text)
        {
            this.text = text;
            this.completed = false;
        }
    }

}
