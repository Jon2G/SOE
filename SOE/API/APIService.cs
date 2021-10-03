using AsyncAwaitBestPractices;
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
using SOE.Views.Pages;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.TasksViews;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System.Text;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.API
{
    public static class APIService
    {
        public const string ShareTodo = "ShareTodo";
        public const string ShareReminder = "ShareReminder";
        //AWS
        public const string NonProdUrl = "dhokq2d69j.execute-api.us-east-2.amazonaws.com";
        public static string NonHttpsUrl => $"{NonProdUrl}/Prod";
        //LOCAL
        //public const string NonHttpsUrl = "192.168.0.32:5001";
        //public const string NonProdUrl = "192.168.0.32";
        //Otros
        public static string BaseUrl => $"https://{NonHttpsUrl}";
        public static string Url => $"{BaseUrl}/App";

        public static async Task<Response> Hello()
        {
            await Task.Yield();
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await WebService.GET("Hello");
            return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
        }
        public static async Task<Response> Login(string Usuario, string PasswordPin, string school = null)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin) || PasswordPin.Length < 8
                                              || (!SOEWeb.Shared.Validations.IsValidEmail(Usuario) && !SOEWeb.Shared.Validations.IsValidBoleta(Usuario)))
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result =
                await WebService.GET("LogIn",
                    Usuario, PasswordPin, Device.Current.DeviceId, school);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> SignUp(string PasswordPin, UserType Type, SOEWeb.Shared.Device Device)
        {
            WebService WebService = new WebService(Url);
            User User = AppData.Instance.User;
            if (SOEWeb.Shared.Validations.ValidateLogin(User.Boleta, PasswordPin, User.NickName, User.Email, User.School, Device.DeviceKey, Type)
                    is string v_error
                && !string.IsNullOrEmpty(v_error))
            {
                return new Response(APIResponseResult.NOT_EXECUTED, v_error, v_error);
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("SignUp",
                User.Boleta, User.Name, User.NickName, User.Email,
                PasswordPin, User.School.Name, ((int)Type).ToString(), JsonConvert.SerializeObject(Device));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostClassTime(byte[] byteArray, string User)
        {
            await Task.Yield();
            WebService WebService = new WebService(Url);
            if (byteArray.Length <= 0)
            {
                return Response.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.PostAsBody(byteArray, "PostClassTime", User);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostGrades(byte[] HTML)
        {
            await Task.Yield();
            WebService WebService = new WebService(Url);
            if (HTML.Length <= 0)
            {
                return Response.Error;
            }
            Kit.Services.Web.ResponseResult result =
                await WebService.PostAsBody(HTML, "PostGrades", AppData.Instance.User.Boleta);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostCareer(string CareerName, string User)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(CareerName))
            {
                return Response.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("PostCareer", CareerName, User);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        internal static async Task<Response> DownloadSharedTodo(Guid todoGuid, bool includeFiles)
        {
            if (Guid.Empty == todoGuid)
            {
                return Response.Error;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("ShareTodo",
                todoGuid.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            if (!string.IsNullOrEmpty(response.Extra))
            {
                ToDo todo = JsonConvert.DeserializeObject<ToDo>(response.Extra);
                List<PhotoArchive> photos = null;
                if (includeFiles)
                {
                    photos = new List<PhotoArchive>();
                    foreach (int id in await GetArchiveIds(todo.Guid))
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
        internal static async Task<Response> DownloadSharedReminder(Guid reminderGuide)
        {
            if (Guid.Empty == reminderGuide)
            {
                return Response.Error;
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("ShareReminder",
                reminderGuide.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            if (!string.IsNullOrEmpty(response.Extra))
            {
                Reminder reminder = JsonConvert.DeserializeObject<Reminder>(response.Extra);
                reminder.Status = PendingStatus.Pending;
                await Reminder.Save(reminder);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                 {
                     PendingRemindersView.Instance?.Model?.Load();
                 });
            }
            return response;
        }

        private static async Task<List<int>> GetArchiveIds(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return new List<int>();
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetArchieveIds", guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<int>();
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<int>(JsonConvert.DeserializeObject<int[]>(response.Extra));
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
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
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
        internal static async Task<List<Classmate>> GetClassmates(string group, int TeacherId, int SubjectId)
        {
            if (string.IsNullOrEmpty(group) || TeacherId <= 0 || SubjectId <= 0)
            {
                return new List<Classmate>();
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("GetClassmates",
                group, TeacherId.ToString(), SubjectId.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<Classmate>();
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<Classmate>(JsonConvert.DeserializeObject<Classmate[]>(response.Extra));
        }
        public static async Task<Response> PostLink(Subject Subject, Link Link, User User)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(Link.Name) || Subject.IdTeacher <= 0)
            {
                return Response.InvalidRequest;
            }
            if (string.IsNullOrEmpty(Link.Url) || !UriExtensions.IsValidUrl(Link.Url, out Uri uri))
            {
                return Response.InvalidRequest;
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
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);

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
        internal static async Task<List<Link>> GetLinks(string group, int teacherId, int subjectId)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(group) || teacherId <= 0 || subjectId <= 0)
            {
                return new List<Link>();
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetLinks",
                group, teacherId.ToString(), subjectId.ToString(), AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<Link>();
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<Link>(JsonConvert.DeserializeObject<Link[]>(response.Extra));
        }
        public static async Task<Response> PostContact(SchoolContact contact, User user)
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(contact.Name))
            {
                return Response.InvalidRequest;
            }
            if (!string.IsNullOrEmpty(contact.Url))
            {
                if (!contact.Url.IsValidUrl(out Uri uri))
                    return Response.InvalidRequest;
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
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);

        }
        internal static async Task<List<ContactsByDeparment>> GetContacts(int schoolId)
        {
            await Task.Yield();
            if (schoolId <= 0)
            {
                return new List<ContactsByDeparment>();
            }
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await webService.GET("GetContacts",
                 schoolId.ToString(), AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<ContactsByDeparment>();
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<ContactsByDeparment>(JsonConvert.DeserializeObject<ContactsByDeparment[]>(response.Extra));
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
        internal static async Task<int> GetSchoolId(User user)
        {
            await Task.Yield();
            WebService webService = new WebService(Url);
            Kit.Services.Web.ResponseResult result =
                await webService.GET("GetSchoolId", user.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return -1;
            }
            Response r = JsonConvert.DeserializeObject<Response>(result.Response);
            if (r is not null && r.ResponseResult == APIResponseResult.OK)
            {
                return Convert.ToInt32(r.Extra);
            }
            return -1;
        }
    }
}