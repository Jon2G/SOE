using Foundation;
using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Serializable,Preserve]
    public class DayModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("classes")]
        public ClassSquare[] Classes {get; set;}
        public DayModel()
        {
        }
    }
}

