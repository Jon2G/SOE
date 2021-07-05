using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using APIModels.Enums;
using FFImageLoading;
using Kit;
using Kit.Daemon.Devices;
using Kit.Services.Web;
using Kit.Sql.Base;
using Newtonsoft.Json;
using SOE.Data;
using SOE.Models.Data;
using SOE.Models.TaskFirst;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.API
{
    public static class APIService
    {
        public const string ShareTodo = "ShareTodo";
        public const string NonHttpsUrl = "soe-api.azurewebsites.net";
        public static string BaseUrl => $"https://{NonHttpsUrl}";
        public static string Url => $"{BaseUrl}/AppAuthentication";

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
        public static async Task<Response> SignUp(string PasswordPin, UserType Type, APIModels.Device Device)
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

        internal static async Task<Response> DownloadSharedTodo(Guid TodoGuid, bool includeFiles)
        {
            if (Guid.Empty == TodoGuid)
            {
                return Response.Error;
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET("DownloadSharedTodo",
                TodoGuid.ToString("N"));
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            var response = JsonConvert.DeserializeObject<Response>(result.Response);
            if (!string.IsNullOrEmpty(response.Extra))
            {
                ToDo todo =  JsonConvert.DeserializeObject<ToDo>(response.Extra);
                todo.Id = 0;
               await ToDo.Save(todo, null);
            }
            return response;
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
            if (Todo.Date > DateTime.Now)
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "Esta tarea ya ha expirado, cambie la fecha de entrega si desea compartirla");
            }
            WebService WebService = new WebService(Url);
            Kit.Services.Web.ResponseResult result = await WebService.GET(
                "PostToDo", AppData.Instance.User.Boleta
                , JsonConvert.SerializeObject(Todo));
            if (result.Response == "ERROR")
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
                Img, "PostTodoPicture", AppData.Instance.User.Boleta, ToDoGuid.ToString());

            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }

            return JsonConvert.DeserializeObject<Response>(result.Response);
        }

    }
}