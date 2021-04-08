using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
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
      
        public Command DeleteCommand { get; set; }

        public ObservableCollection<ByDayGroup> DayGroups { get; set; }

        public TaskFirstViewModel()
        {
            //var subject = new Subject(1, 1, "Base de datos", "#e2F0cb", "7CV22");
            //var subject2 = new Subject(1, 1, "Sistemas de información", "#c7ceea", "7CV25");

            DayGroups = GetDays();
             //new ObservableCollection<ByDayGroup>()
            //{
            //    new()
            //    {
            //        FDateTime = DateTime.Today,
            //        SubjectGroups =
            //        {
            //            new BySubjectGroup(subject)
            //            {
            //                ToDoS =new ObservableCollection<ToDo>()
            //                {
            //                    new()
            //                    {
            //                        N_Tarea = "Diagrama",
            //                        Date = DateTime.Now,
            //                        Description = "Diagrama de don cuco",
            //                        H_Entrga = "11:00",
            //                        Subject = subject
            //                    },
            //                    new()
            //                    {
            //                        N_Tarea = "Formatos de fecha",
            //                        Date = DateTime.Now,
            //                        Description = "Documento de word con la descripcion de los formatos de fecha",
            //                        H_Entrga = "10:00",
            //                        Subject = subject
            //                    }
            //                }
            //            },
            //            new BySubjectGroup(subject2)
            //            {
            //                ToDoS =new ObservableCollection<ToDo>()
            //                {
            //                    new()
            //                    {
            //                        N_Tarea = "Diagrama",
            //                        Date = DateTime.Now,
            //                        Description = "Diagrama de don cuco",
            //                        H_Entrga = "11:00",
            //                        Subject = subject2
            //                    },
            //                    new()
            //                    {
            //                        N_Tarea = "Formatos de fecha",
            //                        Date = DateTime.Now,
            //                        Description = "Documento de word con la descripcion de los formatos de fecha",
            //                        H_Entrga = "10:00",
            //                        Subject = subject2
            //                    }
            //                }
            //            }

            //        }
            //    }
            //};
            DeleteCommand = new Command<ByDayGroup>(Eliminar);
        }
        private new ObservableCollection<ByDayGroup> GetDays()
        {
            var subject = new Subject(1, 1, "Base de datos", "#e2F0cb", "7CV22");
            var subject2 = new Subject(1, 1, "Sistemas de información", "#c7ceea", "7CV25");
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
                                    N_Tarea = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    H_Entrga = "11:00",
                                    Subject = subject
                                },
                                new()
                                {
                                    N_Tarea = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    H_Entrga = "10:00",
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
                                    N_Tarea = "Diagrama",
                                    Date = DateTime.Now,
                                    Description = "Diagrama de don cuco",
                                    H_Entrga = "11:00",
                                    Subject = subject2
                                },
                                new()
                                {
                                    N_Tarea = "Formatos de fecha",
                                    Date = DateTime.Now,
                                    Description = "Documento de word con la descripcion de los formatos de fecha",
                                    H_Entrga = "10:00",
                                    Subject = subject2
                                }
                            }
                        }

                    }
                }
            };

        }
        public void Eliminar(ByDayGroup dayGroup)
            {
                DayGroups.Remove(dayGroup);
            }
    }
}
