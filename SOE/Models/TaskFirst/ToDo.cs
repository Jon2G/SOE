﻿using System;
using System.Windows.Input;
using APIModels;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Models.Scheduler;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.Models.TaskFirst
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
        public string FormattedTime => $"{Time:hh}:{Time:mm}";
       
        private FormattedString _FormattedString;
        [Ignore]
        public FormattedString FormattedString
        {
            get => _FormattedString; set
            {
                _FormattedString = value;
                Raise(() => FormattedString);
            }
        }
        public ICommand OpenBrowserCommand { get; }
        internal void LoadDocument()
        {
            FormattedString = new FormattedString();
            foreach (var part in DocumentPart.GetDoc(this.IdDocument))
            {
                var span = new Span()
                {
                    Text = part.Content
                };
                switch (part.DocType)
                {
                    case Enums.DocType.Link:
                        span.TextColor = Color.DodgerBlue;
                        span.FontAttributes = FontAttributes.Italic | FontAttributes.Bold;
                        span.TextDecorations = TextDecorations.Underline;
                        span.GestureRecognizers.Add(new TapGestureRecognizer() { CommandParameter = part.Content, Command = OpenBrowserCommand });
                        break;
                }
                FormattedString.Spans.Add(span);
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
        public int IdDocument { get; set; }
        public int IdKeeper { get; set; }
        public ToDo()
        {
            OpenBrowserCommand = new Command<string>(OpenBrowser);

        }
        //public ToDo(ToDo toDo)
        //{
        //        this.Id = toDo.Id;
        //        this.Title = toDo.Title;
        //        this.SubjectId = toDo.SubjectId;
        //        this.Date = toDo.Date;
        //        this.Time = toDo.Time;
        //        this.Description = toDo.Description;
        //        this.IdDocument = toDo.IdDocument;
        //        this.IdKeeper = toDo.IdKeeper;

        //}
        private async void OpenBrowser(string zelda)
        {
            UriBuilder builder = new UriBuilder(zelda);
            await Browser.OpenAsync(builder.Uri, BrowserLaunchMode.SystemPreferred);
        }

     

    }
}