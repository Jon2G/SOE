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
using Device = Kit.Daemon.Devices.Device;

namespace SOE.API
{
    public static class APIService
    {
        public const string ShareTodo = "ShareTodo"; 
        public const string ShareReminder = "ShareReminder";
        //public const string NonHttpsUrl = "192.168.0.32:5555";
        public const string NonHttpsUrl = "kq8tb2poo8.execute-api.us-east-2.amazonaws.com";
        public static string BaseUrl => $"https://{NonHttpsUrl}";
        public static string Url => $"{BaseUrl}/Prod/App";

        public static async Task<Response> Login(string Usuario, string PasswordPin, string school = null)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin) || PasswordPin.Length < 8
                                              || (!Validations.IsValidEmail(Usuario) && !Validations.IsValidBoleta(Usuario)))
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("LogIn", Usuario, PasswordPin, Device.Current.DeviceId, school);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> SignUp(string PasswordPin, UserType Type,SOEWeb.Shared.Device Device)
        {
            WebService WebService = new WebService(Url);
            User User = AppData.Instance.User;
            if (string.IsNullOrEmpty(User.Boleta)
                || string.IsNullOrEmpty(PasswordPin)
                || PasswordPin.Length < 8
                || !Validations.IsValidEmail(User.Email)
                || !Validations.IsValidBoleta(User.Boleta)
                || User.School is null
                || string.IsNullOrEmpty(User.School.Name)
                || string.IsNullOrEmpty(Device.DeviceKey)
                || Type == UserType.INVALID)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("SignUp",
                User.Boleta, User.Name, User.Email,
                PasswordPin, User.School.Name, ((int)Type).ToString(), JsonConvert.SerializeObject(Device));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostClassTime(byte[] byteArray, string User)
        {
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
                        var file_result= await DownloadPictureById(Id);
                        Photos.Add(new PhotoArchive(file_result,FileType.Photo)); 
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
            FileInfo file= await Keeper.Save(webService.DownloadFile("GetArchieveById", Id.ToString()));
            return file.FullName;
        }

        public static async Task<Response> PostToDo(TodoBase Todo)
        {
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
            if (Reminder is null || string.IsNullOrEmpty(Reminder.Title)
                            || Guid.Empty == Reminder.Guid
                            || Reminder.Subject is null
                            || Reminder.Subject.Id <= 0
                            || Reminder.Subject.IdTeacher <= 0)
            {
                return Response.Error;
            }
            string json_Reminder = Reminder.JsonSerializeObject<Reminder>();
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
        internal static async Task<List<Classmate>> GetClassmates(string group)
        {
            if (string.IsNullOrEmpty(group))
            {
                return new List<Classmate>();
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("GetClassmates", group);
            if (result.Response == "ERROR" || string.IsNullOrEmpty(result.Response))
            {
                return new List<Classmate>();
            }
            var response = JsonConvert.DeserializeObject<Response>(result.Response);
            return new List<Classmate>(JsonConvert.DeserializeObject<Classmate[]>(response.Extra));
        }
    }
}