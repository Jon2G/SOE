namespace SOEWeb.Shared
{
    public class Classmate
    {
        public string Name { get; set; }
        public string Mail { get; set; }

        public Classmate()
        {

        }
        public Classmate(string Name,string Mail)
        {
            this.Name = Name;
            this.Mail = Mail;
        }
    }
}
