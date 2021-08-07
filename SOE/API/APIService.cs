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
        //public const string NonHttpsUrl = "192.168.0.32:44313";
        public const string NonHttpsUrl = "dhokq2d69j.execute-api.us-east-2.amazonaws.com/Prod";
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
                                              || (!Validations.IsValidEmail(Usuario) && !Validations.IsValidBoleta(Usuario)))
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
            if (Validations.ValidateLogin(User.Boleta,PasswordPin, User.NickName,User.Email,User.School, Device.DeviceKey,Type)
                    is string v_error
                &&!string.IsNullOrEmpty(v_error))
            {
                return new Response(APIResponseResult.NOT_EXECUTED,v_error, v_error);
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
        internal static async Task<Response> DownloadSharedTodo(Guid TodoGuid, bool IncludeFiles)
        {
            if (Guid.Empty == TodoGuid)
            {
                return Response.Error;
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("ShareTodo",
                TodoGuid.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            var response = JsonConvert.DeserializeObject<Response>(result.Response);
            if (!string.IsNullOrEmpty(response.Extra))
            {
                ToDo todo = JsonConvert.DeserializeObject<ToDo>(response.Extra);
                List<PhotoArchive> Photos = null;
                if (IncludeFiles)
                {
                    Photos = new List<PhotoArchive>();
                    foreach (int Id in await GetArchieveIds(todo.Guid))
                    {
                        var file_result = await DownloadPictureById(Id);
                        Photos.Add(new PhotoArchive(file_result, FileType.Photo));
                    }
                }
                await ToDo.Save(todo, Photos);
            }
            return response;
        }
        internal static async Task<Response> DownloadSharedReminder(Guid ReminderGuide)//////Movimiento
        {
            if (Guid.Empty == ReminderGuide)
            {
                return Response.Error;
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("ShareReminder",
                ReminderGuide.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            var response = JsonConvert.DeserializeObject<Response>(result.Response);
            if (!string.IsNullOrEmpty(response.Extra))
            {
                Reminder reminder = JsonConvert.DeserializeObject<Reminder>(response.Extra);
                await Reminder.Save(reminder);
            }
            return response;
        }
        internal static async Task<List<int>> GetArchieveIds(Guid Guid)
        {
            if (Guid == Guid.Empty)
            {
                return new List<int>();
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("GetArchieveIds", Guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<int>();
            }
            var response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<int>(JsonConvert.DeserializeObject<int[]>(response.Extra));
        }
        internal static async Task<string> DownloadPictureById(int Id)
        {
            WebService webService = new WebService(Url);
            FileInfo file = await Keeper.Save(webService.DownloadFile("GetArchieveById", Id.ToString()));
            return file.FullName;
        }
        public static async Task<Response> PostToDo(TodoBase Todo)
        {
            await Task.Yield();
            if (Todo is null || string.IsNullOrEmpty(Todo.Title)
                            || Guid.Empty == Todo.Guid
                            || Todo.Subject is null
                            || Todo.Subject.Id <= 0
                            || Todo.Subject.IdTeacher <= 0)
            {
                return Response.Error;
            }
            string json_todo = Todo.JsonSerializeObject<TodoBase>();
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
            if (string.IsNullOrEmpty(Link.Url) || !Validations.IsValidUrl(Link.Url))
            {
                return Response.InvalidRequest;
            }

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
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET(
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
                var r= JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }
        internal static async Task<bool> DeleteLink(Link link,int UserId)
        {
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("DeleteLink",
                UserId.ToString(),link.Guid.ToString("N"));
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return false;
            }
            else
            {
                var r = JsonConvert.DeserializeObject<Response>(result.Response);
                return r.ResponseResult == APIResponseResult.OK;
            }
        }
        internal static async Task<List<Link>> GetLinks(string group, int TeacherId, int SubjectId)
        {
            if (string.IsNullOrEmpty(group) || TeacherId <= 0 || SubjectId <= 0)
            {
                return new List<Link>();
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("GetLinks",
                group, TeacherId.ToString(), SubjectId.ToString(),AppData.Instance.User.Id.ToString());
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<Link>();
            }
            Response response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<Link>(JsonConvert.DeserializeObject<Link[]>(response.Extra));
        }
    }
}