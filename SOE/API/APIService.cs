using AsyncAwaitBestPractices;
using Kit;
using Kit.Services.Web;
using Newtonsoft.Json;
using SOE.Data.Archives;
using SOE.Models;
using SOE.Secrets;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SOE.API
{
    public class APIService
    {
        public const string ShareTodo = "ShareTodo";
        public const string ShareReminder = "ShareReminder";
        public static APIService Current => _Current.Value;
        private static Lazy<APIService> _Current = new Lazy<APIService>(() => new APIService());
        public bool IsOnline { get; private set; }
        public APIService()
        {
            IsOnline = this.HasWifiOrData();
            Connectivity.ConnectivityChanged += (s, e) => TryToConnect().SafeFireAndForget();
        }
        public async Task<bool> Test()
        {
            await Task.Yield();
            Response hello = await TestDb();
            return hello.ResponseResult == APIResponseResult.OK;
        }

        private Task<PingReply> PingOrTimeout()
        {
            Kit.Services.Web.ScanIpAddress scan = new Kit.Services.Web.ScanIpAddress();
            return scan.PingOrTimeout(DotNetEnviroment.ServiceIP, 1000);
        }
        public async Task<bool> IsPingRechable()
        {
            var reachable = await PingOrTimeout();
            bool isReachable = reachable is not null && (reachable.Status == System.Net.NetworkInformation.IPStatus.Success);
            return isReachable;
        }

        private async Task TryToConnect()
        {
            await Task.Yield();
            if (HasWifiOrData())
            {
                if (await this.IsPingRechable())
                {
                    IsOnline = await this.Test();
                    return;
                }
            }
            IsOnline = false;
        }
        private bool HasWifiOrData()
        {
            var profiles = Connectivity.ConnectionProfiles.ToArray();
            return profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular);
        }

        private async Task<Response> TestDb()
        {
            await Task.Yield();
            try
            {
                WebService WebService = new WebService(DotNetEnviroment.Url);
                Kit.Services.Web.ResponseResult result = await WebService.GET(nameof(TestDb));
                return JsonConvert.DeserializeObject<Response>(result.Response);
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        public async Task<Response> Hello()
        {
            await Task.Yield();
            try
            {
                WebService WebService = new(DotNetEnviroment.Url);
                Kit.Services.Web.ResponseResult result = await WebService.GET(nameof(Hello));
                return JsonConvert.DeserializeObject<Response>(result.Response);
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }

        //public async Task<Response<int>> SignUp(UserType Type, SOEWeb.Shared.Device Device)
        //{
        //    await Task.Yield();
        //    try
        //    {
        //        WebService WebService = new(DotNetEnviroment.Url);

        //        if (!await this.Test())
        //        {
        //            return Response<int>.Offline;
        //        }


        //        User User = AppData.Instance.User;
        //        if (SOEWeb.Shared.Validations.ValidateLogin(User.Boleta, User.NickName, User.Email, User.School,
        //                    Device.DeviceKey, Type)
        //                is string v_error
        //            && !string.IsNullOrEmpty(v_error))
        //        {
        //            return new Response<int>(APIResponseResult.INVALID_REQUEST, v_error);
        //        }

        //        Kit.Services.Web.ResponseResult result = await WebService.GET("SignUp",
        //            User.Boleta, User.Name, User.NickName, User.Email, User.School.Guid.ToString(),
        //            ((int)Type).ToString(), JsonConvert.SerializeObject(Device));
        //        if (result.Response == "ERROR")
        //        {
        //            return new Response<int>(APIResponseResult.INTERNAL_ERROR, result.Response);
        //        }

        //        return JsonConvert.DeserializeObject<Response<int>>(result.Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response<int>(APIResponseResult.INTERNAL_ERROR, ex.Message);
        //    }
        //}
        //public async Task<Response<string>> PostClassTime(byte[] byteArray, string User)
        //{
        //    await Task.Yield();
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    if (byteArray.Length <= 0)
        //    {
        //        return Response<string>.Error;
        //    }
        //    Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(byteArray, "PostClassTime", User);
        //    if (result.Response == "ERROR")
        //    {
        //        return new Response<string>(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    }
        //    return JsonConvert.DeserializeObject<Response<string>>(result.Response);
        //}
        //public async Task<Response<string>> PostGrades(byte[] HTML)
        //{
        //    await Task.Yield();
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    if (HTML.Length <= 0)
        //    {
        //        return Response<string>.Error;
        //    }
        //    Kit.Services.Web.ResponseResult result =
        //        await WebService.PostAsBody(HTML, "PostGrades", AppData.Instance.User.Boleta);
        //    if (result.Response == "ERROR")
        //    {
        //        return new Response<string>(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    }
        //    return JsonConvert.DeserializeObject<Response<string>>(result.Response);
        //}
        //public async Task<Response<int>> PostCareer(string CareerName, string User)
        //{
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    if (string.IsNullOrEmpty(CareerName))
        //    {
        //        return Response<int>.Error;
        //    }
        //    Kit.Services.Web.ResponseResult result = await WebService.GET("PostCareer", CareerName, User);
        //    if (result.Response == "ERROR")
        //    {
        //        return new Response<int>(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    }
        //    return JsonConvert.DeserializeObject<Response<int>>(result.Response);
        //}
        //internal async Task<Response<Todo>> DownloadSharedTodo(Guid todoGuid)
        //{
        //    await Task.Yield();
        //    throw new NotImplementedException();
        //    //if (Guid.Empty == todoGuid)
        //    //{
        //    //    return Response<TodoBase>.Error;
        //    //}
        //    //WebService webService = new(DotNetEnviroment.Url);
        //    //Kit.Services.Web.ResponseResult result = await webService.GET("ShareTodo",
        //    //    todoGuid.ToString("N"));
        //    //if (result.Response == "ERROR")
        //    //{
        //    //    return new Response<TodoBase>(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    //}
        //    //Response<TodoBase> response = JsonConvert.DeserializeObject<Response<TodoBase>>(result.Response);
        //    //if (response.Extra is TodoBase todob)
        //    //{
        //    //    ToDo todo = todob.Elevate<ToDo>();
        //    //    //
        //    //    List<Subject> subjects = SubjectService.ToList(x => x.IsOffline);
        //    //    foreach (Subject offlineSubject in subjects)
        //    //    {
        //    //        if (!await offlineSubject.Sync(AppData.Instance, new SyncService()))
        //    //        {
        //    //            return new Response<TodoBase>(APIResponseResult.INTERNAL_ERROR,
        //    //                "No fue posible descargar esta tarea, revise su conexión a internet");
        //    //        }
        //    //    }
        //    //    //
        //    //    //check if subjects exists or is offline
        //    //    Subject subject = SubjectService.Get(todo.SubjectId);
        //    //    if (subject is null)
        //    //    {
        //    //        return new Response<TodoBase>(APIResponseResult.INVALID_REQUEST,
        //    //            "No esta inscrito en la materia asociada a esta tarea");
        //    //    }

        //    //    List<PhotoArchive> photos = null;
        //    //    if (todo.HasPictures)
        //    //    {
        //    //        photos = new List<PhotoArchive>();

        //    //        var responseArchives = await GetArchiveIds(todo.Guid);
        //    //        if (responseArchives.ResponseResult == APIResponseResult.OK)
        //    //            foreach (int id in responseArchives.Extra)
        //    //            {
        //    //                string fileResult = await DownloadPictureById(id);
        //    //                photos.Add(new PhotoArchive(fileResult, FileType.Photo));
        //    //            }
        //    //    }
        //    //    await ToDo.Save(todo, photos);
        //    //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
        //    //        PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction));
        //    //}
        //    //return response;
        //}
        //internal async Task<Response<ReminderBase>> DownloadSharedReminder(Guid reminderGuide)
        //{
        //    if (Guid.Empty == reminderGuide)
        //    {
        //        return Response<ReminderBase>.Error;
        //    }
        //    throw new NotImplementedException();


        //    //WebService webService = new(DotNetEnviroment.Url);
        //    //Kit.Services.Web.ResponseResult result = await webService.GET("ShareReminder",
        //    //    reminderGuide.ToString("N"));
        //    //if (result.Response == "ERROR")
        //    //{
        //    //    return new Response<ReminderBase>(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    //}
        //    //Response<ReminderBase> response = JsonConvert.DeserializeObject<Response<ReminderBase>>(result.Response);
        //    //if (response.Extra is ReminderBase reminderb)
        //    //{
        //    //    Reminder reminder = reminderb.Elevate<Reminder>();

        //    //    if (reminder.Subject is not null)
        //    //    {
        //    //        List<Subject> subjects = SubjectService.ToList(x => x.IsOffline);
        //    //        foreach (Subject offlineSubject in subjects)
        //    //        {
        //    //            if (!await offlineSubject.Sync(AppData.Instance, new SyncService()))
        //    //            {
        //    //                return new Response<ReminderBase>(APIResponseResult.INTERNAL_ERROR,
        //    //                    "No fue posible descargar este recordatorio, revise su conexión a internet");
        //    //            }
        //    //        }

        //    //        //check if subjects exists or is offline
        //    //        Subject subject = SubjectService.Get(reminder.SubjectId);
        //    //        if (subject is null)
        //    //        {
        //    //            return new Response<ReminderBase>(APIResponseResult.INVALID_REQUEST,
        //    //                "No esta inscrito en la materia asociada a esta tarea");
        //    //        }
        //    //    }

        //    //    reminder.Status = PendingStatus.Pending;
        //    //    await Reminder.Save(reminder);
        //    //    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
        //    //     {
        //    //         PendingRemindersView.Instance?.Model?.Load();
        //    //     });
        //    //}
        //    //return response;
        //}

        private async Task<Response<int[]>> GetArchiveIds(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return Response<int[]>.InvalidRequest;
            }
            WebService webService = new(DotNetEnviroment.Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetArchieveIds", guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<int[]>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<int[]>>(result.Response);
        }

        private async Task<string> DownloadPictureById(int id)
        {
            WebService webService = new(DotNetEnviroment.Url);
            FileInfo file = await Keeper.Save(webService.DownloadFile("GetArchieveById", id.ToString()));
            return file.FullName;
        }
        //public async Task<Response> PostToDo(TodoBase todo)
        //{
        //    await Task.Yield();
        //    if (todo is null || string.IsNullOrEmpty(todo.Title)
        //                    || Guid.Empty == todo.Guid
        //                    || todo.Subject is null
        //                    || todo.Subject.Guid == Guid.Empty
        //                    || todo.Subject.Guid == Guid.Empty)
        //    {
        //        return Response.Error;
        //    }
        //    string json_todo = todo.JsonSerializeObject<TodoBase>();
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
        //        System.Text.Encoding.UTF8.GetBytes(json_todo),
        //        "PostToDo", AppData.Instance.User.Boleta);
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    return JsonConvert.DeserializeObject<Response>(result.Response);
        //}
        //public async Task<Response> PostReminder(Reminder Reminder)
        //{
        //    await Task.Yield();
        //    if (Reminder is null || string.IsNullOrEmpty(Reminder.Title)
        //                    || Guid.Empty == Reminder.Guid)
        //    {
        //        return Response.Error;
        //    }
        //    string json_Reminder = Reminder.JsonSerializeObject<ReminderBase>();
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
        //        System.Text.Encoding.UTF8.GetBytes(json_Reminder),
        //        "PostReminder", AppData.Instance.User.Boleta);
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    }
        //    return JsonConvert.DeserializeObject<Response>(result.Response);
        //}
        //public async Task<Response> PostTodoPicture(byte[] Img, Guid ToDoGuid)
        //{
        //    await Task.Yield();
        //    if (Img is null || Img.Length <= 0 || Guid.Empty == ToDoGuid)
        //    {
        //        return Response.Error;
        //    }
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
        //        Img, "PostTodoPicture", ToDoGuid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
        //    }
        //    else if (result.HttpStatusCode == HttpStatusCode.OK)
        //    {
        //        return JsonConvert.DeserializeObject<Response>(result.Response);
        //    }
        //    return Response.InvalidRequest;
        //}

        //internal Task<Response<IEnumerable<Classmate>>> GetClassmates(Subject subject)
        //    => GetClassmates(subject.Group, subject.IdTeacher, subject.Guid);
        //internal async Task<Response<IEnumerable<Classmate>>> GetClassmates(string group, Guid TeacherId, Guid SubjectId)
        //{
        //    if (string.IsNullOrEmpty(group) || TeacherId == Guid.Empty || SubjectId == Guid.Empty)
        //    {
        //        return Response<IEnumerable<Classmate>>.InvalidRequest;
        //    }
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await WebService.GET("GetClassmates",
        //        group, TeacherId.ToString(), SubjectId.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response<IEnumerable<Classmate>>(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    return JsonConvert.DeserializeObject<Response<IEnumerable<Classmate>>>(result.Response);
        //}



        //public async Task<Response<Guid>> PostLink(Subject Subject, Link Link, User User)
        //{
        //    await Task.Yield();
        //    if (string.IsNullOrEmpty(Link.Name) || Subject.IdTeacher ==Guid.Empty)
        //    {
        //        return Response<Guid>.InvalidRequest;
        //    }
        //    if (string.IsNullOrEmpty(Link.Url) || !UriExtensions.IsValidUrl(Link.Url, out Uri uri))
        //    {
        //        return Response<Guid>.InvalidRequest;
        //    }

        //    Link.Url = uri.AbsoluteUri;
        //    string jsonlink = JsonConvert.SerializeObject(Link);
        //    WebService WebService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result =
        //        await WebService.PostAsBody(
        //            byteArray: Encoding.UTF8.GetBytes(jsonlink),
        //            method: "PostLink",
        //            IQuery: null,
        //            parameters: new[]
        //            {
        //                Subject.Guid.ToString(),
        //                Subject.IdTeacher.ToString(),
        //                User.Boleta,
        //                Subject.Group
        //            });
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response<Guid>(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    return JsonConvert.DeserializeObject<Response<Guid>>(result.Response);

        //}

        //internal async Task<bool> ReportLink(Link link, ReportReason reason)
        //{
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET(
        //        "ReportLink",
        //        AppData.Instance.User.Guid.ToString(),
        //        link.Guid.ToString("N"),
        //        ((int)reason).ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        Response r = JsonConvert.DeserializeObject<Response>(result.Response);
        //        return r.ResponseResult == APIResponseResult.OK;
        //    }
        //}
        //internal async Task<bool> DeleteLink(Link link, Guid userId)
        //{
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET("DeleteLink",
        //        userId.ToString(), link.Guid.ToString("N"));
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        Response r = JsonConvert.DeserializeObject<Response>(result.Response);
        //        return r.ResponseResult == APIResponseResult.OK;
        //    }
        //}

        //internal Task<Response<IEnumerable<Link>>> GetLinks(Subject subject)
        //    => GetLinks(subject.Group, subject.IdTeacher, subject.Guid);

        //internal async Task<Response<IEnumerable<Link>>> GetLinks(string group, Guid teacherId, Guid subjectId)
        //{
        //    await Task.Yield();
        //    if (string.IsNullOrEmpty(group) || teacherId ==Guid.Empty || subjectId == Guid.Empty)
        //    {
        //        return Response<IEnumerable<Link>>.InvalidRequest;
        //    }
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET("GetLinks",
        //        group, teacherId.ToString(), subjectId.ToString(), AppData.Instance.User.Guid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response<IEnumerable<Link>>(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    return JsonConvert.DeserializeObject<Response<IEnumerable<Link>>>(result.Response);
        //}
        //public async Task<Response<Guid>> PostContact(SchoolContact contact, User user)
        //{
        //    await Task.Yield();
        //    if (string.IsNullOrEmpty(contact.Name))
        //    {
        //        return Response<Guid>.InvalidRequest;
        //    }
        //    if (!string.IsNullOrEmpty(contact.Url))
        //    {
        //        if (!contact.Url.IsValidUrl(out Uri uri))
        //            return Response<Guid>.InvalidRequest;
        //        contact.Url = uri.AbsoluteUri;
        //    }
        //    string jsonContact = JsonConvert.SerializeObject(contact);
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result =
        //        await webService.PostAsBody(
        //            byteArray: Encoding.UTF8.GetBytes(jsonContact),
        //            method: "PostContact",
        //            IQuery: null,
        //            parameters: new[]
        //            {
        //                user.Boleta,
        //                user.School.Guid.ToString()
        //            });
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new Response<Guid>(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    return JsonConvert.DeserializeObject<Response<Guid>>(result.Response);

        //}
        //internal async Task<Response<IEnumerable<ContactsByDeparment>>> GetContacts(School school)
        //{
        //    await Task.Yield();
        //    if (school.Guid== Guid.Empty)
        //    {
        //        return new Response<IEnumerable<ContactsByDeparment>>(APIResponseResult.INVALID_REQUEST, "Escuela invalida");
        //    }
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET("GetContacts",
        //        school.Guid.ToString(), AppData.Instance.User.Guid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return new(APIResponseResult.INTERNAL_ERROR, result.Extra);
        //    }
        //    Response<IEnumerable<ContactsByDeparment>> response =
        //        JsonConvert.DeserializeObject<Response<IEnumerable<ContactsByDeparment>>>(result.Response);
        //    return response;
        //}
        //internal async Task<bool> ReportContact(SchoolContact contact, ReportReason reason)
        //{
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET(
        //        "ReportContact",
        //        contact.Guid.ToString("N"),
        //        ((int)reason).ToString(),
        //          AppData.Instance.User.Guid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        Response r = JsonConvert.DeserializeObject<Response>(result.Response);
        //        return r.ResponseResult == APIResponseResult.OK;
        //    }
        //}
        internal async Task<bool> IsNickNameAvaible(string nickname)
        {
            await Task.Yield();
            if (!Models.Data.Validations.IsValidNickName(nickname))
            {
                return true;
            }
            WebService webService = new(DotNetEnviroment.Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(IsNickNameAvaible), nickname);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return true;
            }
            Response r = JsonConvert.DeserializeObject<Response>(result.Response);
            return r.ResponseResult == APIResponseResult.YES;

        }

        //internal async Task<bool> BoletaIsRegistered(string boleta, School school)
        //{
        //    await Task.Yield();
        //    if (!SOEWeb.Shared.Validations.IsValidBoleta(boleta) || school is null || school.Guid ==Guid.Empty)
        //    {
        //        return false;
        //    }
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result =
        //        await webService.GET(nameof(BoletaIsRegistered), boleta, school.Guid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return false;
        //    }
        //    Response r = JsonConvert.DeserializeObject<Response>(result.Response);
        //    return r.ResponseResult == APIResponseResult.YES;
        //}

        //internal async Task<bool> DeleteContact(Guid userId, SchoolContact contact)
        //{
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result = await webService.GET("DeleteContact",
        //        userId.ToString(), contact.Guid.ToString("N"));
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        Response r = JsonConvert.DeserializeObject<Response>(result.Response);
        //        return r.ResponseResult == APIResponseResult.OK;
        //    }
        //}
        //internal async Task<Response<int>> GetSchoolId(User user)
        //{
        //    await Task.Yield();
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result =
        //        await webService.GET("GetSchoolId", user.Guid.ToString());
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return Response<int>.InvalidRequest;
        //    }
        //    return JsonConvert.DeserializeObject<Response<int>>(result.Response);
        //}

        public async Task<Response<Subject>> PostSubject(Subject subject)
        {
            await Task.Yield();
            throw new NotImplementedException();
            //var user = AppData.Instance.User;
            //Guid userId = user.Guid;
            //if (userId ==Guid.Empty )
            //{
            //    return Response<Subject>.InvalidRequest;
            //}
            //int suffixer = (subject.Id - OfflineConstants.IdBase) + 1;
            //string suffix = ClassTimeDigester.GetGroupSuffix(subject.Group, ref suffixer);
            //return await PostSubject(userId, subject.Group, suffix, subject.IdTeacher, subject.Name);
            return null;
        }
        public async Task<Response<Subject>> PostSubject(int UserId, string Group, string Suffix, int TeacherId, string SubjectName)
        {
            await Task.Yield();
            WebService webService = new(DotNetEnviroment.Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostSubject), UserId.ToString(), Group, Suffix, TeacherId.ToString(), SubjectName);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<Subject>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<Subject>>(result.Response);
        }

        public async Task<Response<Teacher>> PostTeacher(Teacher teacher)
        {
            await Task.Yield();
            WebService webService = new(DotNetEnviroment.Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostTeacher), teacher.Name);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<Teacher>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<Teacher>>(result.Response);

        }
        public async Task<Response<ClassTime>> PostClassTime(int TeacherId, int SubjectId, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            await Task.Yield();
            WebService webService = new(DotNetEnviroment.Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostClassTime), TeacherId.ToString(), SubjectId.ToString(), ((int)(Day)).ToString(), Begin.ToString("c"), End.ToString("c"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<ClassTime>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<ClassTime>>(result.Response);
        }
        //public async Task<Response<Grade>> PostGrade(Grade grade, Subject subject)
        //{
        //    await Task.Yield();
        //    WebService webService = new(DotNetEnviroment.Url);
        //    Kit.Services.Web.ResponseResult result =
        //        await webService.GET(nameof(PostGrade), ((int)grade.Partial).ToString(), grade.NumericScore.ToString(), subject.Group, AppData.Instance.User.Boleta);
        //    if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
        //    {
        //        return Response<Grade>.InvalidRequest;
        //    }
        //    return JsonConvert.DeserializeObject<Response<Grade>>(result.Response);

        //}

    }
}