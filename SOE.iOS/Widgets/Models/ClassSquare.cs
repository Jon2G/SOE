using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    public class ClassSquare
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("formattedTime")]
        public string FormattedTime { get; set; }
        [JsonProperty("group")]
        public string Group { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }

        public ClassSquare()
        {
        }
    }
}
