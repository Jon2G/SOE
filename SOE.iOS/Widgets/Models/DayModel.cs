using Foundation;
using Kit.Sql.Interfaces;
using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Serializable,Preserve(AllMembers =true)]
    public class DayModel
    {
        //Day of the week
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

