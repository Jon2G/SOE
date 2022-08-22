using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using FirestoreLINQ;

using Kit;
using Kit.Forms.Extensions;
using Kit.Model;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
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
    [FireStoreCollection("ToDo"),Xamarin.Forms.Internals.Preserve(AllMembers =true)]
    public class ToDo : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }

        private string _Title;

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

        public Document Document
        {
            get => _Document;
            set
            {
                if (this._Document != value)
                {
                    _Document = value;
                    Raise(()=>Document);
                    this.LoadDocument();
                }
            }
        }

        public string Description
        {
            get => this._Description;
            set
            {
                this._Description = value;
                this.Raise(() => this.Description);
            }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;
                Raise(() => Date);
            }
        }

        private TimeSpan _Time;
        [Ignored]
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                if (TotalHours != value.TotalHours)
                {
                    TotalHours = value.TotalHours;
                }
                Raise(() => Time);
            }
        }

        public double TotalHours
        {
            get => Time.TotalHours;
            set
            {
                if (Time.TotalHours != value)
                {
                    Time = TimeSpan.FromHours(value);
                }
            }
        }

        public bool HasPictures { get; set; }
        [Ignored]
        private Subject _Subject;
        [Ignored]
        public Subject? Subject
        {
            get => this._Subject;
            set
            {
                this._Subject = value;
                this.SubjectId = value?.GetDocumentId();
                this.Raise(() => this.Subject);
            }
        }
        public string? SubjectId { get; set; }
        [Ignored]
        public string FormattedTime => $"{Time:hh}:{Time:mm}";
        [Ignored]
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
        [Ignored]
        private FormattedString _FormattedString;
        [Ignored]
        public FormattedString FormattedString
        {
            get => _FormattedString; set
            {
                _FormattedString = value;
                Raise(() => FormattedString);
            }
        }
        [Ignored]
        public ICommand OpenBrowserCommand { get; }
        private void LoadDocument()
        {
            FormattedString? formattedString = new FormattedString();
            foreach (DocumentPart? part in this.Document.DocumentParts)
            {
                Span? span = new Span()
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
        [Ignored]
        private PendingStatus _Status;
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
            OpenBrowserCommand = new AsyncCommand<string>(OpenBrowser);
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;

        }
        private Task OpenBrowser(string zelda)
        {
            UriBuilder builder = new(zelda);
            return Browser.OpenAsync(builder.Uri, BrowserLaunchMode.SystemPreferred);
        }

        public async Task<Subject> GetSubject()
        {
            this.Subject = await Subject.GetCachedSubject(this.SubjectId);
            return Subject;
        }

        public static ICollectionReference Collection =>
            FireBaseConnection.UserDocument.Collection<ToDo>();

        public static async IAsyncEnumerable<ToDo> IQuery(IQuery IQuery)
        {
            IQuerySnapshot capitalQuerySnapshot = await IQuery.GetAsync();
            foreach (IDocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                yield return documentSnapshot.ToObject<ToDo>();
            }
        }

        public Task Delete() => ToDo.Collection.Document(this.DocumentId).DeleteAsync();

        public async Task Save(IEnumerable<PhotoArchive>? Photos = null)
        {
            await Task.Yield();
            try
            {
                HasPictures = Photos is not null && Photos.Any();
                if (Description is null)
                {
                    Description = "";
                }

                if (string.IsNullOrEmpty(this.DocumentId))
                {
                    this.Document = new Document().Parse(Description);
                    //le quita las horas y segundos a la fecha
                    Date = new DateTime(Date.Year, Date.Month, Date.Day);
                    IDocumentReference? result = await ToDo.Collection.AddAsync<ToDo>(this);
                    this.DocumentId = result.Id;
                }
                else
                {
                    await Collection.Document(DocumentId).SetAsync(this);
                }


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
            }
            catch(Exception ex)
            {
                Console.WriteLine("SOE.iOS " + ex.ToString());
                Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString(), "ERROR", "Ok");
            }
            Shell.Current.Navigation.PopToRootAsync(true).SafeFireAndForget();
        }

        public static async Task<List<ToDo>> Get(PendingStatus status = PendingStatus.Pending)
        {
            IQuerySnapshot query = await ToDo.Collection
                .WhereEqualsTo(nameof(ToDo.Status), (int)status)
                .OrderBy(nameof(ToDo.Date)).OrderBy(nameof(ToDo.SubjectId)).OrderBy(nameof(ToDo.TotalHours))
                .GetAsync();
            return query.ToObjects<ToDo>().ToList();
        }
        public static async ValueTask<ToDo?> Get(string DocumentId)
        {
            IQuery q = Collection.WhereEqualsTo(nameof(DocumentId), DocumentId);
            var todo=await IQuery(q).FirstOrDefaultAsync();
            return todo;
        }
    }
}