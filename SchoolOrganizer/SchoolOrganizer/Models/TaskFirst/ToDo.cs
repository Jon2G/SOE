using Kit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kit.Forms.Pages;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;

namespace SchoolOrganizer.Models.TaskFirst
{
    public class ToDo : ModelBase, IGuid
    {
        private string _Title;
        private TimeSpan _Time;
        private DateTime _Date;
        private Subject subject;
        private string _Description;
        public Guid Guid { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get;
            set;
        }
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                Raise(() => Title);
            }
        }
        public string Description
        {
            get => _Description;
            set
            {
                _Description = value;
                Raise(() => Description);
            }
        }
        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;
                Raise(() => Date);
            }
        }
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                Raise(() => Time);
            }
        }
        [Ignore]
        public Subject Subject
        {
            get => subject;
            set
            {
                subject = value;
                Raise(() => Subject);
            }
        }
        public int SubjectId
        {
            get => Subject.Id;
            set
            {
                if (Subject is null)
                {
                    Subject = new Subject();
                }
                Subject.Id = value;
            }
        }
        public ToDo()
        {
            //Title = "";
            //Time =new TimeSpan(0,11,0);
            //Date = DateTime.Now;
            ////Subject = null;
            //Description = "";

            //var todo=AppData.Instance.LiteConnection.Table<ToDo>().ToList();
            //todo.subject = Subject.Get(todo.Id);
            //hacer los grupos
            //por dia, por materia ...
            //  Kit.Forms.Pages.BasePopUp a = new BasePopUp();
            //await  a.ShowDialog();

            //var a = new ViewChangePopUp();
            //await a.Show();
            //await a.ShowDialog();



        }


    }
}
