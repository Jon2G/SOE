using AsyncAwaitBestPractices;
using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.API;
using SOE.Data.Archives;
using SOE.Enums;
using SOE.Views.ViewItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.Models.TodoModels
{
    [FirestoreData, FireStoreCollection("ToDo")]
    public class ToDo : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }

        private string _Title;
        [FirestoreProperty]
        public string Title
        {
            get => this._Title;
            set
            {
                this._Title = value;
                this.Raise(() => this.Title);
            }
        }
        private string _Description;
        private Document _Document;
        [FirestoreProperty]
        public Document Document
        {
            get => _Document;
            set
            {
                if (this._Document != value)
                {
                    _Document = value;
                    Raise(() => Document);
                    this.LoadDocument();
                }
            }
        }
        [FirestoreProperty]
        public string Description
        {
            get => this._Description;
            set
            {
                this._Description = value;
                this.Raise(() => this.Description);
            }
        }
        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp GDate { get; set; }
        public DateTime Date
        {
            get => this.GDate.ToDateTime().ToLocalTime();
            set
            {
                GDate = Google.Cloud.Firestore.Timestamp.FromDateTime(value.ToUniversalTime());
                Raise(() => Date);
            }
        }
        [FirestoreProperty]
        public int DueTime { get; set; }
        public TimeSpan Time
        {
            get => FireStoreExtensions.ToFirestoreTime(DueTime);
            set
            {
                DueTime = FireStoreExtensions.ToFirestoreTime(value);
                Raise(() => Time);
            }
        }
        [FirestoreProperty]
        public bool HasPictures { get; set; }

        private Subject _Subject;
        [FirestoreProperty]
        public Subject Subject
        {
            get => this._Subject;
            set
            {
                this._Subject = value;
                this.Raise(() => this.Subject);
            }
        }
        public string FormattedTime => $"{Time:hh}:{Time:mm}";
        public string FormattedDate => $"{Date.DayOfWeek.GetDayName()} - {Date:dd/MM}";

        public async Task<List<PhotoArchive>> GetPhotos()
        {
            await Task.Yield();
            List<PhotoArchive> photos = new();
            if (!HasPictures)
            {
                return photos;
            }
            try
            {
                photos = await
                    Keeper.GetById<PhotoArchive>(this.DocumentId)
                        .ToListAsync();
                foreach (PhotoArchive photoArchive in photos)
                {
                    photoArchive.LoadImage().SafeFireAndForget();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "GetPhotos");
            }
            return photos;
        }

        private FormattedString _FormattedString;
        public FormattedString FormattedString
        {
            get => _FormattedString; set
            {
                _FormattedString = value;
                Raise(() => FormattedString);
            }
        }
        public ICommand OpenBrowserCommand { get; }
        private void LoadDocument()
        {
            var formattedString = new FormattedString();
            foreach (var part in this.Document.DocumentParts)
            {
                var span = new Span()
                {
                    Text = part.Content
                };
                switch (part.DocType)
                {
                    case DocType.Link:
                        span.TextColor = Color.DodgerBlue;
                        span.FontAttributes = FontAttributes.Italic | FontAttributes.Bold;
                        span.TextDecorations = TextDecorations.Underline;
                        span.GestureRecognizers.Add(new TapGestureRecognizer() { CommandParameter = part.Content, Command = OpenBrowserCommand });
                        break;
                }
                formattedString.Spans.Add(span);
            }
            FormattedString = formattedString;
        }

        private PendingStatus _Status;

        [FirestoreProperty]
        public PendingStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                Raise(() => Status);
            }
        }

        internal static async Task<string> Share(ToDo toDo)
        {
            await Task.Yield();
            throw new NotImplementedException();
            //try
            //{
            //    if (toDo.Subject.IsOffline)
            //    {

            //        if (!await toDo.Subject.Sync(AppData.Instance, new SyncService()))
            //        {
            //            Application.Current.MainPage.DisplayAlert("Opps...",
            //                    "No fue posible compartir esta tarea, revise su conexión a internet", "Ok")
            //                .SafeFireAndForget();
            //            return null;
            //        }
            //    }

            //    Response Response = await APIService.Current.PostToDo(toDo);
            //    if (Response.ResponseResult != APIResponseResult.OK)
            //    {
            //        Application.Current.MainPage.DisplayAlert("Opps...",
            //            $"No fue posible compartir esta tarea, revise su conexión a internet.\n{Response.Message}",
            //            "Ok").SafeFireAndForget();
            //        return null;
            //    }

            //    if (toDo.HasPictures)
            //    {
            //        await PostPictures();
            //    }

            //    return DynamicLinkFormatter.GetDynamicUrl("share",
            //        new Dictionary<string, string>() { { "type", "todo" }, { "id", toDo.Guid.ToString("N") } });
            //}
            //catch (Exception ex)
            //{
            //    App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget();
            //    return null;
            //}

            //async Task PostPictures()
            //{
            //    await Task.Yield();
            //    try
            //    {
            //        foreach (PhotoArchive photo in GetPhotos(toDo))
            //        {
            //            using (Stream stream = await photo.GetStream())
            //            {
            //                await APIService.Current.PostTodoPicture(stream.ToByteArray(), toDo.Guid);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Logger?.Error(ex, "PostPictures");
            //    }
            //}
        }


        public static int DaysLeft(ToDo toDo) => DaysLeft(toDo.Date.Add(toDo.Time));
        private static int DaysLeft(DateTime date) => (date - DateTime.Now).Days;
        public ToDo()
        {
            this.Status = PendingStatus.Pending;
            OpenBrowserCommand = new Command<string>(OpenBrowser);
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;

        }
        private async void OpenBrowser(string zelda)
        {
            UriBuilder builder = new(zelda);
            await Browser.OpenAsync(builder.Uri, BrowserLaunchMode.SystemPreferred);
        }

        public ToDo LoadSubject()
        {
            //this.Subject = SubjectService.Get(this.SubjectId);
            return this;
        }

        public static CollectionReference Collection =>
            FireBaseConnection.Instance.UserDocument.Collection<ToDo>();

        public static async IAsyncEnumerable<ToDo> Query(Query query)
        {
            QuerySnapshot capitalQuerySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ConvertTo<ToDo>();
            }
        }

        public async Task Save(IEnumerable<PhotoArchive> Photos = null)
        {
            await Task.Yield();
            HasPictures = Photos is not null && Photos.Any();
            if (Description is null)
            {
                Description = "";
            }
            this.Document = new Document().Parse(Description);
            //le quita las horas y segundos a la fecha
            Date = new DateTime(Date.Year, Date.Month, Date.Day);
            var result = await ToDo.Collection.AddAsync(this);
            this.DocumentId = result.Id;
            if (HasPictures)
            {
                foreach (PhotoArchive archive in Photos)
                {
                    archive.ParentId = this.DocumentId;
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
                    await Keeper.Save(archive);
                }
            }
            /////////////
            if (Shell.Current is AppShell app)
            {
                PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction);
            }
            await Shell.Current.Navigation.PopToRootAsync(true);
        }

        public static Task<List<ToDo>> Get(PendingStatus status = PendingStatus.Pending)
        {
            return ToDo.Query(ToDo.Collection
                     .WhereEqualTo(nameof(ToDo.Status), status)
                     .OrderBy(nameof(ToDo.GDate)).OrderBy(nameof(ToDo.Subject)).OrderBy(nameof(ToDo.DueTime)))
                 .ToListAsync()
                 .AsTask();
        }
        public static ValueTask<ToDo> Get(string DocumentId)
        {
            Query q = Collection.WhereEqualTo(nameof(DocumentId), DocumentId);
            return Query(q).FirstOrDefaultAsync();
        }
    }
}