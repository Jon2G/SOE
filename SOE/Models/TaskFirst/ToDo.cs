using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kit.Sql.Attributes;
using Xamarin.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using AsyncAwaitBestPractices;
using FFImageLoading;
using Kit;
using Kit.Forms.Extensions;
using Newtonsoft.Json;
using SOE.API;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Services;
using SOE.Views.ViewItems.TasksViews;
using Xamarin.Essentials;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using SOE.Views.ViewItems;
using Device = Xamarin.Forms.Device;

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
            List<PhotoArchive> photos = new();
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == toDo.IdKeeper))
            {
                photos.Add(new PhotoArchive(archive.Path, FileType.Photo, false));
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
        private Enums.PendingStatus _Status;

        public Enums.PendingStatus Status
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
            await Task.Yield();
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
                    using (Stream stream = await photo.GetStream())
                    {
                        await APIService.PostTodoPicture(stream.ToByteArray(), toDo.Guid);
                    }
                }
            }
            return DynamicLinkFormatter.GetDynamicUrl("share",
                new Dictionary<string, string>() { { "type", "todo" }, { "id", toDo.Guid.ToString("N") } });
        }


        public static int DaysLeft(ToDo toDo) => DaysLeft(toDo.Date.Add(toDo.Time));
        private static int DaysLeft(DateTime date) => (date - DateTime.Now).Days;
        public ToDo()
        {
            this.Status = Enums.PendingStatus.Pending;
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
            await Task.Yield();
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
                Keeper keeper = Keeper.New();
                foreach (PhotoArchive archive in Photos)
                {
                    FileImageSource image = archive.Value;
                    if (!File.Exists(archive.Path))
                    {
                        using (FileStream file = new(archive.Path, FileMode.OpenOrCreate))
                        {
                            if (image is not null && !image.IsEmpty) // image.Height > 0
                            {
                                using Stream memory = image.ImageToStream();
                                await memory.CopyToAsync(file);
                            }
                        }
                    }
                    await keeper.Save(archive);
                }
                Keeper.Delete(Todo.IdKeeper);
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
                Device.BeginInvokeOnMainThread(() =>
                    PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction));
            }
            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }

    }
}