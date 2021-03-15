using System.Collections.Generic;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.DailyTasksModels
{
    public class Task
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public Color Color { get; set; }
        public List<Person> People { get; set; }
        public bool Completed { get; set; }
    }
}
