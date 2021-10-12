using AsyncAwaitBestPractices;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kit;
using Kit.Services.Web;
using Newtonsoft.Json;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Models;
using SOE.Models.Data;
using SOE.Models.TaskFirst;
using SOE.Services;
using SOE.Views.Pages;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using SOEWeb.Shared;
using System.Linq;
using System.Text;
using Device = Kit.Daemon.Devices.Device;
using SOEWeb.Shared.Enums;
using SOEWeb.Shared.Interfaces;
using SOEWeb.Shared.Processors;

namespace SOE.API
{
    public static class APIService
    {
        public const string ShareTodo = "ShareTodo";
        public const string ShareReminder = "ShareReminder";

#if DEBUG
        //LOCAL
        public const string NonHttpsUrl = "192.168.0.32:5001";
        public const string NonProdUrl = "192.168.0.32";
#else
        //AWS
        public const string NonProdUrl = "dhokq2d69j.execute-api.us-east-2.amazonaws.com";
        public static string NonHttpsUrl => $"{NonProdUrl}/Prod";
#endif

        //Otros
        public static string BaseUrl => $"https://{NonHttpsUrl}";
        public static string Url => $"{BaseUrl}/App";

        public static async Task<Response> TestDb()
        {
            await Task.Yield();
            try
            {
                WebService WebService = new WebService(Url);
                Kit.Services.Web.ResponseResult result = await WebService.GET(nameof(TestDb));
                return JsonConvert.DeserializeObject<Response>(result.Response);
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        public static async Task<Response> Hello()
        {
            await Task.Yield();
            try
            {
                WebService WebService = new WebService(Url);
                Kit.Services.Web.ResponseResult result = await WebService.GET(nameof(Hello));
                return JsonConvert.DeserializeObject<Response>(result.Response);
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        internal static async Task<bool> IsOnline()
        {
            Response hello = await APIService.TestDb();
            return hello.ResponseResult == APIResponseResult.OK;
        }

        public static async Task<Response<int>> SignUp(UserType Type, SOEWeb.Shared.Device Device)
        {
            await Task.Yield();
            try
            {
                WebService WebService = new WebService(Url);

                if (!await IsOnline())
                {
                    return Response<int>.Offline;
                }


                User User = AppData.Instance.User;
                if (SOEWeb.Shared.Validations.ValidateLogin(User.Boleta, User.NickName, User.Email, User.School,
                            Device.DeviceKey, Type)
                        is string v_error
                    && !string.IsNullOrEmpty(v_error))
                {
                    return new Response<int>(APIResponseResult.NOT_EXECUTED, v_error);
                }

                Kit.Services.Web.ResponseResult result = await WebService.GET("SignUp",
                    User.Boleta, User.Name, User.NickName, User.Email, User.School.Id.ToString(),
                    ((int)Type).ToString(), JsonConvert.SerializeObject(Device));
                if (result.Response == "ERROR")
                {
                    return new Response<int>(APIResponseResult.INTERNAL_ERROR, result.Response);
                }

                return JsonConvert.DeserializeObject<Response<int>>(result.Response);
            }
            catch (Exception ex)
            {
                return new Response<int>(APIResponseResult.INTERNAL_ERROR, ex.Message);
            }
        }
        public static async Task<Response<string>> PostClassTime(byte[] byteArray, string User)
        {
            await Task.Yield();
            WebService WebService = new WebService(Url);
            if (byteArray.Length <= 0)
            {
                return Response<string>.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(byteArray, "PostClassTime", User);
            if (result.Response == "ERROR")
            {
                return new Response<string>(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response<string>>(result.Response);
        }
        public static async Task<Response<string>> PostGrades(byte[] HTML)
        {
            await Task.Yield();
            WebService WebService = new WebService(Url);
            if (HTML.Length <= 0)
            {
                return Response<string>.Error;
            }
            Kit.Services.Web.ResponseResult result =
                await WebService.PostAsBody(HTML, "PostGrades", AppData.Instance.User.Boleta);
            if (result.Response == "ERROR")
            {
                return new Response<string>(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response<string>>(result.Response);
        }
        public static async Task<Response<int>> PostCareer(string CareerName, string User)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(CareerName))
            {
                return Response<int>.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("PostCareer", CareerName, User);
            if (result.Response == "ERROR")
            {
                return new Response<int>(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response<int>>(result.Response);
        }
        internal static async Task<Response<TodoBase>> DownloadSharedTodo(Guid todoGuid, bool includeFiles)
        {
            if (Guid.Empty == todoGuid)
            {
                return Response<TodoBase>.Error;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("ShareTodo",
                todoGuid.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response<TodoBase>(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            Response<TodoBase> response = JsonConvert.DeserializeObject<Response<TodoBase>>(result.Response);
            if (response.Extra is TodoBase todob)
            {
                ToDo todo = todob.Elevate<ToDo>();
                //
                List<Subject> subjects = SubjectService.ToList(x => x.IsOffline);
                foreach (Subject offlineSubject in subjects)
                {
                    if (!await offlineSubject.Sync(AppData.Instance, new SyncService()))
                    {
                        return new Response<TodoBase>(APIResponseResult.INTERNAL_ERROR,
                            "No fue posible descargar esta tarea, revise su conexión a internet");
                    }
                }
                //
                //check if subjects exists or is offline
                Subject subject = SubjectService.Get(todo.SubjectId);
                if (subject is null)
                {
                    return new Response<TodoBase>(APIResponseResult.INVALID_REQUEST,
                        "No esta inscrito en la materia asociada a esta tarea");
                }

                List<PhotoArchive> photos = null;
                if (includeFiles)
                {
                    photos = new List<PhotoArchive>();

                    var responseArchives = await GetArchiveIds(todo.Guid);
                    if (responseArchives.ResponseResult == APIResponseResult.OK)
                        foreach (int id in responseArchives.Extra)
                        {
                            string fileResult = await DownloadPictureById(id);
                            photos.Add(new PhotoArchive(fileResult, FileType.Photo));
                        }
                }
                await ToDo.Save(todo, photos);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    PendingTasksView.Instance?.Model.Refresh(PendingTasksView.Instance?.OnRefreshCompleteAction));
            }
            return response;
        }
        internal static async Task<Response<ReminderBase>> DownloadSharedReminder(Guid reminderGuide)
        {
            if (Guid.Empty == reminderGuide)
            {
                return Response<ReminderBase>.Error;
            }



            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("ShareReminder",
                reminderGuide.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response<ReminderBase>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            Response<ReminderBase> response = JsonConvert.DeserializeObject<Response<ReminderBase>>(result.Response);
            if (response.Extra is ReminderBase reminderb)
            {
                Reminder reminder = reminderb.Elevate<Reminder>();

                if (reminder.Subject is not null)
                {
                    List<Subject> subjects = SubjectService.ToList(x => x.IsOffline);
                    foreach (Subject offlineSubject in subjects)
                    {
                        if (!await offlineSubject.Sync(AppData.Instance, new SyncService()))
                        {
                            return new Response<ReminderBase>(APIResponseResult.INTERNAL_ERROR,
                                "No fue posible descargar este recordatorio, revise su conexión a internet");
                        }
                    }

                    //check if subjects exists or is offline
                    Subject subject = SubjectService.Get(reminder.SubjectId);
                    if (subject is null)
                    {
                        return new Response<ReminderBase>(APIResponseResult.INVALID_REQUEST,
                            "No esta inscrito en la materia asociada a esta tarea");
                    }
                }

                reminder.Status = PendingStatus.Pending;
                await Reminder.Save(reminder);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                 {
                     PendingRemindersView.Instance?.Model?.Load();
                 });
            }
            return response;
        }

        private static async Task<Response<int[]>> GetArchiveIds(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return Response<int[]>.NotExecuted;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetArchieveIds", guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<int[]>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<int[]>>(result.Response);
        }

        private static async Task<string> DownloadPictureById(int id)
        {
            WebService webService = new WebService(Url);
            FileInfo file = await Keeper.Save(webService.DownloadFile("GetArchieveById", id.ToString()));
            return file.FullName;
        }
        public static async Task<Response> PostToDo(TodoBase todo)
        {
            await Task.Yield();
            if (todo is null || string.IsNullOrEmpty(todo.Title)
                            || Guid.Empty == todo.Guid
                            || todo.Subject is null
                            || todo.Subject.Id <= 0
                            || todo.Subject.IdTeacher <= 0)
            {
                return Response.Error;
            }
            string json_todo = todo.JsonSerializeObject<TodoBase>();
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
                System.Text.Encoding.UTF8.GetBytes(json_todo),
                "PostToDo", AppData.Instance.User.Boleta);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostReminder(Reminder Reminder)
        {
            await Task.Yield();
            if (Reminder is null || string.IsNullOrEmpty(Reminder.Title)
                            || Guid.Empty == Reminder.Guid)
            {
                return Response.Error;
            }
            string json_Reminder = Reminder.JsonSerializeObject<ReminderBase>();
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
                System.Text.Encoding.UTF8.GetBytes(json_Reminder),
                "PostReminder", AppData.Instance.User.Boleta);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostTodoPicture(byte[] Img, Guid ToDoGuid)
        {
            await Task.Yield();
            if (Img is null || Img.Length <= 0 || Guid.Empty == ToDoGuid)
            {
                return Response.Error;
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(
                Img, "PostTodoPicture", ToDoGuid.ToString());

            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }

            return JsonConvert.DeserializeObject<Response>(result.Response);
        }

        internal static Task<Response<IEnumerable<Classmate>>> GetClassmates(Subject subject)
            => GetClassmates(subject.Group, subject.IdTeacher, subject.Id);
        internal static async Task<Response<IEnumerable<Classmate>>> GetClassmates(string group, int TeacherId, int SubjectId)
        {
            if (string.IsNullOrEmpty(group) || TeacherId <= 0 || SubjectId <= 0)
            {
                return Response<IEnumerable<Classmate>>.InvalidRequest;
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("GetClassmates",
                group, TeacherId.ToString(), SubjectId.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<IEnumerable<Classmate>>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<IEnumerable<Classmate>>>(result.Response);
        }



        public static async Task<Response<Guid>> PostLink(Subject Subject, Link Link, User User)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(Link.Name) || Subject.IdTeacher <= 0)
            {
                return Response<Guid>.InvalidRequest;
            }
            if (string.IsNullOrEmpty(Link.Url) || !UriExtensions.IsValidUrl(Link.Url, out Uri uri))
            {
                return Response<Guid>.InvalidRequest;
            }

            Link.Url = uri.AbsoluteUri;
            string jsonlink = JsonConvert.SerializeObject(Link);
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await WebService.PostAsBody(
                    byteArray: Encoding.UTF8.GetBytes(jsonlink),
                    method: "PostLink",
                    query: null,
                    parameters: new[]
                    {
                        Subject.Id.ToString(),
                        Subject.IdTeacher.ToString(),
                        User.Boleta,
                        Subject.Group
                    });
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<Guid>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<Guid>>(result.Response);

        }

        internal static async Task<bool> ReportLink(Link link, ReportReason reason)
        {
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET(
                "ReportLink",
                AppData.Instance.User.Id.ToString(),
                link.Guid.ToString("N"),
                ((int)reason).ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            else
            {
                Response r = JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }
        internal static async Task<bool> DeleteLink(Link link, int userId)
        {
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("DeleteLink",
                userId.ToString(), link.Guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            else
            {
                Response r = JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }

        internal static Task<Response<IEnumerable<Link>>> GetLinks(Subject subject)
            => GetLinks(subject.Group, subject.IdTeacher, subject.Id);

        internal static async Task<Response<IEnumerable<Link>>> GetLinks(string group, int teacherId, int subjectId)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(group) || teacherId <= 0 || subjectId <= 0)
            {
                return Response<IEnumerable<Link>>.InvalidRequest;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetLinks",
                group, teacherId.ToString(), subjectId.ToString(), AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<IEnumerable<Link>>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<IEnumerable<Link>>>(result.Response);
        }
        public static async Task<Response<Guid>> PostContact(SchoolContact contact, User user)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(contact.Name))
            {
                return Response<Guid>.InvalidRequest;
            }
            if (!string.IsNullOrEmpty(contact.Url))
            {
                if (!contact.Url.IsValidUrl(out Uri uri))
                    return Response<Guid>.InvalidRequest;
                contact.Url = uri.AbsoluteUri;
            }
            string jsonContact = JsonConvert.SerializeObject(contact);
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.PostAsBody(
                    byteArray: Encoding.UTF8.GetBytes(jsonContact),
                    method: "PostContact",
                    query: null,
                    parameters: new[]
                    {
                        user.Boleta,
                        user.School.Id.ToString()
                    });
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new Response<Guid>(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            return JsonConvert.DeserializeObject<Response<Guid>>(result.Response);

        }
        internal static async Task<Response<IEnumerable<ContactsByDeparment>>> GetContacts(School school)
        {
            await Task.Yield();
            if (school.Id <= 0)
            {
                return new Response<IEnumerable<ContactsByDeparment>>(APIResponseResult.INVALID_REQUEST, "Escuela invalida");
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetContacts",
                school.Id.ToString(), AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new(APIResponseResult.INTERNAL_ERROR, result.Extra);
            }
            Response<IEnumerable<ContactsByDeparment>> response =
                JsonConvert.DeserializeObject<Response<IEnumerable<ContactsByDeparment>>>(result.Response);
            return response;
        }
        internal static async Task<bool> ReportContact(SchoolContact contact, ReportReason reason)
        {
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET(
                "ReportContact",
                contact.Guid.ToString("N"),
                ((int)reason).ToString(),
                  AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            else
            {
                Response r = JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }
        internal static async Task<bool> IsNickNameAvaible(string nickname)
        {
            await Task.Yield();
            if (!SOEWeb.Shared.Validations.IsValidNickName(nickname))
            {
                return true;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(IsNickNameAvaible), nickname);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return true;
            }
            Response r = JsonConvert.DeserializeObject<Response>(result.Response);
            return r.ResponseResult == APIResponseResult.YES;

        }

        internal static async Task<bool> BoletaIsRegistered(string boleta, School school)
        {
            await Task.Yield();
            if (!SOEWeb.Shared.Validations.IsValidBoleta(boleta) || school is null || school.Id <= 0)
            {
                return false;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(BoletaIsRegistered), boleta, school.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            Response r = JsonConvert.DeserializeObject<Response>(result.Response);
            return r.ResponseResult == APIResponseResult.YES;
        }

        internal static async Task<bool> DeleteContact(int userId, SchoolContact contact)
        {
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("DeleteContact",
                userId.ToString(), contact.Guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            else
            {
                Response r = JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }
        internal static async Task<Response<int>> GetSchoolId(User user)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET("GetSchoolId", user.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<int>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<int>>(result.Response);
        }

        public static async Task<Response<Subject>> PostSubject(Subject subject)
        {
            await Task.Yield();
            int userId = AppData.Instance.User.Id;
            if (userId <= 0 || subject.IsOffline)
            {
                return Response<Subject>.InvalidRequest;
            }
            int suffixer = subject.Id - OfflineConstants.IdBase;
            string suffix = ClassTimeDigester.GetGroupSuffix(subject.Group, ref suffixer);
            return await PostSubject(userId, subject.Group, suffix, subject.IdTeacher, subject.Name);
        }
        public static async Task<Response<Subject>> PostSubject(int UserId, string Group, string Suffix, int TeacherId, string SubjectName)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostSubject), UserId.ToString(), Group, Suffix, TeacherId.ToString(), SubjectName);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<Subject>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<Subject>>(result.Response);
        }

        public static async Task<Response<Teacher>> PostTeacher(Teacher teacher)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostTeacher), teacher.Name);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<Teacher>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<Teacher>>(result.Response);

        }
        public static async Task<Response<ClassTime>> PostClassTime(int TeacherId, int SubjectId, DayOfWeek Day, TimeSpan Begin, TimeSpan End)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostClassTime), TeacherId.ToString(), SubjectId.ToString(), ((int)(Day)).ToString(), Begin.ToString("c"), End.ToString("c"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<ClassTime>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<ClassTime>>(result.Response);
        }
        public static async Task<Response<Grade>> PostGrade(Grade grade, Subject subject)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET(nameof(PostGrade), ((int)grade.Partial).ToString(), grade.NumericScore.ToString(), subject.Group, AppData.Instance.User.Boleta);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return Response<Grade>.InvalidRequest;
            }
            return JsonConvert.DeserializeObject<Response<Grade>>(result.Response);

        }

    }
}