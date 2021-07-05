using Kit.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kit.Forms.Pages;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using FFImageLoading.Forms;
using Kit;
using SOE.API;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Services;
using Xamarin.Essentials;

namespace SOE.Models.TaskFirst
{
    public class ToDo : TodoBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get;
            set;
        }
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
                    await APIService.PostTodoPicture(await photo.Value.GetImageAsJpgAsync(), toDo.Guid);
                }
            }
            return $"{APIService.Url}/{APIService.ShareTodo}/{toDo.Guid:N}"; 

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
        private bool _Archived;
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
        private bool _Done;
        public bool Done
        {
            get => _Done;
            set
            {
                _Done = value;
                Raise(() => Done);
            }
        }
        public static int DaysLeft(ToDo toDo) => DaysLeft(toDo.Date.Add(toDo.Time));
        private static int DaysLeft(DateTime date) => (date - DateTime.Now).Days;
        public ToDo()
        {
            Done = false;
            Archived = false;
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

        public static ToDo GetById(int Id) =>
            AppData.Instance.LiteConnection.DeferredQuery<ToDo>($"SELECT * FROM {nameof(ToDo)} WHERE ID={Id} LIMIT 1")
                .ToList().FirstOrDefault();

        public static async Task Save(ToDo Todo, IEnumerable<Archive<CachedImage>> Photos = null)
        {
            Document.Delete(Todo.IdDocument);
            if (Todo.Description == null)
            {
                Todo.Description = "";
            }
            Document doc = Document.PaseAndSave(Todo.Description);
            Todo.IdDocument = doc.Id;


            Keeper.Delete(Todo.IdKeeper);
            Keeper keeper = Keeper.New();
            foreach (Archive<CachedImage> archive in Photos)
            {
                CachedImage image = archive.Value;
                using (FileStream file = new FileStream(archive.Path, FileMode.OpenOrCreate))
                {
                    using (MemoryStream memory = new MemoryStream(await image.GetImageAsPngAsync()))
                    {
                        await memory.CopyToAsync(file);
                    }
                }
                await keeper.Save(archive);
            }
            Todo.IdKeeper = keeper.Id;
            /////////////

            //le quita las horas y segundos a la fecha
            Todo.Date = new DateTime(Todo.Date.Year, Todo.Date.Month, Todo.Date.Day);

            AppData.Instance.LiteConnection.InsertOrReplace(Todo);
            /////////////
            if (Shell.Current is AppShell app)
            {
                await app.MasterPage.TaskFirstPage.Model.Refresh();
            }

            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }

    }
}