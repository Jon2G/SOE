using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Kit.Model;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.DailyTasksModels
{
    public class DailyTasks:ModelBase
    {
        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                Raise(()=>Tasks);
            }
        }
        public DateTime Date { get; internal set; }

        public DailyTasks(DateTime Date)
        {
            this.Date = Date;
            Tasks = new ObservableCollection<Task>();

            LoadData();
        }



        public ICommand ItemSelectedCommand => new Command<string>(ItemSelected);



        private void LoadData()
        {
            var tasks = GetTasks();

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }

        private void ItemSelected(string parameter)
        {
            var tasks = GetTasks();

            Tasks.Clear();

            // Filter tasks
            foreach (var task in tasks
                .Where(t => t.Status.Equals(parameter, StringComparison.InvariantCultureIgnoreCase)))
            {
                Tasks.Add(task);
            }
        }
        public List<Task> GetTasks()
        {
            // NOTE: In this sample the focus is on the UI. This is a Fake service.
            return new List<Task>
            {
                new Task { Name = "Customer meeting", Category = "Hangouts", Status = "Warning", Time = "6pm", Color = Color.FromHex("#EEB611"), People = new List<Person> { new Person { Photo = "face2.jpg" }, new Person { Photo = "face5.jpg" } }, Completed = false },
                new Task { Name = "Catch up with Brian", Category = "Mobile Project", Status = "Warning", Time = "5pm", Color = Color.FromHex("#EEB611"), Completed = false },
                new Task { Name = "Approve final design review", Category = "Mobile Project", Status = "Problem", Time = "4pm", Color = Color.FromHex("#5677CB"), Completed = false },                new Task { Name = "Make new icons", Category = "Web App", Status = "Ready", Time = "3pm", Color = Color.FromHex("#51C6BF"), Completed = false },
                new Task { Name = "Design explorations", Category = "Company Website", Status = "Delayed", Time = "2pm", Color = Color.FromHex("#EE376C"), Completed = false },
                new Task { Name = "Lunch with Mary", Category = "Grill House", Status = "Ready", Time = "12pm", Color = Color.FromHex("#51C6BF"), Completed = false },
                new Task { Name = "Team meeting", Category = "Hangouts", Status = "Ready", Time = "10am", Color = Color.FromHex("#51C6BF"), People = new List<Person> { new Person { Photo = "face2.jpg" }, new Person { Photo = "face3.jpg" }, new Person { Photo = "face4.jpg" }, new Person { Photo = "face5.jpg" } }, Completed = false }
            };
        }

    }
}
