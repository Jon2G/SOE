using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Serializable,Preserve]
    public class DayModel:IGuid
    {
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("classes")]
        public ClassSquare[] Classes {get; set;}
        public DayModel()
        {
        }
    }
}

