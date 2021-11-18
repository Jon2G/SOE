using Newtonsoft.Json;
using System;
namespace SOE.iOS.Widgets.Models
{
    [Serializable]
    public class ModelData
    {
        [JsonProperty("days")]
        public DayModel[] Days { get; set; }

        public ModelData()
        {

        }
    }
}

