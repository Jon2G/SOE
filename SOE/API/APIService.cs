using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIModels;
using APIModels.Enums;
using Kit;
using Kit.Daemon.Devices;
using Kit.Services.Web;
using Kit.Sql.Base;
using Newtonsoft.Json;
using SOE.Data;
using SOE.Models.Data;
using Device = Kit.Daemon.Devices.Device;

namespace SOE.API
{
    public static class APIService
    {
        private const string Url = "https://aspnetclusters-36090-0.cloudclusters.net/AppAuthentication";
        //"https://192.168.0.32:44371/AppAuthentication";
        public static async Task<Response> Login(string Usuario, string PasswordPin, string school = null)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(PasswordPin) || PasswordPin.Length < 8
                                              || (!Validations.IsValidEmail(Usuario) && !Validations.IsValidBoleta(Usuario)))
            {
                return new Response(APIResponseResult.INVALID_REQUEST,
                    "!Solicitud invalida!");
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("LogIn", Usuario, PasswordPin,Device.Current.DeviceId, school);
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
        public static async Task<Response> PostClassTime(string HTML,string User)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(HTML))
            {
                return Response.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("PostClassTime",
                new Dictionary<string, string>() 
                {
                    {"HTML",HTML}
                }, User);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostGrades(string HTML)
        {
            WebService WebService = new WebService(Url);
            if (string.IsNullOrEmpty(HTML))
            {
                return Response.Error;
            }
            Kit.Services.Web.ResponseResult result = await WebService.GET("PostGrades",
                new Dictionary<string, string>()
                {
                    {"HTML",HTML}
                }, AppData.Instance.User.Boleta);
            if (result.Response == "ERROR")
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, result.Response);
            }
            return JsonConvert.DeserializeObject<Response>(result.Response);
        }
        public static async Task<Response> PostCareer(string CareerName,string User)
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
    }
}
