using Foundation;
using Newtonsoft.Json;
using SOEWeb.Shared;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Preserve]
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
        public ClassSquare(Subject subject,int index,string formattedTime)
        {
            Id = subject.Id;
            Index = index;
            SubjectName = subject.Name;
            FormattedTime = formattedTime;
            Group = subject.Group;
            Color = subject.Color;
        }
    }
}
