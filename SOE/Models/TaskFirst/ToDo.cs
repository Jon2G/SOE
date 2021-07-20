using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kit.Sql.Attributes;
using Xamarin.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using FFImageLoading;
using FFImageLoading.Forms;
using Kit;
using Newtonsoft.Json;
using SOE.API;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Services;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Essentials;

namespace SOE.Models.TaskFirst
{
    public class ToDo : TodoBase
    {
        [Ignore]
        public string FormattedTime => $"{Time:hh}:{Time:mm}";
        [Ignore]
        public string FormattedDate => $"{Date.DayOfWeek.Dia()} - {Date:dd/MM}";

        internal static List<PhotoArchive> GetPhotos(ToDo toDo)
        {
            List<PhotoArchive> photos = new List<PhotoArchive>();
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == toDo.IdKeeper))
            {
                photos.Add(new PhotoArchive(archive.Path, FileType.Photo));
            }
            return photos;
        }

        private FormattedString _FormattedString;

        [Ignore, XmlIgnore, JsonIgnore]
        public FormattedString FormattedString
        {
            get => _FormattedString; set
            {
                _FormattedString = value;
                Raise(() => FormattedString);
            }
        }
        [XmlIgnore]
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
        private ToDoStatus _Status;
       
        public  ToDoStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                Raise(() => Status);
            }
        }
        public Guid IdDocument { get; set; }
        public int IdKeeper { get; set; }

        public int Index { get; set; }
        public void SetNextIndex()
        {
            int index = AppData.Instance.LiteConnection.Single<int>("SELECT MAX([INDEX]) FROM TODO");
            index++;
            AppData.Instance.LiteConnection.Execute("UPDATE TODO SET [INDEX]=? WHERE GUID=?", index, this.Guid);
            this.Index = index;
        }
        internal static async Task<string> Share(ToDo toDo, bool IncludeFiles)
        {
            Response Response = await APIService.PostToDo(toDo);
            if (Response.ResponseResult != APIResponseResult.OK)
            {
                App.Current.MainPage.DisplayAlert("Opps...", Response.Message, "Ok").SafeFireAndForget();
                return null;
            }
            if (IncludeFiles)
            {
                foreach (PhotoArchive photo in GetPhotos(toDo))
                {
                    using FileStream photo_file = new FileStream(photo.Path, FileMode.Open);
                    await APIService.PostTodoPicture(photo_file.ToByteArray(), toDo.Guid);

                }
            }
            return $"{APIService.Url}/{APIService.ShareTodo}/{toDo.Guid:N}";

        }

        public static int DaysLeft(ToDo toDo) => DaysLeft(toDo.Date.Add(toDo.Time));
        private static int DaysLeft(DateTime date) => (date - DateTime.Now).Days;
        public ToDo()
        {
            Status = ToDoStatus.Pending;
            OpenBrowserCommand = new Command<string>(OpenBrowser);
            Date = DateTime.Now;

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

        public static ToDo GetById(Guid Id) =>
            AppData.Instance.LiteConnection.DeferredQuery<ToDo>($"SELECT * FROM {nameof(ToDo)} WHERE Guid=? LIMIT 1", Id)
                .ToList().FirstOrDefault();

        public static async Task Save(ToDo Todo, IEnumerable<PhotoArchive> Photos = null)
        {
            if (Todo.IdDocument != Guid.Empty)
            {
                Document.Delete(Todo.IdDocument);
            }
            if (Todo.Description == null)
            {
                Todo.Description = "";
            }
            Document doc = Document.PaseAndSave(Todo.Description);
            Todo.IdDocument = doc.Guid;
            if (Photos is not null)
            {
                Keeper.Delete(Todo.IdKeeper);
                Keeper keeper = Keeper.New();
                foreach (Archive<CachedImage> archive in Photos)
                {
                    CachedImage image = archive.Value;
                    using (FileStream file = new FileStream(archive.Path, FileMode.OpenOrCreate))
                    {
                        if (image is not null && image.Height > 0)
                        {
                            using MemoryStream memory = new MemoryStream(await image.GetImageAsPngAsync());
                            await memory.CopyToAsync(file);
                        }
                    }
                    await keeper.Save(archive);
                }
                Todo.IdKeeper = keeper.Id;
            }
            /////////////

            //le quita las horas y segundos a la fecha
            Todo.Date = new DateTime(Todo.Date.Year, Todo.Date.Month, Todo.Date.Day);

            AppData.Instance.LiteConnection.InsertOrReplace(Todo);
            Todo.SetNextIndex();
            /////////////
            if (Shell.Current is AppShell app)
            {
                MainTaskView.Instance?.Model.Refresh().SafeFireAndForget();
            }

            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }

    }
}