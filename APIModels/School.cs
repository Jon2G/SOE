namespace APIModels
{
    public class School
    {
        public string HomePage { get; private set; }
        public string Name { get;private set; }
        public string ImgPath { get; private set; }

        public School(){}
        public School(string HomePage, string Name, string ImgPath)
        {
            this.HomePage = HomePage;
            this.Name = Name;
            this.ImgPath = ImgPath;
        }
    }
}
