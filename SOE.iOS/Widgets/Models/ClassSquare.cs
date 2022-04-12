using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using SOE.Models;
using SOEWeb.Shared;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Preserve]
    public class ClassSquare : IFireBaseKey
    {
        [JsonProperty("key")]
        public string Id { get; set; }
        public Subject Subject { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("formattedTime")]
        public string FormattedTime { get; set; }
        [JsonProperty("group")]
        public Group Group { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }

        public ClassSquare()
        {
        }
        public ClassSquare(Subject subject, int index, string formattedTime)
        {
            Subject = subject;
            Index = index;
            SubjectName = subject.Name;
            FormattedTime = formattedTime;
            Group = subject.GroupId;
            Color = subject.Color;
        }
    }
}
