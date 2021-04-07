using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.ViewItems
{
    public class TaskFirstViewModel : BaseViewModel
    {
        TaskViewModel TaskView = new();
        public Command DeleteCommand { get; set; }
        public ObservableCollection<ToDo> MyAgenda
        {
            get;
            set;
        }

        public TaskFirstViewModel()
        {

            MyAgenda = TaskView.Tareas;
            DeleteCommand = new Command<ToDo>(Eliminar);
        }
        //private ObservableCollection<Agenda> GetAgenda()
        //{

        //    return MyAgenda;
        //}
        public void Eliminar(ToDo agenda)
        {
            MyAgenda.Remove(agenda);
        }
    }
}
