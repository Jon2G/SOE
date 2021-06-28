using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.Saes
{
    public class School
    {
        public bool IsSchoolSelected => !string.IsNullOrEmpty(HomePage);
        public string HomePage { get; set; }
        public string Name { get;private set; }
        public string ImgPath { get; private set; }

        public School() : this(null, null, null)
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
