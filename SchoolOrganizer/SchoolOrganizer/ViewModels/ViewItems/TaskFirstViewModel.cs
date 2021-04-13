using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TaskFirstViewModel : BaseViewModel
    {


        public ObservableCollection<ByDayGroup> DayGroups { get; set; }

        public TaskFirstViewModel()
        {
           
            DayGroups = GetDays();
            

        }



        private  ObservableCollection<ByDayGroup> GetDays()
        {
            var subject = new Subject(1, 1, "Base de datos", "#e2F0cb", "7CV22");
            var subject2 = new Subject(1, 1, "Sistemas de información", "#c7ceea", "7CV25");
            var subject3 = new Subject(1, 1, "Administración", "#c7ceea", "7CV25");
            var subject4 = new Subject(1, 1, "Control digital", "#c7ceea", "7CV25");
            return new ObservableCollection<ByDayGroup>
            {
               new ByDayGroup
               { 
                   FDateTime = DateTime.Today, 
                   SubjectGroups ={ new BySubjectGroup(subject)
                        {
                            ToDoS =new ObservableCollection<ToDo>()
                            {
                                new()
                                {
                                    Title = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    Time =new TimeSpan(11,0,0),// "11:00",
                                    Subject = subject
                                },
                                new()
                                {
                                    Title = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject
                                }
                            }
                        },
                        new BySubjectGroup(subject2)
                        {
                            ToDoS =new ObservableCollection<ToDo>()
                            {
                                new()
                                {
                                    Title = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject2
                                },
                                new()
                                {
                                    Title = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject2
                                }
                            }
                        },
                        new BySubjectGroup(subject3)
                        {
                            ToDoS =new ObservableCollection<ToDo>()
                            {
                                new()
                                {
                                    Title = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject3
                                },
                                new()
                                {
                                    Title = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject3
                                }
                            }
                        },

                        new BySubjectGroup(subject4)
                        {
                            ToDoS =new ObservableCollection<ToDo>()
                            {
                                new()
                                {
                                    Title = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    Time =new TimeSpan(11,0,0),
                                    Subject = subject4
                                },
                                new()
                                {
                                    Title = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    Time = new TimeSpan(11,0,0),
                                    Subject = subject4
                                }
                            }
                        }

                    }
                }
            };

        }

    }
}
