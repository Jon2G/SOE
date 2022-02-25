using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Serializable, Preserve]
    public class ToDoModel : IGuid
    {
        [JsonProperty("id")]
        public Guid Guid { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subject")]
        public ClassSquare Subject { get; set; }
        [JsonProperty("emoji")]
        public string Emoji { get; set; }
        [JsonProperty("day")]
        public string DayName { get; set; }
        [JsonProperty("formattedDateTime")]
        public string FormattedDatetime { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        public ToDoModel()
        {
        }
    }
}
