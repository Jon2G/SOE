using Newtonsoft.Json;

namespace SOEWeb.Shared
{
    public class School
    {
        [JsonProperty(nameof(Id))]
        public int Id { get; set; }
        [JsonProperty(nameof(HomePage))]
        public string HomePage { get; set; }
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }
        [JsonProperty(nameof(ImgPath))]
        public string ImgPath { get; set; }

        public School()
        {

        }
        public School(string HomePage, string Name, string ImgPath)
        {
            this.HomePage = HomePage;
            this.Name = Name;
            this.ImgPath = ImgPath;
        }
    }
}
