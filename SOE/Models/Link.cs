using Newtonsoft.Json;
using System;

namespace SOE.Models
{
    public class Link
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsOwner { get; set; }

        [JsonIgnore]
        public string ImageUrl
        {
            get
            {
                string google_service = $"https://www.google.com/s2/favicons?sz=64&domain_url={this.Url}";
                return google_service;
            }
        }

        public Link()
        {

        }
        public Link(string Name,string Url)
        {
            this.Name = Name;
            this.Url = Url;
        }
    }
}
