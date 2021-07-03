using Kit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Kit.Forms.Pages;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Windows.Input;
using APIModels;
using Kit;
using SOE.Services;
using Xamarin.Essentials;

namespace SOE.Models.TaskFirst
{
    public class ToDo : ModelBase, IGuid
    {
        private string _Title;
        private bool _Archived;
        private bool _Done;
        private TimeSpan _Time;
        private DateTime _Date;
        private Subject _Subject;
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
        [Ignore]
        public string FormattedDate => $"{Date.DayOfWeek.Dia()} - {Date:dd/MM}";

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
            get => _Subject;
            set
            {
                _Subject = value;
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
        public bool Done
        {
            get => _Done;
            set
            {
                _Done = value;
                Raise(() => Done);
            }
        }
        public bool Archived
        {
            get => _Archived;
            set
            {
                _Archived = value;
                Raise(() => Archived);
            }
        }

        public int IdDocument { get; set; }
        public int IdKeeper { get; set; }



        public ToDo()
        {
            Done = false;
            Archived = false;
            OpenBrowserCommand = new Command<string>(OpenBrowser);
            Date=DateTime.Now;

        }
        private async void OpenBrowser(string zelda)
        {
            UriBuilder builder = new UriBuilder(zelda);
            await Browser.OpenAsync(builder.Uri, BrowserLaunchMode.SystemPreferred);
        }

        public ToDo LoadSubject()
        {
            this.Subject = SubjectService.Get(this.SubjectId);
            return this;
        }

    }
}