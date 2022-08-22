using FirestoreLINQ;
using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using SOE.Models;
using SOEWeb.Shared;
using System;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
namespace SOE.iOS.Widgets.Models
{
    [Serializable, Preserve(AllMembers = true)]
    public class ClassSquare
    {
        [JsonProperty("id")]
        public string Id { get; set; }
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

        public ClassSquare(Subject subject, int index, string formattedTime)
        {
            Id = subject.GetDocumentId();
            Index = index;
            SubjectName = subject.Name;
            FormattedTime = formattedTime;
            Group = subject.Group.Name;
            Color = subject.Color;
        }
    }
}
